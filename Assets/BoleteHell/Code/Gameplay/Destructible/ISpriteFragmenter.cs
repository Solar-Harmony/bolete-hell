using UnityEngine;

namespace BoleteHell.Code.Gameplay.Destructible
{
    public interface ISpriteFragmenter
    {
        void Fragment(Transform transform, SpriteFragmentConfig config);
    }
}