using UnityEngine;

namespace BoleteHell.Utils.Level
{
    [RequireComponent(typeof(Collider2D), typeof(Renderer))]
    public class LevelCeiling : MonoBehaviour
    {
        private Renderer _renderer;
        
        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            _renderer.enabled = false;
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            _renderer.enabled = true;
        }
    }
}