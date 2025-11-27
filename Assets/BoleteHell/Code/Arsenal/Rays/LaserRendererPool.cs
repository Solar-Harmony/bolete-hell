using System.Collections.Generic;
using BoleteHell.Code.Gameplay.Characters;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.Rays
{
    public class LaserRendererPool : MonoBehaviour
    {
        [SerializeField] public GameObject lineRendererPrefab; 

        [SerializeField] private int initialPoolSize = 25;
         private int currentPoolSize;

        private readonly Queue<LaserInstance> _pool = new();
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
            currentPoolSize = size;
            for (int i = 0; i < size; i++)
            {
                AddObjectToPool();
            }
        }
        
        private void AddObjectToPool()
        {
            if (!lineRendererPrefab.GetComponent<LaserInstance>())
            {
                Debug.LogError("lineRendererPrefab empty");
                return;
            }

            Vector3 spawnPos = transform.position + new Vector3(0.0f, 0.0f, -1.0f); // keep in front of backgrounds
            GameObject obj = Instantiate(lineRendererPrefab, spawnPos, Quaternion.identity);
            
            obj.name = $"LaserRenderer {_pool.Count}";
            obj.SetActive(false);
            LaserInstance rayRenderer = obj.GetComponent<LaserInstance>();
            _pool.Enqueue(rayRenderer);
        }

        public LaserInstance Get(GameObject owner, AffectedSide side)
        {
            if (_pool.Count == 0)
            {
                Debug.LogWarning("Pool empty adding more");
                for (int i = 0; i < currentPoolSize; i++)
                {
                    AddObjectToPool();
                }
            
                currentPoolSize *= 2;
            }

            LaserInstance laserRenderer = _pool.Dequeue();
            // l'instigateur doit être setup avant que le laser est dessiné car le laserbeam fait les dégats avant d'être déssiné
            laserRenderer.SetFactionInfo(owner, side);
            laserRenderer.gameObject.SetActive(true);
            return laserRenderer;
        }

        public void Release(LaserInstance laserRenderer)
        {
            if (!laserRenderer)
            {
                Debug.LogError("Attempted to release null object");
                return;
            }
            
            //Empeche un laserRenderer de se faire enqueue plusieur fois si il se fait re-release avant d'avoir été dequeued first
            if (_pool.Contains(laserRenderer)) return;
            laserRenderer.gameObject.SetActive(false);
            _pool.Enqueue(laserRenderer);
        }
    }
}
