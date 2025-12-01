using BoleteHell.Code.Graphics;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace BoleteHell.Gameplay.Destructible
{
    [UsedImplicitly]
    public class SpriteFragmenter : ISpriteFragmenter
    {
        [Inject]
        private SpriteFragment.Pool _spriteFragmentPool;

        [Inject]
        private TransientLight.Pool _transientLightPool;
        
        public void Fragment(Transform transform, SpriteFragmentConfig config)
        {
            _transientLightPool.Spawn(transform.position, 3.0f, 0.2f);
            
            var sprite = config.sr.sprite;

            Vector2 spriteSize = sprite.bounds.size;
            var fragmentSize = new Vector2(spriteSize.x / config.fragmentsX, spriteSize.y / config.fragmentsY);
            var origin = (Vector2)transform.position - spriteSize / 2;

            for (var y = 0; y < config.fragmentsY; y++)
            for (var x = 0; x < config.fragmentsX; x++)
            {
                var pos = origin + new Vector2((x + 0.5f) * fragmentSize.x, (y + 0.5f) * fragmentSize.y);
                _spriteFragmentPool.Spawn(
                    pos,
                    config,
                    new SpriteFragmentParams(
                        x, 
                        y, 
                        fragmentSize, 
                        transform.localScale, 
                        4.0f
                    )
                );
            }
        }
    }
}