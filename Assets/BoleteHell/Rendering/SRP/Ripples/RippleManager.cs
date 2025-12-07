using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoleteHell.Rendering.Ripples
{
    public class RippleManager : MonoBehaviour
    {
        private const int MaxRipplesInBuffer = 128;
        private const int MaxRipplesToShader = 32;

        private static readonly int _rippleDataId = Shader.PropertyToID("_RippleData");
        private static readonly int _rippleCountId = Shader.PropertyToID("_RippleCount");
        private static readonly int _rippleLifetimeId = Shader.PropertyToID("_RippleLifetime");
        private static readonly int _rippleTextureBoundsId = Shader.PropertyToID("_RippleTextureBounds");

        [SerializeField]
        private float _rippleLifetime = 1.5f;

        [SerializeField]
        [Tooltip("Extra padding beyond camera bounds for ripple texture coverage.")]
        private float _boundsMargin = 5f;

        private readonly RippleData[] _activeRipples = new RippleData[MaxRipplesInBuffer];
        private int _activeCount = 0;
        private readonly Vector4[] _shaderRippleData = new Vector4[MaxRipplesToShader];
        
        private readonly RippleSortEntry[] _sortBuffer = new RippleSortEntry[MaxRipplesInBuffer];
        private int _sortCount = 0;

        private Camera _mainCamera;

        public float RippleLifetime => _rippleLifetime;

        private struct RippleSortEntry : IComparable<RippleSortEntry>
        {
            public int Index;
            public float Score;

            public int CompareTo(RippleSortEntry other) => Score.CompareTo(other.Score);
        }

        private sealed class RippleSortComparer : IComparer<RippleSortEntry>
        {
            public static readonly RippleSortComparer Instance = new();
            public int Compare(RippleSortEntry x, RippleSortEntry y) => x.Score.CompareTo(y.Score);
        }

        private void Awake()
        {
            _mainCamera = Camera.main;
            ClearShaderData();
        }

        public void EmitRipple(Vector2 position, float intensity)
        {
            if (_activeCount >= MaxRipplesInBuffer)
            {
                RemoveOldestRipple();
            }

            _activeRipples[_activeCount++] = new RippleData(position, Time.time, intensity);
        }

        private void Update()
        {
            RemoveExpiredRipples();
            UpdateShaderData();
        }

        private void RemoveExpiredRipples()
        {
            float currentTime = Time.time;
            int writeIndex = 0;
            
            for (int i = 0; i < _activeCount; i++)
            {
                if (currentTime - _activeRipples[i].SpawnTime <= _rippleLifetime)
                {
                    if (writeIndex != i)
                        _activeRipples[writeIndex] = _activeRipples[i];
                    writeIndex++;
                }
            }
            
            _activeCount = writeIndex;
        }

        private void RemoveOldestRipple()
        {
            if (_activeCount == 0)
                return;

            int oldestIndex = 0;
            float oldestTime = _activeRipples[0].SpawnTime;

            for (int i = 1; i < _activeCount; i++)
            {
                if (_activeRipples[i].SpawnTime < oldestTime)
                {
                    oldestTime = _activeRipples[i].SpawnTime;
                    oldestIndex = i;
                }
            }

            _activeCount--;
            if (oldestIndex < _activeCount)
                _activeRipples[oldestIndex] = _activeRipples[_activeCount];
        }

        private void UpdateShaderData()
        {
            if (!_mainCamera)
            {
                _mainCamera = Camera.main;
                if (!_mainCamera) return;
            }

            Vector2 cameraPos = _mainCamera.transform.position;
            float currentTime = Time.time;

            float halfHeight = _mainCamera.orthographicSize + _boundsMargin;
            float halfWidth = halfHeight * _mainCamera.aspect + _boundsMargin;
            float extent = Mathf.Max(halfWidth, halfHeight);

            Vector4 worldBounds = new Vector4(
                cameraPos.x - extent,
                cameraPos.y - extent,
                cameraPos.x + extent,
                cameraPos.y + extent
            );
            Shader.SetGlobalVector(_rippleTextureBoundsId, worldBounds);

            float minX = worldBounds.x;
            float minY = worldBounds.y;
            float maxX = worldBounds.z;
            float maxY = worldBounds.w;

            _sortCount = 0;
            for (int i = 0; i < _activeCount; i++)
            {
                ref var ripple = ref _activeRipples[i];
                
                if (ripple.Position.x >= minX && ripple.Position.x <= maxX &&
                    ripple.Position.y >= minY && ripple.Position.y <= maxY)
                {
                    float dx = ripple.Position.x - cameraPos.x;
                    float dy = ripple.Position.y - cameraPos.y;
                    float distSq = dx * dx + dy * dy;
                    float age = currentTime - ripple.SpawnTime;
                    
                    _sortBuffer[_sortCount++] = new RippleSortEntry
                    {
                        Index = i,
                        Score = age * Mathf.Sqrt(distSq)
                    };
                }
            }

            if (_sortCount > 1)
                Array.Sort(_sortBuffer, 0, _sortCount, RippleSortComparer.Instance);

            int count = Mathf.Min(_sortCount, MaxRipplesToShader);

            for (int i = 0; i < count; i++)
            {
                _shaderRippleData[i] = _activeRipples[_sortBuffer[i].Index].ToVector4();
            }
            
            for (int i = count; i < MaxRipplesToShader; i++)
            {
                _shaderRippleData[i] = new Vector4(0, 0, -100, 0);
            }

            Shader.SetGlobalVectorArray(_rippleDataId, _shaderRippleData);
            Shader.SetGlobalInt(_rippleCountId, count);
            Shader.SetGlobalFloat(_rippleLifetimeId, _rippleLifetime);
        }

        private void ClearShaderData()
        {
            for (int i = 0; i < MaxRipplesToShader; i++)
            {
                _shaderRippleData[i] = new Vector4(0, 0, -100, 0);
            }

            Shader.SetGlobalVectorArray(_rippleDataId, _shaderRippleData);
            Shader.SetGlobalInt(_rippleCountId, 0);
        }

        private void OnDestroy()
        {
            ClearShaderData();
        }
    }
}
