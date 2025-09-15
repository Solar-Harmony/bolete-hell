using UnityEngine;

namespace BoleteHell.Gameplay.Destructible
{
    public interface ISpriteFragmenter
    {
        void Fragment(Transform transform, SpriteFragmentConfig config);
    }
}