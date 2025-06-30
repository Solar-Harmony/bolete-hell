using UnityEngine;

namespace BoleteHell.Code.Gameplay.Destructible
{
    public interface ISpriteFragmenter
    {
        void Fragment(Vector2 position, SpriteFragmentConfig config);
    }
}