using BoleteHell.Gameplay.Destructible;
using UnityEngine;
using Zenject;

namespace BoleteHell.Gameplay.Characters
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(HealthComponent))]
    public class FragmentOnDeath : MonoBehaviour
    {
        [Inject]
        private ISpriteFragmenter _spriteFragmenter;
        
        [SerializeField]
        private SpriteFragmentConfig _spriteFragmentConfig;

        private HealthComponent _health;

        private void Awake()
        {
            _health = GetComponent<HealthComponent>();
        }

        private void OnEnable()
        {
            _health.OnDeath += OnDeath;
        }

        private void OnDeath()
        {
            _spriteFragmenter.Fragment(transform, _spriteFragmentConfig);
        }

        private void OnDisable()
        {
            _health.OnDeath -= OnDeath;
        }
    }
}