using System.Collections;
using UnityEngine;

namespace BoleteHell.Utils.Advisor
{
    public class Tutorial : MonoBehaviour, ITutorialService
    {
        [SerializeField]
        private AdvisorPopup _popup; 

        [SerializeField]
        private Speaker _speakerA;

        [SerializeField]
        private Speaker _speakerB;
        
        [SerializeField]
        private Speaker _speakerC;
        
        private void Start()
        {
            StartCoroutine(RunTutorial());
        }

        private IEnumerator RunTutorial()
        {
            yield return _popup.ShowAsync(_speakerA, "Sir, the situation is critical. The BOLETES are closing in to our base!");
            yield return _popup.ShowAsync(_speakerB, "An obvious result of your mismanagement. If we had-", 0.2f);
            yield return _popup.ShowAsync(_speakerA, "Toi, ferme ta yeule. Commander, defend our base! Tin can and I will find the source the attack.", 1.5f);
            yield return _popup.ShowAsync(_speakerB, "WASD to move\nLeft click to <b>attack</b>\nRight click to <b>draw prism shield</b>\nQ/E to <b>cycle shields</b>\nYou're welcome.       ", 0.2f);
        }

        public void ShowTutorial(TutorialEvent tutorialEvent)
        {
            StartCoroutine(Speech());
            return;

            IEnumerator Speech()
            {
                yield return _popup.ShowAsync(_speakerA, "Wow, you killed a BOLETE! Impressive! They drop random bullshit like health and energy for your shield.");
                yield return _popup.ShowAsync(_speakerC, "Fuck you??");
            }
        }
    }
}