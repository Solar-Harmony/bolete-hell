using BoleteHell.Code.Input;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace BoleteHell.Gameplay.InputControllers
{
    public class DebugInput : MonoBehaviour
    {
        [Inject] 
        private IInputDispatcher _input;

        private void OnEnable()
        {
            _input.OnReloadGame += OnReloadGame;
        }

        private void OnDisable()
        {
            _input.OnReloadGame -= OnReloadGame;
        }

        private static void OnReloadGame()
        {
            var activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.name);
        }
    }
}
