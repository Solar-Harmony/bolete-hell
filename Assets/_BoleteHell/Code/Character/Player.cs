using System;
using _BoleteHell.Code.Character;
using Graphics;
using UnityEngine;

namespace _BoleteHell.Code.Player
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
                fragmenter.Fragment(Vector3.zero); // TODO: find way to retrieve normal...
            };
        }

        private void OnGUI()
        {
            GUI.skin.label.fontSize = 32;
            GUI.Label(new Rect(10, 10, 300, 80), "Health: " + _health.CurrentHealth);
        }
    }
}