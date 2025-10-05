using BoleteHell.Code.Gameplay.Destructible;
using BoleteHell.Code.Gameplay.GameState;
using Unity.Behavior;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Gameplay.Character
{
    [RequireComponent(typeof(Arsenal.Arsenal))]
    public class Enemy : Character
    {
        
        private Arsenal.Arsenal _weapon;
        private Camera _mainCamera;

        [Inject]
        private IGameOutcomeService _outcome;
        
        [Inject]
        private ISpriteFragmenter _spriteFragmenter;
        
        [SerializeField]
        private SpriteFragmentConfig spriteFragmentConfig;

        private BehaviorGraphAgent _agent;

        public override FactionType faction { get; set; } = FactionType.Enemy;

        protected override void Awake()
        {
            base.Awake(); // TODO: I hate this so much
            _mainCamera = Camera.main;
            _weapon = GetComponent<Arsenal.Arsenal>();
            _agent = GetComponent<BehaviorGraphAgent>();
            
            Health.OnDeath += () =>
            {
                gameObject.SetActive(false);
                Destroy(gameObject);
                _spriteFragmenter.Fragment(transform, spriteFragmentConfig);
            };
            
            _outcome.OnDefeat += OnDefeat;
        }

        private void OnDefeat(string reason)
        {
            _agent.Graph.End();
            _agent.enabled = false;
        }
        
        private void OnGUI()
        {
            Vector2 position = new Vector2(transform.position.x, transform.position.y + GetComponent<SpriteRenderer>().bounds.size.y * 0.5f);
            Vector2 ss = _mainCamera.WorldToScreenPoint(position);
            ss.y = Screen.height - ss.y;
            Rect rect = new(ss, new Vector2(100, 50));
            GUI.skin.label.fontSize = 24;
            GUI.Label(rect, Health.CurrentHealth + "hp");
        }

        private void OnDestroy()
        {
            _outcome.OnDefeat -= OnDefeat;
        }
    }
}