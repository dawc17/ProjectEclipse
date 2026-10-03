using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Eclipse.Multiplayer.Balance
{
    [Serializable, JsonObject(MemberSerialization.OptIn)]
    public sealed class PvpDamageOverride
    {
        [JsonProperty("id", Required = Required.Always)] public string Id = "";
        [JsonProperty("hitDamageScale")] public float? HitDamageScale;
        [JsonProperty("blockedDamageScale")] public float? BlockedDamageScale;
    }

    [Serializable, JsonObject(MemberSerialization.OptIn)]
    public sealed class PvpBalanceProfile
    {
        public const int MaxJsonBytes = 32768;
        [JsonProperty("schemaVersion", Required = Required.Always)] public int SchemaVersion = 1;
        [JsonProperty("id", Required = Required.Always)] public string Id = "default";
        [JsonProperty("name", Required = Required.Always)] public string Name = "Default";
        [JsonProperty("hitDamageScale")] public float HitDamageScale = .5f;
        [JsonProperty("blockedDamageScale")] public float BlockedDamageScale = .25f;
        [JsonProperty("recoverableBlockedFraction")] public float RecoverableBlockedFraction = 1f;
        [JsonProperty("recoverOnHit")] public float RecoverOnHit = .5f;
        [JsonProperty("recoverOnBlock")] public float RecoverOnBlock = .25f;
        [JsonProperty("recoverableLossOnHit")] public float RecoverableLossOnHit;
        [JsonProperty("minimumLifeOnBlock")] public float MinimumLifeOnBlock = .0001f;
        [JsonProperty("categories")] public PvpDamageOverride[] Categories = Array.Empty<PvpDamageOverride>();
        [JsonProperty("equipment")] public PvpDamageOverride[] Equipment = Array.Empty<PvpDamageOverride>();
        [JsonProperty("moves")] public PvpDamageOverride[] Moves = Array.Empty<PvpDamageOverride>();

        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Error,
            Culture = CultureInfo.InvariantCulture,
            MaxDepth = 12,
            FloatParseHandling = FloatParseHandling.Double,
        };

        public static PvpBalanceProfile Parse(string json)
        {
            if (json == null || Encoding.UTF8.GetByteCount(json) > MaxJsonBytes)
                throw new ArgumentException("A balance profile must fit in 32 KiB.");
            // Reject duplicate keys instead of silently accepting a different rule on each peer.
            var token = JObject.Parse(json, new JsonLoadSettings { DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error });
            ValidateJsonTypes(token, false);
            var profile = token.ToObject<PvpBalanceProfile>(JsonSerializer.Create(JsonSettings));
            profile.Validate();
            return profile;
        }

        private static void ValidateJsonTypes(JObject value, bool isOverride)
        {
            foreach (var property in value.Properties())
            {
                var type = property.Value.Type;
                if (property.Name == "id" || property.Name == "name")
                {
                    if (type != JTokenType.String) throw new ArgumentException(property.Name + " must be a JSON string.");
                }
                else if (property.Name == "schemaVersion")
                {
                    if (type != JTokenType.Integer) throw new ArgumentException("schemaVersion must be a JSON integer.");
                }
                else if (property.Name == "categories" || property.Name == "equipment" || property.Name == "moves")
                {
                    if (type != JTokenType.Array) throw new ArgumentException(property.Name + " must be a JSON array.");
                    foreach (var entry in (JArray)property.Value)
                    {
                        if (!(entry is JObject item)) throw new ArgumentException(property.Name + " entries must be JSON objects.");
                        ValidateJsonTypes(item, true);
                    }
                }
                else if (type != JTokenType.Integer && type != JTokenType.Float && !(isOverride && type == JTokenType.Null))
                    throw new ArgumentException(property.Name + " must be a JSON number.");
            }
        }

        public string ToJson(bool pretty = true)
        {
            Validate();
            var json = JsonConvert.SerializeObject(this, pretty ? Formatting.Indented : Formatting.None, JsonSettings);
            if (Encoding.UTF8.GetByteCount(json) > MaxJsonBytes) throw new ArgumentException("A balance profile must fit in 32 KiB.");
            return json;
        }

        public PvpBalanceSnapshot Compile() => new PvpBalanceSnapshot(this);

        public void Validate()
        {
            if (SchemaVersion != 1) throw new ArgumentException("Unsupported PvP balance schema version.");
            if (string.IsNullOrWhiteSpace(Id) || Id.Length > 64 || Id.Any(c => !char.IsLetterOrDigit(c) && c != '-' && c != '_'))
                throw new ArgumentException("Profile id must contain 1–64 letters, digits, hyphens or underscores.");
            if (string.IsNullOrWhiteSpace(Name) || Name.Length > 64) throw new ArgumentException("Profile name must contain 1–64 characters.");
            Range(HitDamageScale, 0, 10, "hitDamageScale");
            Range(BlockedDamageScale, 0, 10, "blockedDamageScale");
            Range(RecoverableBlockedFraction, 0, 1, "recoverableBlockedFraction");
            Range(RecoverOnHit, 0, 10, "recoverOnHit");
            Range(RecoverOnBlock, 0, 10, "recoverOnBlock");
            Range(RecoverableLossOnHit, 0, 1, "recoverableLossOnHit");
            Range(MinimumLifeOnBlock, .000001f, .1f, "minimumLifeOnBlock");
            ValidateOverrides(Categories, "categories");
            ValidateOverrides(Equipment, "equipment");
            ValidateOverrides(Moves, "moves");
        }

        private static void ValidateOverrides(PvpDamageOverride[] entries, string label)
        {
            if (entries == null || entries.Length > 256) throw new ArgumentException(label + " must be an array with at most 256 entries.");
            var ids = new HashSet<string>(StringComparer.Ordinal);
            foreach (var entry in entries)
            {
                if (entry == null || string.IsNullOrWhiteSpace(entry.Id) || entry.Id.Length > 128 || !ids.Add(entry.Id))
                    throw new ArgumentException(label + " contains an empty, duplicate or overlong id.");
                if (!entry.HitDamageScale.HasValue && !entry.BlockedDamageScale.HasValue)
                    throw new ArgumentException(label + ": " + entry.Id + " must override at least one value.");
                if (entry.HitDamageScale.HasValue) Range(entry.HitDamageScale.Value, 0, 10, label + ".hitDamageScale");
                if (entry.BlockedDamageScale.HasValue) Range(entry.BlockedDamageScale.Value, 0, 10, label + ".blockedDamageScale");
            }
        }

        private static void Range(float value, float min, float max, string label)
        {
            if (float.IsNaN(value) || float.IsInfinity(value) || value < min || value > max)
                throw new ArgumentException(label + " must be finite and between " + min.ToString(CultureInfo.InvariantCulture) + " and " + max.ToString(CultureInfo.InvariantCulture) + ".");
        }
    }

    public readonly struct PvpDamageRules
    {
        public readonly float HitDamageScale, BlockedDamageScale;
        public readonly string HitSource, BlockedSource;
        internal PvpDamageRules(float hit, float block, string hitSource, string blockSource)
        { HitDamageScale = hit; BlockedDamageScale = block; HitSource = hitSource; BlockedSource = blockSource; }
    }

    /// <summary>Immutable match rules. Authored objects and JSON cannot mutate a running fight.</summary>
    public sealed class PvpBalanceSnapshot
    {
        private readonly PvpBalanceProfile data;
        private readonly Dictionary<string, PvpDamageOverride> categories, equipment, moves;
        public string Id => data.Id;
        public string Name => data.Name;
        public string Hash { get; }
        public float RecoverableBlockedFraction => data.RecoverableBlockedFraction;
        public float RecoverOnHit => data.RecoverOnHit;
        public float RecoverOnBlock => data.RecoverOnBlock;
        public float RecoverableLossOnHit => data.RecoverableLossOnHit;
        public float MinimumLifeOnBlock => data.MinimumLifeOnBlock;

        internal PvpBalanceSnapshot(PvpBalanceProfile profile)
        {
            data = PvpBalanceProfile.Parse(profile.ToJson(false));
            data.Categories = Sorted(data.Categories);
            data.Equipment = Sorted(data.Equipment);
            data.Moves = Sorted(data.Moves);
            categories = data.Categories.ToDictionary(x => x.Id, StringComparer.Ordinal);
            equipment = data.Equipment.ToDictionary(x => x.Id, StringComparer.Ordinal);
            moves = data.Moves.ToDictionary(x => x.Id, StringComparer.Ordinal);
            // Names, whitespace, JSON property order and override array order are not gameplay.
            var canonical = Export();
            canonical.Id = "rules"; canonical.Name = "Rules";
            using (var sha = SHA256.Create())
                Hash = BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(canonical.ToJson(false)))).Replace("-", "").ToLowerInvariant();
        }

        private static PvpDamageOverride[] Sorted(PvpDamageOverride[] values) => values.OrderBy(x => x.Id, StringComparer.Ordinal).ToArray();
        public PvpBalanceProfile Export() => PvpBalanceProfile.Parse(data.ToJson(false));
        public string ToJson() => data.ToJson(false);

        public PvpDamageRules Resolve(string category, string item, string move)
        {
            float hit = data.HitDamageScale, block = data.BlockedDamageScale;
            string hitSource = "Global", blockSource = "Global";
            Apply(categories, category, "Category", ref hit, ref block, ref hitSource, ref blockSource);
            Apply(equipment, item, "Equipment", ref hit, ref block, ref hitSource, ref blockSource);
            Apply(moves, move, "Move", ref hit, ref block, ref hitSource, ref blockSource);
            return new PvpDamageRules(hit, block, hitSource, blockSource);
        }

        private static void Apply(Dictionary<string, PvpDamageOverride> entries, string id, string label,
            ref float hit, ref float block, ref string hitSource, ref string blockSource)
        {
            if (id == null || !entries.TryGetValue(id, out var entry)) return;
            if (entry.HitDamageScale.HasValue) { hit = entry.HitDamageScale.Value; hitSource = label + ": " + id; }
            if (entry.BlockedDamageScale.HasValue) { block = entry.BlockedDamageScale.Value; blockSource = label + ": " + id; }
        }
    }

    /// <summary>Pure single-bar health math, shared with tests. Blocked damage cannot knock out.</summary>
    public static class PvpRecoverableHealth
    {
        public static float ClampBlockedDamage(float damage, float life, float maxLife, float minimumFraction)
        {
            if (life <= 0 || maxLife <= 0 || damage <= 0) return 0;
            // Never raise a fighter already below the configured floor.
            float floor = Math.Min(life, maxLife * minimumFraction);
            return Math.Min(damage, Math.Max(0, life - floor));
        }

        public static float PoolAfterDamage(float pool, float lifeAfter, float maxLife, float actualDamage, bool blocked, PvpBalanceSnapshot rules)
        {
            if (lifeAfter <= 0 || maxLife <= 0) return 0;
            float next = blocked ? pool + actualDamage * rules.RecoverableBlockedFraction : pool - actualDamage * rules.RecoverableLossOnHit;
            return Math.Max(0, Math.Min(maxLife - lifeAfter, next));
        }

        public static float Recovery(float pool, float life, float maxLife, float inflictedDamage, bool blocked, PvpBalanceSnapshot rules)
        {
            if (life <= 0 || pool <= 0 || inflictedDamage <= 0) return 0;
            return Math.Max(0, Math.Min(Math.Min(pool, maxLife - life), inflictedDamage * (blocked ? rules.RecoverOnBlock : rules.RecoverOnHit)));
        }
    }
}
