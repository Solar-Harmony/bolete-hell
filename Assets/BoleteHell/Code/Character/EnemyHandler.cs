using Graphics;
using UnityEngine;

namespace _BoleteHell.Code.Character
{
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(SpriteRuntimeFragmenter))]
    public class EnemyHandler : MonoBehaviour
    {
        private Health _health;
        private Camera _mainCamera;
        
        private void Awake()
        {
            _mainCamera = Camera.main;
            _health = GetComponent<Health>();
            _health.OnDeath += () =>
            {
                var fragmenter = GetComponent<SpriteRuntimeFragmenter>();
                fragmenter.Fragment();
            };
        }
        
        private void OnGUI()
        {
            Vector2 position = new Vector2(transform.position.x, transform.position.y + GetComponent<SpriteRenderer>().bounds.size.y * 0.5f);
            Vector2 ss = _mainCamera.WorldToScreenPoint(position);
            ss.y = Screen.height - ss.y;
            Rect rect = new(ss, new Vector2(100, 50));
            GUI.skin.label.fontSize = 24;
            GUI.Label(rect, _health.CurrentHealth + "hp");
        }
    }
}