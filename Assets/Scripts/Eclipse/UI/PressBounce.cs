using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Eclipse.UI
{
    // Special Edition style press feedback: the button squashes while held and springs back
    // with a small overshoot when released. Runs on unscaled time and animates around the
    // scale the button had when attached, so layout code keeps owning the base scale.
    [DisallowMultipleComponent]
    public sealed class PressBounce : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private const float PressedScale = .86f, PressRate = 28f, Stiffness = 420f, Damping = 24f;
        private Selectable selectable;
        private Vector3 baseScale = Vector3.one;
        private float scale = 1f, velocity;
        private bool held, animating;

        public static PressBounce Attach(GameObject target)
        {
            if (target == null) return null;
            var bounce = target.GetComponent<PressBounce>();
            return bounce != null ? bounce : target.AddComponent<PressBounce>();
        }

        private void Awake()
        {
            selectable = GetComponent<Selectable>();
            baseScale = transform.localScale;
        }

        public void OnPointerDown(PointerEventData data)
        {
            if (data.button != PointerEventData.InputButton.Left) return;
            if (selectable != null && !selectable.IsInteractable()) return;
            if (!animating) baseScale = transform.localScale;
            held = true;
            animating = true;
        }

        public void OnPointerUp(PointerEventData data)
        {
            if (data.button != PointerEventData.InputButton.Left) return;
            held = false;
        }

        private void OnDisable()
        {
            if (animating) transform.localScale = baseScale;
            held = false;
            animating = false;
            scale = 1f;
            velocity = 0f;
        }

        private void Update()
        {
            if (!animating) return;
            float dt = Mathf.Min(Time.unscaledDeltaTime, 1f / 30f);
            if (held)
            {
                scale = Mathf.Lerp(scale, PressedScale, 1f - Mathf.Exp(-PressRate * dt));
                velocity = 0f;
            }
            else
            {
                // Damped spring back to rest; slightly underdamped for the bounce.
                velocity += ((1f - scale) * Stiffness - velocity * Damping) * dt;
                scale += velocity * dt;
                if (Mathf.Abs(1f - scale) < .001f && Mathf.Abs(velocity) < .01f)
                {
                    scale = 1f;
                    velocity = 0f;
                    animating = false;
                }
            }
            transform.localScale = new Vector3(baseScale.x * scale, baseScale.y * scale, baseScale.z);
        }
    }
}
