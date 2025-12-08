using BoleteHell.Utils.Extensions;
using Dreamteck.Splines;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BoleteHell.Utils.Progress
{
    public class BossTimer : MonoBehaviour
    {
        [Range(0f, 1f)]
        [ShowIf("@!Indeterminate")]
        public float Progress = 0f;

        public Color gainingProgressColor;
        public Color losingProgressColor;
        
        public bool Indeterminate = true;
        
        [Range(0.01f, 1f)]
        [ShowIf(nameof(Indeterminate))]
        public float IndeterminateWidth = 0.2f;

        private PathGenerator _path;
        
        //Litéralement copier le materialProperties mais le component est dans un autre assembly et j'ai peur de tout casser si je touche a ça
        private MaterialPropertyBlock _propertyBlock;
        private MeshRenderer _meshRenderer;
        
        private float previousProgress = 0f;
        private static readonly int ColorId = Shader.PropertyToID("_Color");

        private void Awake()
        {
            this.GetComponentChecked(out _path);
            _meshRenderer = GetComponent<MeshRenderer>();
            _propertyBlock = new MaterialPropertyBlock();
            _meshRenderer.SetPropertyBlock(_propertyBlock);
        }
        
        private void Update()
        {
            if (!Indeterminate)
            {
                _path.clipFrom = 0f;
                _path.clipTo = Progress;
                _propertyBlock.SetColor(ColorId, Progress > previousProgress ? gainingProgressColor : losingProgressColor);
                _meshRenderer.SetPropertyBlock(_propertyBlock);
                previousProgress = Progress;
                return;
            }

            float offset = Time.time * 0.25f;
            float width = IndeterminateWidth;
            float start = Mathf.Repeat(offset, 1f);
            float end = Mathf.Repeat(offset + width, 1f);

            if (end >= start)
            {
                _path.clipFrom = start;
                _path.clipTo = end;
                return;
            }

            float transitionProgress = (start - (1f - width)) / width;
            if (transitionProgress < 0.5f)
            {
                float newWidth = Mathf.Lerp(width, 0, transitionProgress * 2f);
                _path.clipTo = 1f;
                _path.clipFrom = 1f - newWidth;
            }
            else
            {
                float newWidth = Mathf.Lerp(0, width, (transitionProgress - 0.5f) * 2f);
                _path.clipFrom = 0f;
                _path.clipTo = newWidth;
            }

            _path.Rebuild();
        }
    }
}