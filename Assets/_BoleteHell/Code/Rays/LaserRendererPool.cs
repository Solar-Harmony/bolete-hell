using System.Collections.Generic;
using BoleteHell.Rays;
using UnityEngine;

namespace Lasers
{
    public class LaserRendererPool : MonoBehaviour
    {
        [SerializeField] public GameObject lineRendererPrefab; 

        [SerializeField] private int initialPoolSize = 25;

        private readonly Queue<LaserRenderer> _pool = new();

        public static LaserRendererPool Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            InitializePool(initialPoolSize);
        }

        private void InitializePool(int size)
        {
            for (int i = 0; i < size; i++)
            {
                AddObjectToPool();
            }
        }
        
        private void AddObjectToPool()
        {
            if (!lineRendererPrefab.GetComponent<LaserRenderer>())
            {
                Debug.LogError("lineRendererPrefab empty");
                return;
            }
            GameObject obj = Instantiate(lineRendererPrefab, transform);
            obj.SetActive(false);
            LaserRenderer rayRenderer = obj.GetComponent<LaserRenderer>();
            _pool.Enqueue(rayRenderer);
        }

        public LaserRenderer Get()
        {
            if (_pool.Count == 0)
            {
                Debug.LogWarning("Pool empty adding more");
                AddObjectToPool();
            }

            LaserRenderer laserRenderer = _pool.Dequeue();
            laserRenderer.gameObject.SetActive(true);
            return laserRenderer;
        }

        public void Release(LaserRenderer laserRenderer)
        {
            if (!laserRenderer)
            {
                Debug.LogError("Attempted to release null object");
                return;
            }

            laserRenderer.gameObject.SetActive(false);
            _pool.Enqueue(laserRenderer);
        }
    }
}
