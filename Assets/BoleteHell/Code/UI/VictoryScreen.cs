using BoleteHell.Gameplay.GameState;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace BoleteHell.Code.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class VictoryScreen : MonoBehaviour
    {
        [Inject]
        private IGameOutcomeService _gameOutcomeService;
        
        private VisualElement _root;

        private void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            Hide();
            
            _gameOutcomeService.OnVictory += () => Show("You win!", "Congratulations!");
            _gameOutcomeService.OnDefeat  += reason => Show("You lose!", reason);
        }
        
        private void Show(string text, string subtext) 
        {
            _root.style.display = DisplayStyle.Flex;
            _root.Q<Label>("Text").text = GetDefeatText();
            _root.Q<Label>("SubText").text = subtext;
        }
        
        private void Hide()
        {
            _root.style.display = DisplayStyle.None;
        }
        
        private readonly string[] _defeatTexts = 
        {
            "L RIZZ",
            "RIP BOZO GET BETTER",
            "GIT GUD",
            "IMAGINE BEING SO BAD",
            "LOL GG WP",
            "BRUH WTF WAS THAT L",
            "BRO IS COOKED",
            "THAT WAS TRASH FR NO CAP",
            "TOO EZ",
        };

        private string GetDefeatText()
        {
            int index = Random.Range(0, _defeatTexts.Length);
            return _defeatTexts[index];
        }
    }
}