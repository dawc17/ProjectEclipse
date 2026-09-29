using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Eclipse.Multiplayer.Online
{
    /// <summary>Decides what an <see cref="ObjectGraphSnapshotter"/> saves.</summary>
    public interface ISnapshotPolicy
    {
        /// <summary>
        /// Opaque objects are kept by reference and never saved or walked: immutable data
        /// (strings, delegates, shared definitions) and anything whose state lives outside
        /// managed fields.
        /// </summary>
        bool IsOpaque(Type type);
        /// <summary>False leaves a field untouched on restore and does not walk it.</summary>
        bool Captures(FieldInfo field);
        /// <summary>True when the type must be saved field by field instead of cloned.</summary>
        bool NeedsFieldCopy(Type type);
        /// <summary>Optional custom handling for a type, or null.</summary>
        SnapshotCodec CodecFor(Type type);
        /// <summary>
        /// Properties saved alongside the fields of a field-copied type, for state kept
        /// outside its own fields (an engine-backed value); null for none.
        /// </summary>
        PropertyInfo[] ExtraProperties(Type type);
    }

    /// <summary>Saves and restores one object in place, for types where reflection is too slow or incomplete.</summary>
    public abstract class SnapshotCodec
    {
        public abstract void Save(object target, SnapshotTape tape);
        public abstract void Load(object target, SnapshotTape tape);
        /// <summary>Passes every object reference the target holds that should be saved too.</summary>
        public virtual void Walk(object target, List<object> into) { }
    }

    /// <summary>Sequential storage for custom codecs; read back in the order written.</summary>
    public sealed class SnapshotTape
    {
        private float[] _floats = new float[256];
        private int[] _ints = new int[64];
        private object[] _objects = new object[16];
        private int _floatCount, _intCount, _objectCount;
        private int _floatRead, _intRead, _objectRead;

        public int FloatCount => _floatCount;

        public void Clear() { _floatCount = _intCount = 0; ClearObjects(); Rewind(); }
        public void Rewind() { _floatRead = _intRead = _objectRead = 0; }

        public void Float(float value)
        {
            if (_floatCount == _floats.Length) Array.Resize(ref _floats, _floats.Length * 2);
            _floats[_floatCount++] = value;
        }

        public void Int(int value)
        {
            if (_intCount == _ints.Length) Array.Resize(ref _ints, _ints.Length * 2);
            _ints[_intCount++] = value;
        }

        public void Object(object value)
        {
            if (_objectCount == _objects.Length) Array.Resize(ref _objects, _objects.Length * 2);
            _objects[_objectCount++] = value;
        }

        public float ReadFloat() => _floats[_floatRead++];
        public int ReadInt() => _ints[_intRead++];
        public object ReadObject() => _objects[_objectRead++];

        internal float FloatAt(int index) => _floats[index];

        private void ClearObjects()
        {
            Array.Clear(_objects, 0, _objectCount);
            _objectCount = 0;
        }
    }

    /// <summary>
    /// A saved object graph. Holding it keeps every saved object alive, so a restore
    /// can always write the saved references back.
    /// </summary>
    public sealed class StateSnapshot
    {
        internal readonly List<object> Objects = new List<object>(4096);
        internal readonly List<object> Saved = new List<object>(4096);
        internal readonly List<FieldInfo> StaticFields = new List<FieldInfo>();
        internal readonly List<object> StaticValues = new List<object>();
        internal readonly SnapshotTape Tape = new SnapshotTape();

        /// <summary>The tick this snapshot was taken before, or -1 when empty.</summary>
        public int Tick { get; internal set; } = -1;
        public int ObjectCount => Objects.Count;
        public bool IsEmpty => Tick < 0;

        public void Clear()
        {
            Objects.Clear();
            Saved.Clear();
            StaticFields.Clear();
            StaticValues.Clear();
            Tape.Clear();
            Tick = -1;
        }
    }

    /// <summary>
    /// Saves a managed object graph and restores it in place. Restoring writes the
    /// saved field values back into the same objects, so references held elsewhere
    /// (event listeners, cached nodes) stay valid. Objects created after the save are
    /// simply no longer referenced once it is restored.
    /// </summary>
    public sealed class ObjectGraphSnapshotter
    {
        private enum Kind : byte { Opaque, Class, FieldCopy, PrimitiveArray, ReferenceArray, StructArray, Custom }

        private sealed class TypeInfo
        {
            public Kind Kind;
            public FieldInfo[] Fields = Array.Empty<FieldInfo>();
            /// <summary>Fields that may hold references to walk (reference types, or structs holding them).</summary>
            public FieldInfo[] ReferenceFields = Array.Empty<FieldInfo>();
            public PropertyInfo[] Properties = Array.Empty<PropertyInfo>();
            public SnapshotCodec Codec;
            public TypeInfo Element;
        }

        private sealed class ReferenceComparer : IEqualityComparer<object>
        {
            public static readonly ReferenceComparer Instance = new ReferenceComparer();
            public new bool Equals(object a, object b) => ReferenceEquals(a, b);
            public int GetHashCode(object value) => RuntimeHelpers.GetHashCode(value);
        }

        private const BindingFlags InstanceFields = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
        private static readonly MethodInfo MemberwiseCloneMethod = typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);

        private readonly ISnapshotPolicy _policy;
        private readonly Dictionary<Type, TypeInfo> _types = new Dictionary<Type, TypeInfo>();
        private readonly Dictionary<Type, bool> _structHasReferences = new Dictionary<Type, bool>();
        private readonly HashSet<object> _visited = new HashSet<object>(ReferenceComparer.Instance);
        private readonly List<object> _pending = new List<object>(1024);
        private readonly List<object> _walk = new List<object>(64);
        private Func<object, object> _clone;

        public ObjectGraphSnapshotter(ISnapshotPolicy policy)
        {
            _policy = policy ?? throw new ArgumentNullException(nameof(policy));
            try { _clone = (Func<object, object>)Delegate.CreateDelegate(typeof(Func<object, object>), MemberwiseCloneMethod); }
            catch (Exception) { _clone = null; }
        }

        /// <summary>Objects saved by the most recent capture.</summary>
        public int LastObjectCount { get; private set; }

        /// <summary>Saves everything reachable from the roots and static fields into <paramref name="into"/>.</summary>
        public void Capture(StateSnapshot into, int tick, IList<object> roots, IList<FieldInfo> statics = null)
        {
            into.Clear();
            _visited.Clear();
            _pending.Clear();
            if (statics != null)
            {
                foreach (var field in statics)
                {
                    object value = field.GetValue(null);
                    into.StaticFields.Add(field);
                    into.StaticValues.Add(value);
                    WalkValue(value);
                }
            }
            foreach (var root in roots) WalkValue(root);
            while (_pending.Count > 0)
            {
                object target = _pending[_pending.Count - 1];
                _pending.RemoveAt(_pending.Count - 1);
                if (!_visited.Add(target)) continue;
                Save(into, target, Info(target.GetType()));
            }
            _visited.Clear();
            into.Tick = tick;
            LastObjectCount = into.Objects.Count;
        }

        /// <summary>Writes every saved value back into the objects it came from.</summary>
        public void Restore(StateSnapshot from)
        {
            if (from.IsEmpty) throw new InvalidOperationException("Nothing was saved in this snapshot.");
            for (int i = 0; i < from.StaticFields.Count; i++)
                if (!from.StaticFields[i].IsInitOnly) from.StaticFields[i].SetValue(null, from.StaticValues[i]);
            from.Tape.Rewind();
            var objects = from.Objects;
            var saved = from.Saved;
            for (int i = 0; i < objects.Count; i++)
            {
                object target = objects[i];
                var info = Info(target.GetType());
                switch (info.Kind)
                {
                    case Kind.Class:
                    case Kind.FieldCopy:
                        CopyFields(info, saved[i], target, info.Kind == Kind.FieldCopy);
                        break;
                    case Kind.PrimitiveArray:
                    case Kind.ReferenceArray:
                    case Kind.StructArray:
                        var source = (Array)saved[i];
                        Array.Copy(source, (Array)target, source.Length);
                        break;
                    case Kind.Custom:
                        info.Codec.Load(target, from.Tape);
                        break;
                }
            }
        }

        /// <summary>
        /// Describes the saved values that differ between two snapshots, or returns null
        /// when they match. Objects are matched by their position in the saved graph, so a
        /// tick that allocates fresh objects with the same contents still compares equal.
        /// Used to find state a rollback fails to restore.
        /// </summary>
        public string FirstDifference(StateSnapshot a, StateSnapshot b, int maxReports = 8)
        {
            var report = new System.Text.StringBuilder();
            int found = 0;
            var indexA = IndexOf(a);
            var indexB = IndexOf(b);
            if (a.Objects.Count != b.Objects.Count)
            {
                report.Append("object count ").Append(a.Objects.Count).Append(" vs ").Append(b.Objects.Count).Append("; ");
                found++;
            }
            int count = Math.Min(a.Objects.Count, b.Objects.Count);
            for (int i = 0; i < count && found < maxReports; i++)
            {
                object left = a.Objects[i], right = b.Objects[i];
                var type = left.GetType();
                if (type != right.GetType())
                {
                    report.Append("graph shape differs at #").Append(i).Append(" (").Append(type.Name).Append(" vs ")
                        .Append(right.GetType().Name).Append("); ");
                    found++;
                    break;
                }
                var info = Info(type);
                switch (info.Kind)
                {
                    case Kind.Class:
                    case Kind.FieldCopy:
                        for (int f = 0; f < info.Fields.Length + info.Properties.Length && found < maxReports; f++)
                        {
                            object x = SavedValue(info, a.Saved[i], f), y = SavedValue(info, b.Saved[i], f);
                            if (Same(x, y, indexA, indexB)) continue;
                            string name = f < info.Fields.Length ? info.Fields[f].Name : info.Properties[f - info.Fields.Length].Name;
                            report.Append(Describe(type)).Append('.').Append(name).Append(": ").Append(Show(x)).Append(" vs ").Append(Show(y)).Append("; ");
                            found++;
                        }
                        break;
                    case Kind.PrimitiveArray:
                    case Kind.ReferenceArray:
                    case Kind.StructArray:
                        var arrayA = (Array)a.Saved[i];
                        var arrayB = (Array)b.Saved[i];
                        if (arrayA.Length != arrayB.Length) { report.Append(Describe(type)).Append(" length differs; "); found++; break; }
                        for (int j = 0; j < arrayA.Length; j++)
                        {
                            if (Same(arrayA.GetValue(j), arrayB.GetValue(j), indexA, indexB)) continue;
                            report.Append(Describe(type)).Append('[').Append(j).Append("]: ").Append(Show(arrayA.GetValue(j)))
                                .Append(" vs ").Append(Show(arrayB.GetValue(j))).Append("; ");
                            found++;
                            break;
                        }
                        break;
                }
            }
            if (a.Tape.FloatCount != b.Tape.FloatCount) { report.Append("vector count differs; "); found++; }
            else
            {
                for (int i = 0; i < a.Tape.FloatCount && found < maxReports; i++)
                {
                    if (FloatBits(a.Tape.FloatAt(i)) == FloatBits(b.Tape.FloatAt(i))) continue;
                    report.Append("vector value #").Append(i).Append(": ").Append(a.Tape.FloatAt(i).ToString("R")).Append(" vs ")
                        .Append(b.Tape.FloatAt(i).ToString("R")).Append("; ");
                    found++;
                    break;
                }
            }
            return found == 0 ? null : report.ToString();
        }

        private static Dictionary<object, int> IndexOf(StateSnapshot snapshot)
        {
            var index = new Dictionary<object, int>(snapshot.Objects.Count, ReferenceComparer.Instance);
            for (int i = 0; i < snapshot.Objects.Count; i++) index[snapshot.Objects[i]] = i;
            return index;
        }

        private static object SavedValue(TypeInfo info, object saved, int slot)
        {
            if (info.Kind == Kind.FieldCopy) return ((object[])saved)[slot];
            return info.Fields[slot].GetValue(saved);
        }

        private static string Describe(Type type) => type.DeclaringType != null ? type.DeclaringType.Name + "." + type.Name : type.Name;

        /// <summary>Equal values, or references to objects at the same place in each graph.</summary>
        private static bool Same(object x, object y, Dictionary<object, int> indexA, Dictionary<object, int> indexB)
        {
            if (x == null || y == null) return x == null && y == null;
            var type = x.GetType();
            if (type != y.GetType()) return false;
            if (x is float fx) return FloatBits(fx) == FloatBits((float)y);
            if (x is double dx) return BitConverter.DoubleToInt64Bits(dx) == BitConverter.DoubleToInt64Bits((double)y);
            if (type.IsPrimitive || type.IsEnum || x is string) return x.Equals(y);
            if (x is Delegate dxd) return dxd.Method == ((Delegate)y).Method;
            if (type.IsValueType)
            {
                foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                    if (!Same(field.GetValue(x), field.GetValue(y), indexA, indexB)) return false;
                return true;
            }
            bool savedX = indexA.TryGetValue(x, out int at), savedY = indexB.TryGetValue(y, out int bt);
            if (savedX || savedY) return savedX && savedY && at == bt;
            // Neither was saved (opaque): the same object must be referenced.
            return ReferenceEquals(x, y);
        }

        private static string Show(object value)
        {
            if (value == null) return "null";
            if (value is float f) return f.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
            var type = value.GetType();
            if (type.IsPrimitive || type.IsEnum || value is string) return value.ToString();
            return type.Name;
        }

        private static int FloatBits(float value) => BitConverter.ToInt32(BitConverter.GetBytes(value), 0);

        private void Push(object value)
        {
            if (value == null) return;
            if (Info(value.GetType()).Kind == Kind.Opaque) return;
            _pending.Add(value);
        }

        private void Save(StateSnapshot into, object target, TypeInfo info)
        {
            switch (info.Kind)
            {
                case Kind.Class:
                    into.Objects.Add(target);
                    into.Saved.Add(Clone(target));
                    WalkFields(info, target);
                    break;
                case Kind.FieldCopy:
                    var values = new object[info.Fields.Length + info.Properties.Length];
                    for (int i = 0; i < info.Fields.Length; i++) values[i] = info.Fields[i].GetValue(target);
                    for (int i = 0; i < info.Properties.Length; i++) values[info.Fields.Length + i] = info.Properties[i].GetValue(target, null);
                    into.Objects.Add(target);
                    into.Saved.Add(values);
                    for (int i = 0; i < info.Fields.Length; i++)
                        if (MayHoldReferences(info.Fields[i].FieldType)) WalkValue(values[i]);
                    break;
                case Kind.PrimitiveArray:
                    into.Objects.Add(target);
                    into.Saved.Add(((Array)target).Clone());
                    break;
                case Kind.ReferenceArray:
                {
                    var array = (object[])((Array)target).Clone();
                    into.Objects.Add(target);
                    into.Saved.Add(array);
                    for (int i = 0; i < array.Length; i++) WalkValue(array[i]);
                    break;
                }
                case Kind.StructArray:
                {
                    var array = (Array)((Array)target).Clone();
                    into.Objects.Add(target);
                    into.Saved.Add(array);
                    for (int i = 0; i < array.Length; i++) WalkStruct(info.Element, array.GetValue(i));
                    break;
                }
                case Kind.Custom:
                    into.Objects.Add(target);
                    into.Saved.Add(null);
                    info.Codec.Save(target, into.Tape);
                    _walk.Clear();
                    info.Codec.Walk(target, _walk);
                    foreach (var child in _walk) Push(child);
                    _walk.Clear();
                    break;
            }
        }

        private object Clone(object target)
        {
            if (_clone != null)
            {
                try { return _clone(target); }
                catch (Exception) { _clone = null; }
            }
            return MemberwiseCloneMethod.Invoke(target, null);
        }

        private void WalkFields(TypeInfo info, object target)
        {
            foreach (var field in info.ReferenceFields) WalkValue(field.GetValue(target));
        }

        private void WalkValue(object value)
        {
            if (value == null) return;
            var type = value.GetType();
            if (type.IsValueType) WalkStruct(Info(type), value);
            else Push(value);
        }

        private void WalkStruct(TypeInfo info, object boxed)
        {
            if (boxed == null || info == null) return;
            foreach (var field in info.ReferenceFields) WalkValue(field.GetValue(boxed));
        }

        private static void CopyFields(TypeInfo info, object saved, object target, bool fromValues)
        {
            var fields = info.Fields;
            if (fromValues)
            {
                var values = (object[])saved;
                for (int i = 0; i < fields.Length; i++) fields[i].SetValue(target, values[i]);
                for (int i = 0; i < info.Properties.Length; i++) info.Properties[i].SetValue(target, values[fields.Length + i], null);
            }
            else
            {
                for (int i = 0; i < fields.Length; i++) fields[i].SetValue(target, fields[i].GetValue(saved));
            }
        }

        private TypeInfo Info(Type type)
        {
            if (_types.TryGetValue(type, out var info)) return info;
            info = new TypeInfo();
            _types[type] = info;
            if (type.IsPrimitive || type.IsEnum || type.IsPointer || type == typeof(string) || typeof(Delegate).IsAssignableFrom(type) ||
                typeof(System.Threading.WaitHandle).IsAssignableFrom(type) ||
                typeof(MemberInfo).IsAssignableFrom(type) || typeof(Type).IsAssignableFrom(type) || _policy.IsOpaque(type))
            {
                info.Kind = Kind.Opaque;
                return info;
            }
            var codec = _policy.CodecFor(type);
            if (codec != null)
            {
                info.Kind = Kind.Custom;
                info.Codec = codec;
                return info;
            }
            if (type.IsArray)
            {
                var element = type.GetElementType();
                if (type.GetArrayRank() != 1) { info.Kind = Kind.Opaque; return info; }
                if (!element.IsValueType) info.Kind = Kind.ReferenceArray;
                else if (StructHasReferences(element)) { info.Kind = Kind.StructArray; info.Element = Info(element); }
                else info.Kind = Kind.PrimitiveArray;
                return info;
            }
            var fields = new List<FieldInfo>();
            var references = new List<FieldInfo>();
            for (var current = type; current != null && current != typeof(object) && current != typeof(ValueType); current = current.BaseType)
            {
                foreach (var field in current.GetFields(InstanceFields))
                {
                    if (!_policy.Captures(field)) continue;
                    fields.Add(field);
                    if (MayHoldReferences(field.FieldType)) references.Add(field);
                }
            }
            info.Fields = fields.ToArray();
            info.ReferenceFields = references.ToArray();
            if (!type.IsValueType && _policy.NeedsFieldCopy(type)) info.Properties = _policy.ExtraProperties(type) ?? Array.Empty<PropertyInfo>();
            // A boxed struct is never mutated in place, so it is kept by reference; its
            // fields are still walked for the references it carries.
            info.Kind = type.IsValueType ? Kind.Opaque : _policy.NeedsFieldCopy(type) ? Kind.FieldCopy : Kind.Class;
            return info;
        }

        private bool MayHoldReferences(Type type)
        {
            if (type.IsPrimitive || type.IsEnum || type.IsPointer || type == typeof(string)) return false;
            if (!type.IsValueType) return true;
            return StructHasReferences(type);
        }

        private bool StructHasReferences(Type type)
        {
            if (type.IsPrimitive || type.IsEnum || type.IsPointer) return false;
            if (_structHasReferences.TryGetValue(type, out var known)) return known;
            _structHasReferences[type] = false;
            bool result = false;
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var fieldType = field.FieldType;
                if (fieldType == typeof(string)) continue;
                if (!fieldType.IsValueType || StructHasReferences(fieldType)) { result = true; break; }
            }
            _structHasReferences[type] = result;
            return result;
        }
    }
}
