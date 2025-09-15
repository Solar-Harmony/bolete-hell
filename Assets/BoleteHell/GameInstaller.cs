using BoleteHell.AI;
using BoleteHell.Arsenals;
using BoleteHell.Audio;
using BoleteHell.Code.UI;
using BoleteHell.Gameplay;
using UnityEngine;
using Utils.BoleteHell.Utils;
using Zenject;

namespace BoleteHell
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField]
        private GameObject transientLightPrefab;
        
        [SerializeField]
        private GameObject spriteFragmentPrefab;

        public override void InstallBindings()
        {
            Container.Install<UtilsInstaller>();
            Container.Install<AudioInstaller>();
            Container.Install<GameplayInstaller>(new object[] {transientLightPrefab, spriteFragmentPrefab});
            Container.Install<ArsenalInstaller>();
            Container.Install<AIInstaller>();
            Container.Install<UIInstaller>();
        }
    }
}
