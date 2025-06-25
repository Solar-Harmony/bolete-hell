using BoleteHell.Code.Graphics;
using UnityEngine;

namespace BoleteHell.Code.Character
{
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(SpriteRuntimeFragmenter))]
    public class Player : MonoBehaviour
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

        private void OnGUI()
        {
            GUI.skin.label.fontSize = 32;
            GUI.Label(new Rect(10, 10, 300, 80), "Health: " + _health.CurrentHealth);
        }
    }
}