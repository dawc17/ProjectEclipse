using UnityEngine;

namespace Eclipse.Content
{
    // A native asset container keeps logical sprite names independent of its filename.
    public sealed class NativeSpriteAtlas : ScriptableObject
    {
        public Sprite[] sprites;
    }
}
