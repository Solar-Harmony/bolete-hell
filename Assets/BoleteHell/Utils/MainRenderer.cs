using UnityEngine;

namespace BoleteHell.Utils
{
    // For objects that have multiple renderers in sub-objects and we need to quickly get the main one.
    public class MainRenderer : MonoBehaviour
    {
        [field: SerializeField]
        public Renderer Renderer { get; private set; }

        private void Awake()
        {
            Debug.Assert(Renderer);
        }
    }
}