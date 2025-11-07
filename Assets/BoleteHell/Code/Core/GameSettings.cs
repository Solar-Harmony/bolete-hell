using BoleteHell.Code.Gameplay.Droppables;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Core
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "BoleteHell/Game Settings")]
    public class GameSettings : ScriptableObjectInstaller
    {
        public DropManager.Config DropManagerConfig;
        
        public override void InstallBindings()
        {
            Container.BindInstance(DropManagerConfig);
        }
    }
}