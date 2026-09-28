using UnityEngine;

namespace Eclipse.Rendering
{
    // Scene-owned native dojo scenery, including animated layers and mod atmosphere.
    // Kept separate from ModelContainer so changing items does not rebuild the location.
    public sealed class ShopDojoBackdrop : MonoBehaviour
    {
        private Render backdrop;
        private GameObject root;

        public static void Attach(GameObject owner, SpriteRenderer left, SpriteRenderer right)
        {
            if (owner.GetComponent<ShopDojoBackdrop>() != null) return;
            var preview = owner.AddComponent<ShopDojoBackdrop>();
            var location = Location.CreateDojoPreview();
            if (location.layers == null || location.layers.Count == 0 || location.gameLayer == null)
            {
                Destroy(preview);
                return;
            }
            preview.root = new GameObject("Shop dojo scenery");
            preview.backdrop = new Render(preview.root);
            preview.backdrop.Init(location);
            preview.backdrop.UpdateMenuBackdrop(UnityEngine.Camera.main, false);
            if (left != null) left.enabled = false;
            if (right != null) right.enabled = false;
        }

        private void LateUpdate() { backdrop?.UpdateMenuBackdrop(UnityEngine.Camera.main, false); }
        private void FixedUpdate() { backdrop?.UpdateMenuBackdrop(UnityEngine.Camera.main, true); }

        private void OnDestroy()
        {
            backdrop?.DestroyMenuBackdrop();
            if (root != null) Destroy(root);
        }
    }
}
