using System.Collections.Generic;
using UnityEngine;

namespace Lasers
{
    //TODO: Créé un linerenderer pour les projectile laser
    //si je tire une projectile laser on get un ProjectileRenderer random et on lui donne une vitesse et une direction
    //Le projectileRenderer object devrais avoir un collider et faire la logique du mur si il touche un mur (ce check la devrait être dans le mur)
    //et faire le onHit si il touche un ennemis
    //Ajouter un rigid body 2d au projectile object
    public class LineRendererPool : MonoBehaviour
    {
        [SerializeField] public GameObject lineRendererObj;
        
        [SerializeField] private int numLineRenderers = 25;

        private readonly List<LaserRenderer> _pool = new();
        private List<bool> _activeRenderers;

        public static LineRendererPool Instance { get; private set; }

        private void Awake()
        {
            if (Instance && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;

            for (var i = 0; i <= numLineRenderers; i++)
            {
                var obj = Instantiate(lineRendererObj, gameObject.transform);
                obj.SetActive(false);
                var rayRenderer = obj.GetComponent<LaserRenderer>();
                _pool.Add(rayRenderer);
                rayRenderer.Init(i);
            }

            _activeRenderers = new List<bool>(new bool[_pool.Count]);
        }

        public static LaserRenderer GetRandomAvailable()
        {
            var availableIndices = new List<int>();
            for (var i = 0; i < Instance._activeRenderers.Count; i++)
                if (!Instance._activeRenderers[i])
                    availableIndices.Add(i);

            if (availableIndices.Count == 0)
            {
                Debug.LogWarning("No linerenderer available");
                return null;
            }

            // Choose one index at random among the available ones
            var randomIndex = availableIndices[Random.Range(0, availableIndices.Count)];
            Debug.Log($"Activated {Instance._pool[randomIndex].name} {randomIndex}");
            Instance._activeRenderers[randomIndex] = true;
            Instance._pool[randomIndex].gameObject.SetActive(true);
            return Instance._pool[randomIndex];
        }

        public static void Release(int index)
        {
            if (index < 0)
                return;

            Debug.Log("Setting false");
            Instance._pool[index].gameObject.SetActive(false);
            Instance._activeRenderers[index] = false;
        }
    }
}