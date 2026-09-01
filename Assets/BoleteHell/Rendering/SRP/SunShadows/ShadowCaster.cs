using Unity.Mathematics;
using UnityEngine;

namespace BoleteHell.Rendering.SRP.SunShadows
{
    [RequireComponent(typeof(MeshRenderer))]
    public class ShadowCaster : MonoBehaviour
    {
        [SerializeField]
        [Range(0.01f, 1)]
        private float _height = 0.5f;

        private void Awake()      => Apply();
        private void OnEnable()   => Apply();
        private void OnValidate() => Apply();

        private void Apply()
        {
            GetComponent<MeshRenderer>().SetShaderUserValue(math.asuint(_height));
        }
    }
}