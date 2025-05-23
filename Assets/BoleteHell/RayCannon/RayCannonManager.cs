using System.Collections.Generic;
using Input;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;

//TODO: Make this a general assetLoader that give the data to the right managers and objects
namespace Prisms
{
    public class RayCannonManager : MonoBehaviour
    {
        private const string GroupLabel = "Prisms";
        [SerializeField] private PlayerLaserInput player;
        
        [SerializeField] private List<GameObject> prisms = new();

        private void Start()
        {
            LoadPrisms();
            Physics2D.queriesHitTriggers = false;
        }

        private void LoadPrisms()
        {
            Addressables.LoadAssetsAsync<GameObject>(GroupLabel, obj =>
            {
                Debug.Log($"instantiating {obj.name}");
                prisms.Add(obj);

                if (!obj.TryGetComponent(out RayCannon prism)) return;
                
                prism.Init();
                if(prism.IsDefault)
                    player.AddPrism(prism);
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