using System.Collections.Generic;
using Eclipse.Modding;
using UnityEngine;

namespace Eclipse.Rendering
{
    // One arena-owned system; drops and pools remain in its coordinate space
    // while fighters, the camera and the render root move independently.
    public sealed class FloorStainEffects : MonoBehaviour
    {
        private const int MaxAirborne = 128, MaxLanded = 512, MaxCached = 128;
        private const float MaxFlightSeconds = 8f;
        private sealed class Drop
        {
            public ModFxDefinition Definition;
            public SpriteRenderer Renderer;
            public float X, Y, Vx, Vy, Floor, Up, Size, Age;
            public float Flip;
        }

        private readonly List<Drop> _airborne = new List<Drop>();
        private readonly List<Drop> _landed = new List<Drop>();
        private readonly Stack<SpriteRenderer> _pool = new Stack<SpriteRenderer>();
        private Material _alpha, _additive;
        private bool _shaderChecked;

        public static FloorStainEffects For(Transform container)
            => container.GetComponent<FloorStainEffects>() ?? container.gameObject.AddComponent<FloorStainEffects>();

        public void Clear()
        {
            foreach (Drop drop in _airborne) Recycle(drop);
            foreach (Drop drop in _landed) Recycle(drop);
            _airborne.Clear();
            _landed.Clear();
        }

        private bool PrepareMaterials()
        {
            if (_shaderChecked) return _alpha != null;
            _shaderChecked = true;
            Shader shader = Resources.Load<Shader>("shaders/EclipseFloorDecal");
            if (shader == null || !shader.isSupported)
            {
                Debug.LogWarning("[Eclipse] Floor decal shader unavailable; floor stains are disabled.");
                return false;
            }
            _alpha = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
            _additive = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
            _additive.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.One);
            return true;
        }

        // All values are local arena coordinates; up is +1 or -1 for mirrored arenas.
        public void Spawn(ModFxDefinition definition, Vector3 point, Vector2 impulse,
            float floor, float up, Sprite sprite, float size, Color color, float strength)
        {
            if (!PrepareMaterials()) return;
            SpriteRenderer renderer = _pool.Count > 0 ? _pool.Pop() : CreateRenderer();
            renderer.gameObject.SetActive(true);
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.transform.localRotation = Quaternion.identity;
            var drop = new Drop { Definition = definition, Renderer = renderer, X = point.x,
                Y = Mathf.Max(point.y * up, floor * up), Floor = floor * up, Up = up,
                Size = size, Flip = Random.value < 0.5f ? -1f : 1f };
            renderer.transform.localPosition = new Vector3(drop.X, drop.Y * up, point.z);
            if (definition.Number("speed_max") <= 0f)
            {
                drop.X += Random.Range(-definition.Number("spread"), definition.Number("spread"));
                Land(drop);
                return;
            }

            Vector2 direction = new Vector2(impulse.x, impulse.y * up);
            if (direction.sqrMagnitude < 1e-5f) direction = Vector2.up;
            direction.Normalize();
            float angle = Mathf.Atan2(direction.y, direction.x) + Random.Range(-0.45f, 0.45f);
            float speed = Random.Range(definition.Number("speed_min"), definition.Number("speed_max")) * strength;
            drop.Vx = Mathf.Cos(angle) * speed;
            drop.Vy = Mathf.Sin(angle) * speed + definition.Number("lift");
            // Spread changes the trajectory, never teleports the landed splat.
            drop.Vx += Random.Range(-definition.Number("spread"), definition.Number("spread"));
            renderer.sharedMaterial = FxBuilder.MaterialFor(sprite.texture, definition.Blend);
            renderer.sortingOrder = 0;
            // Keep the spray readable at normal fight zoom; the old 22% width
            // reduced ordinary drops to barely visible specks.
            Scale(drop, size * 0.75f, 0.8f);
            if (_airborne.Count >= MaxAirborne) { Recycle(_airborne[0]); _airborne.RemoveAt(0); }
            _airborne.Add(drop);
        }

        private SpriteRenderer CreateRenderer()
        {
            var renderer = new GameObject("Floor effect").AddComponent<SpriteRenderer>();
            renderer.transform.SetParent(transform, false);
            renderer.gameObject.layer = gameObject.layer;
            return renderer;
        }

        private void Update()
        {
            // Disabling a mod switch removes its existing drops and pools too.
            List<ModFxDefinition> active = ModVisuals.ActiveFx(ModFxKind.Stain);
            for (int i = _landed.Count - 1; i >= 0; i--)
                if (!active.Contains(_landed[i].Definition))
                { Recycle(_landed[i]); _landed.RemoveAt(i); }
            Fight fight = Fight.GetCurrentFight();
            bool advance = fight != null && !fight.IsPaused();
            for (int i = _airborne.Count - 1; i >= 0; i--)
            {
                Drop drop = _airborne[i];
                if (!active.Contains(drop.Definition))
                { Recycle(drop); _airborne.RemoveAt(i); continue; }
                if (!advance) continue;
                float seconds = Mathf.Min(Time.deltaTime, MaxFlightSeconds - drop.Age);
                drop.Age += seconds;
                bool landed = FloorStainMath.Advance(ref drop.X, ref drop.Y, drop.Vx, ref drop.Vy,
                    drop.Definition.Number("gravity"), drop.Floor, seconds);
                if (landed) { _airborne.RemoveAt(i); Land(drop); }
                else if (drop.Age >= MaxFlightSeconds) { Recycle(drop); _airborne.RemoveAt(i); }
                else
                {
                    Vector3 position = drop.Renderer.transform.localPosition;
                    drop.Renderer.transform.localPosition = new Vector3(drop.X, drop.Y * drop.Up, position.z);
                    drop.Renderer.transform.localRotation = Quaternion.Euler(0f, 0f,
                        Mathf.Atan2(drop.Vy * drop.Up, drop.Vx) * Mathf.Rad2Deg);
                }
            }
        }

        private void Land(Drop drop)
        {
            ModFxDefinition d = drop.Definition;
            float radius = d.Number("merge_radius");
            Drop nearest = null;
            float distance = radius;
            // Definitions never merge across mods, colours, sprites or blend modes.
            foreach (Drop pool in _landed)
            {
                if (pool.Definition != d || pool.Size >= d.Number("max_pool_size")) continue;
                float dx = Mathf.Abs(pool.X - drop.X);
                if (radius > 0f && dx <= distance && Mathf.Abs(pool.Floor - drop.Floor) < 1f)
                { nearest = pool; distance = dx; }
            }
            if (nearest != null)
            {
                nearest.Size = FloorStainMath.MergeSize(nearest.Size, drop.Size, d.Number("max_pool_size"));
                Color tint = nearest.Renderer.color;
                tint.a = FloorStainMath.MergeAlpha(tint.a, drop.Renderer.color.a);
                nearest.Renderer.color = tint;
                Scale(nearest, nearest.Size, d.Number("flatten"));
                // Keep the original irregular silhouette and anchor; repeated hits
                // enlarge/darken it without sliding a pool across the floor.
                Recycle(drop);
                return;
            }
            drop.Y = drop.Floor;
            drop.Renderer.transform.localPosition = new Vector3(drop.X, drop.Floor * drop.Up, 0.06f);
            drop.Renderer.transform.localRotation = Quaternion.identity;
            drop.Renderer.sharedMaterial = d.Blend == ModFxBlend.Additive ? _additive : _alpha;
            drop.Renderer.sortingOrder = 1;
            Scale(drop, drop.Size, d.Number("flatten"));
            int count = 0, oldest = -1;
            for (int i = 0; i < _landed.Count; i++)
                if (_landed[i].Definition == d) { count++; if (oldest < 0) oldest = i; }
            if (count >= Mathf.RoundToInt(d.Number("limit")))
            { Recycle(_landed[oldest]); _landed.RemoveAt(oldest); }
            if (_landed.Count >= MaxLanded) { Recycle(_landed[0]); _landed.RemoveAt(0); }
            _landed.Add(drop);
        }

        private static void Scale(Drop drop, float size, float flatten)
        {
            Vector2 bounds = drop.Renderer.sprite.bounds.size;
            drop.Renderer.transform.localScale = new Vector3(drop.Flip * size / Mathf.Max(bounds.x, 1e-3f),
                size * flatten / Mathf.Max(bounds.y, 1e-3f), 1f);
        }

        private void Recycle(Drop drop)
        {
            if (drop.Renderer == null) return;
            drop.Renderer.gameObject.SetActive(false);
            if (_pool.Count < MaxCached) _pool.Push(drop.Renderer);
            else Destroy(drop.Renderer.gameObject);
        }

        private void OnDisable() { Clear(); }
        private void OnDestroy()
        {
            if (_alpha != null) Destroy(_alpha);
            if (_additive != null) Destroy(_additive);
        }
    }
}
