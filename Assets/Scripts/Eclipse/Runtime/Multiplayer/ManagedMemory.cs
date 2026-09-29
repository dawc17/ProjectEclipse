using System;
using System.Reflection;
using Unity.Collections.LowLevel.Unsafe;

namespace Eclipse.Multiplayer
{
    /// <summary>
    /// Direct reads and writes of managed object fields, for rollback snapshots, where
    /// reflection (and the boxing it does) costs far more than the copy itself on IL2CPP.
    /// <para>
    /// Relies on Unity's collector not moving objects (Boehm, in both Mono and IL2CPP),
    /// so an object's address is stable while it is referenced. Value bytes are copied
    /// directly; references are stored through a typed <c>ref object</c> so the
    /// incremental collector's write barrier still runs.
    /// </para>
    /// <see cref="Available"/> is false when a probe object's fields cannot be found
    /// where the runtime reports them; callers then keep using reflection.
    /// </summary>
    public static unsafe class ManagedMemory
    {
        private sealed class Probe
        {
            public int A;
            public object B;
            public long C;
            public byte D;
        }

        private static readonly int Adjust;

        /// <summary>True when field offsets have been confirmed on this runtime.</summary>
        public static bool Available { get; }

        static ManagedMemory()
        {
            try { Available = Calibrate(out Adjust); }
            catch (Exception) { Available = false; }
        }

        private static byte* Address(object target) => (byte*)UnsafeUtility.As<object, IntPtr>(ref target);

        /// <summary>
        /// Finds how the runtime's reported field offsets relate to the object reference
        /// (whether they already include the object header), using a probe with known values.
        /// </summary>
        private static bool Calibrate(out int adjust)
        {
            adjust = 0;
            var marker = new object();
            var probe = new Probe { A = 0x5A17C0DE, B = marker, C = 0x0123456789ABCDEFL, D = 0x7E };
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
            int a = UnsafeUtility.GetFieldOffset(typeof(Probe).GetField("A", flags));
            int b = UnsafeUtility.GetFieldOffset(typeof(Probe).GetField("B", flags));
            int c = UnsafeUtility.GetFieldOffset(typeof(Probe).GetField("C", flags));
            int d = UnsafeUtility.GetFieldOffset(typeof(Probe).GetField("D", flags));
            int header = 2 * IntPtr.Size;
            foreach (int candidate in new[] { 0, header, -header })
            {
                // Never read outside the probe: every candidate position must fall after the header.
                if (Math.Min(Math.Min(a, b), Math.Min(c, d)) + candidate < IntPtr.Size) continue;
                byte* data = Address(probe);
                if (*(int*)(data + a + candidate) != probe.A || *(long*)(data + c + candidate) != probe.C ||
                    *(data + d + candidate) != probe.D) continue;
                if (!ReferenceEquals(UnsafeUtility.As<IntPtr, object>(ref *(IntPtr*)(data + b + candidate)), marker)) continue;
                // Writes land where reflection reads them.
                var copy = new Probe();
                adjust = candidate;
                CopyBlocks(probe, copy, new[] { a + candidate, c + candidate, d + candidate }, new[] { 4, 8, 1 });
                CopyReferences(probe, copy, new[] { b + candidate });
                if (copy.A == probe.A && copy.C == probe.C && copy.D == probe.D && ReferenceEquals(copy.B, marker)) return true;
            }
            adjust = 0;
            return false;
        }

        /// <summary>The field's byte offset from the object reference, or -1.</summary>
        public static int FieldOffset(FieldInfo field)
        {
            if (!Available || field == null || field.IsStatic) return -1;
            try { return UnsafeUtility.GetFieldOffset(field) + Adjust; }
            catch (Exception) { return -1; }
        }

        /// <summary>The in-memory size of a value type, or -1.</summary>
        public static int SizeOf(Type type)
        {
            if (type == null || !type.IsValueType) return -1;
            if (type.IsEnum) type = Enum.GetUnderlyingType(type);
            try { return UnsafeUtility.SizeOf(type); }
            catch (Exception) { return -1; }
        }

        /// <summary>Copies byte ranges between two objects of the same type; the ranges must hold no references.</summary>
        public static void CopyBlocks(object from, object to, int[] offsets, int[] sizes)
        {
            byte* source = Address(from), target = Address(to);
            for (int i = 0; i < offsets.Length; i++)
            {
                int offset = offsets[i];
                switch (sizes[i])
                {
                    case 1: target[offset] = source[offset]; break;
                    case 2: *(short*)(target + offset) = *(short*)(source + offset); break;
                    case 4: *(int*)(target + offset) = *(int*)(source + offset); break;
                    case 8: *(long*)(target + offset) = *(long*)(source + offset); break;
                    default: UnsafeUtility.MemCpy(target + offset, source + offset, sizes[i]); break;
                }
            }
        }

        /// <summary>Copies reference fields between two objects of the same type.</summary>
        public static void CopyReferences(object from, object to, int[] offsets)
        {
            byte* source = Address(from), target = Address(to);
            for (int i = 0; i < offsets.Length; i++)
            {
                ref object slot = ref UnsafeUtility.As<IntPtr, object>(ref *(IntPtr*)(target + offsets[i]));
                slot = UnsafeUtility.As<IntPtr, object>(ref *(IntPtr*)(source + offsets[i]));
            }
        }

        public static object ReadReference(object target, int offset) =>
            UnsafeUtility.As<IntPtr, object>(ref *(IntPtr*)(Address(target) + offset));
    }
}
