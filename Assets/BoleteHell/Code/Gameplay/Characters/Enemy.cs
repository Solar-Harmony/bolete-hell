using BoleteHell.Code.Gameplay.Destructible;
using BoleteHell.Code.Gameplay.GameState;
using Sirenix.OdinInspector;
using Unity.Behavior;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Gameplay.Characters
{
    [RequireComponent(typeof(Arsenal.Arsenal))]
    public class Enemy : Character, ILootable
    {
        private Arsenal.Arsenal _weapon;
        private Camera _mainCamera;

        [Inject]
        private IGameOutcomeService _outcome;
        
        [Inject]
        private ISpriteFragmenter _spriteFragmenter;
        
        [field: SerializeField]
        public bool isElite { get; private set; }
        
        [SerializeField]
        private SpriteFragmentConfig spriteFragmentConfig;

        private BehaviorGraphAgent _agent;

        public override FactionType faction { get; set; } = FactionType.Enemy;
        
        [field:SerializeReference, HideReferenceObjectPicker]
        public DropContext dropContext { get; set; }

        public DropManager dropManager { get; set; }


        protected override void Awake()
        {
            base.Awake();
            _mainCamera = Camera.main;
            _weapon = GetComponent<Arsenal.Arsenal>();
            _agent = GetComponent<BehaviorGraphAgent>();
            dropManager = FindFirstObjectByType<DropManager>();
            
            Health.OnDeath += () =>
            {
                dropManager.DropDroplets(gameObject, dropContext.dropletContext);
                //dropManager.DropGold(this.gameObject, dropContext.goldContext);
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