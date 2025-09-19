using System;
using BoleteHell.Code.Gameplay.GameState;
using Unity.Behavior;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Gameplay.Character
{
    public class Enemy : Character
    {
        private Camera _mainCamera;
        public float visionRange;
        public float attackRange;

        [Inject]
        private IGameOutcomeService _outcome;

        private BehaviorGraphAgent _agent;

        protected override void Awake()
        {
            base.Awake(); // TODO: I hate this so much
            _mainCamera = Camera.main;
            _agent = GetComponent<BehaviorGraphAgent>();
            _agent.BlackboardReference.SetVariableValue("SelfCharacter", this);
            _agent.BlackboardReference.SetVariableValue("Self", gameObject);

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