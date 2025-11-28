using BoleteHell.Gameplay.Characters;
using UnityEngine;

namespace BoleteHell.Code.UI.Previz
{
    [RequireComponent(typeof(HealthComponent))]
    public class ShowEnemyHealth : MonoBehaviour
    {
        private Camera _camera;
        private HealthComponent _health;

        private void Awake()
        {
            _camera = Camera.main;
            _health = GetComponent<HealthComponent>();
        }

        private void OnGUI()
        {
            Vector2 position = new Vector2(transform.position.x, transform.position.y + GetComponent<SpriteRenderer>().bounds.size.y * 0.5f);
            Vector2 ss = _camera.WorldToScreenPoint(position);
            ss.y = Screen.height - ss.y;
            Rect rect = new(ss, new Vector2(100, 50));
            GUI.skin.label.fontSize = 24;
            GUI.Label(rect, _health.CurrentHealth + "hp");
        }
    }
}