using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

namespace BoleteHell.Code.Utils
{
    // Useful class for testing lighting when LD
    public class LightFollowCursor : MonoBehaviour
    {
        [SerializeField]
        private Light2D _light;
        
        [SerializeField]
        private Camera _mainCamera;
        
        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.V))
            {
                _light.enabled = !_light.enabled;
            }
            
            if (!_light.enabled)
                return;
            
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 worldPos = _mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, _mainCamera.nearClipPlane));
            _light.transform.position = new Vector3(worldPos.x, worldPos.y, _light.transform.position.z);
            
            float scroll = Mouse.current.scroll.ReadValue().y;
            if (scroll != 0)
            {
                _light.pointLightOuterRadius += scroll * Time.deltaTime;
            }
        }
    }
}