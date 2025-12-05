using BoleteHell.Utils;
using UnityEngine;

namespace BoleteHell.Rendering.VFX
{
    public class FakeShadow : MonoBehaviour
    {
        [SerializeField]
        private GameObject _shadowObject;

        // TODO: Synchronize with render feature settings
        [SerializeField]
        [AnglePicker]
        private Vector2 _shadowDirection = new(0.1f, 0.1f);

        private void Awake()
        {
            Debug.Assert(_shadowObject);
        }

        private void Update()
        {
            Vector3 shadowOffset = new Vector3(_shadowDirection.x, 0.0f, _shadowDirection.y).normalized * 0.1f;
            _shadowObject.transform.position = shadowOffset;
        }
    }
}