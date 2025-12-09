using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace BoleteHell.Utils.Advisor
{
    public class Tutorial : MonoBehaviour
    {
        [SerializeField]
        private TutorialPopup _popup;

        [Serializable]
        public class Speakers
        {
            public Speaker MainAdvisor;
            public Speaker TechAdvisor;
            public Speaker BaseAdvisor;
        }
        
        [Inject]
        private Speakers _speakers;
        
        private void Start()
        {
            StartCoroutine(RunTutorial());
        }

        private IEnumerator RunTutorial()
        {
            yield return _popup.ShowAsync(new(_speakers.MainAdvisor, "Sir, the situation is critical. The BOLETES are closing in on our base!"));
            yield return _popup.ShowAsync(new(_speakers.TechAdvisor, "An obvious result of your mismanagement. If we had-", 0.2f));
            yield return _popup.ShowAsync(new(_speakers.MainAdvisor, "Toi, ferme ta yeule. Commander, defend our base! Tin Can and I will find the source the attack.", 1.5f));
            yield return _popup.ShowAsync(new(_speakers.TechAdvisor, "WASD to move,\nLEFT CLICK to shoot.\n\nYou're welcome.", 3.0f));
        }
    }
}