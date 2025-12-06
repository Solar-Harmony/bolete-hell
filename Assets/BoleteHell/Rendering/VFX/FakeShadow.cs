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

        [SerializeField]
        private float _shadowLength = 0.5f;

        private void Awake()
        {
            Debug.Assert(_shadowObject);
        }
        
        private void LateUpdate()
        {
            Vector3 shadowOffset = _shadowDirection.normalized * _shadowLength;
            _shadowObject.transform.position = _shadowObject.transform.parent.position + shadowOffset;
        }
    }
}