using BoleteHell.Code.Gameplay.Destructible;
using BoleteHell.Code.Gameplay.Droppables;
using BoleteHell.Code.Gameplay.GameState;
using Unity.Behavior;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Gameplay.Characters
{
    public class Enemy : Character
    {
        [field: SerializeField]
        public bool isElite { get; private set; }
        
        public override FactionType faction { get; set; } = FactionType.Enemy;
        
        [SerializeField]
        private DropSettings _dropSettings;
        
        [SerializeField]
        private SpriteFragmentConfig spriteFragmentConfig;
        
        [Inject]
        private IGameOutcomeService _outcome;
        
        [Inject]
        private ISpriteFragmenter _spriteFragmenter;
        
        [Inject]
        public IDropManager dropManager { get; set; }

        private BehaviorGraphAgent _agent;
        private Camera _mainCamera;
        
        protected override void Awake()
        {
            base.Awake();
            _mainCamera = Camera.main;
            _agent = GetComponent<BehaviorGraphAgent>();
            
            Health.OnDeath += () =>
            {
                dropManager.DropDroplets(gameObject, _dropSettings.dropletContext);
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