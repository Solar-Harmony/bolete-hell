using System.Collections.Generic;
using Input;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

//TODO: Make this a general assetLoader that give the data to the right managers and objects
namespace BoleteHell.RayCannon
{
    public class CannonLoader : MonoBehaviour
    {
        private const string GroupLabel = "Prisms";
        [SerializeField] private PlayerLaserInput player;
        [SerializeField] private List<GameObject> rayCannons = new();
 
        private void Start()
        {
            LoadRayCannons();
        }

        private void LoadRayCannons()
        {
            Addressables.LoadAssetsAsync<GameObject>(GroupLabel, obj =>
            {
                Debug.Log($"instantiating {obj.name}");
                rayCannons.Add(obj);
            }).Completed += OnLoadComplete;
        }

        private void OnLoadComplete(AsyncOperationHandle<IList<GameObject>> handle)
        {
            //TODO: Send prisms to the DropManager
            if (handle.Status == AsyncOperationStatus.Succeeded)
                Debug.Log("All prisms instantiated successfully.");
            else
                Debug.LogError("Failed to load Addressables.");
        }
    }
}