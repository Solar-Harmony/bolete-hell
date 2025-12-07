using BoleteHell.Gameplay.Characters;
using UnityEngine;

namespace BoleteHell.UI.Previz
{
    [RequireComponent(typeof(HealthComponent))]
    public class ShowHealth : MonoBehaviour
    {
        private Camera _camera;
        private HealthComponent _health;
        
        [SerializeField]
        private Renderer _renderer;

        private void Awake()
        {
            _camera = Camera.main;
            _health = GetComponent<HealthComponent>();
        }

        private void OnGUI()
        {
            Vector3 worldPosition = new Vector3(transform.position.x, transform.position.y + _renderer.bounds.size.y * 0.5f, transform.position.z);
            Vector2 ss = _camera.WorldToScreenPoint(worldPosition);
            ss.y = Screen.height - ss.y;
            const float labelWidth = 100f;
            const float labelHeight = 50f;
            Rect rect = new(ss.x - labelWidth * 0.5f, ss.y - labelHeight, labelWidth, labelHeight);
            GUI.skin.label.fontSize = 24;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            
            string healthText = _health.CurrentHealth + "hp";
            
            GUI.color = Color.black;
            GUI.Label(new Rect(rect.x - 2, rect.y - 2, rect.width, rect.height), healthText);
            GUI.Label(new Rect(rect.x + 2, rect.y - 2, rect.width, rect.height), healthText);
            GUI.Label(new Rect(rect.x - 2, rect.y + 2, rect.width, rect.height), healthText);
            GUI.Label(new Rect(rect.x + 2, rect.y + 2, rect.width, rect.height), healthText);
            
            GUI.color = Color.white;
            GUI.Label(rect, healthText);
        }
    }
}