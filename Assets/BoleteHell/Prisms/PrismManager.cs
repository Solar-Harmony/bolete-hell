using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

//TODO: Make this a general assetLoader that give the data to the right managers and objects
public class PrismManager : MonoBehaviour
{
    private string groupLabel = "Prisms";
    private List<GameObject> prisms = new ();
    
    [SerializeField] private PlayerPrisms player;
    void Start()
    {
        LoadPrisms();
    }

    void LoadPrisms()
    {
        Addressables.LoadAssetsAsync<GameObject>(groupLabel, obj =>
        {
            Debug.Log($"instantiating {obj.name}");
            prisms.Add(obj);
            
            if (obj.TryGetComponent(out Prism prism))
            {
                prism.Init();
                player.AddPrism(prism);
            }
            
        }).Completed += OnLoadComplete;
    }

    void OnLoadComplete(AsyncOperationHandle<IList<GameObject>> handle)
    {
        //TODO: Send prisms to the DropManager
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("All prisms instantiated successfully.");
        }
        else
        {
            Debug.LogError("Failed to load Addressables.");
        }
    }
}
