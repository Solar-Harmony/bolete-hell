using System.Collections.Generic;
using UnityEngine;

public class LineRendererPool : MonoBehaviour
{
    [SerializeField] public GameObject lineRendererObj;

    [SerializeField] private int numLinerenderer = 25;
    public static LineRendererPool Instance { get; private set; }
    [Header("Line Renderer Pool Settings")]
    private List<InstantRayRenderer> pool = new(); 

    private List<bool> activeRenderers;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        
        for (int i = 0; i <= numLinerenderer; i++)
        {
            GameObject obj = Instantiate(lineRendererObj, gameObject.transform);
            obj.SetActive(false);
            InstantRayRenderer rayRenderer = obj.GetComponent<InstantRayRenderer>();
            pool.Add(rayRenderer);
            rayRenderer.Init(i);
        }

        activeRenderers = new List<bool>(new bool[pool.Count]);
    }
    
    public InstantRayRenderer GetRandomAvailable()
    {
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < Instance.activeRenderers.Count; i++)
        {
            if (!Instance.activeRenderers[i])
            {
                availableIndices.Add(i);
            }
        }

        if (availableIndices.Count == 0)
        {
            Debug.LogWarning("No linerenderer available");
            return null;
        }

        // Choose one index at random among the available ones
        int randomIndex = availableIndices[Random.Range(0, availableIndices.Count)];
        Debug.Log($"Activated {Instance.pool[randomIndex].name} {randomIndex}");
        Instance.activeRenderers[randomIndex] = true; 
        Instance.pool[randomIndex].gameObject.SetActive(true);
        return Instance.pool[randomIndex];
    }
    
    public static void Release(int index)
    {
        if (index >= 0)
        {
            Debug.Log("Setting false");
            Instance.pool[index].gameObject.SetActive(false);
            Instance.activeRenderers[index] = false;
        }
    }
}
