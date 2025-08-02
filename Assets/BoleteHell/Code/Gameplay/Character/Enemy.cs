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

        protected override void Awake()
        {
            base.Awake(); // TODO: I hate this so much
            _mainCamera = Camera.main;
            _weapon = GetComponent<Arsenal.Arsenal>();
            
            _outcome.OnDefeat += reason =>
            {
                if (TryGetComponent(out BehaviorGraphAgent graph))
                {
                    graph.enabled = false;
                }
            };
        }
        
        public void Shoot(Vector3 direction)
        {
            _weapon.Shoot( direction);
        }
        
        public float GetProjectileSpeed()
        {
            if (!_weapon) return 0.0f;
            return _weapon.GetSelectedWeapon().cannonData.projectileSpeed;
        }
        
        private void OnGUI()
        {
            Vector2 position = new Vector2(transform.position.x, transform.position.y + GetComponent<SpriteRenderer>().bounds.size.y * 0.5f);
            Vector2 ss = _mainCamera.WorldToScreenPoint(position);
            ss.y = Screen.height - ss.y;
            Rect rect = new(ss, new Vector2(100, 50));
            GUI.skin.label.fontSize = 24;
            GUI.Label(rect, health.CurrentHealth + "hp");
        }
    }
}