using Graphics;
using UnityEngine;

namespace _BoleteHell.Code.Character
{
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(SpriteRuntimeFragmenter))]
    // TODO: We're starting to have way too many components on stuff
    public class EnemyHandler : MonoBehaviour
    {
        private Health _health;
        
        private void Awake()
        {
            _health = GetComponent<Health>();
            _health.OnDeath += () =>
            {
                var fragmenter = GetComponent<SpriteRuntimeFragmenter>();
                fragmenter.Fragment();
            };
        }
    }
}