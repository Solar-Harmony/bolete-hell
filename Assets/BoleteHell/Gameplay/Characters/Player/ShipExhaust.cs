using BoleteHell.Code.Input;
using UnityEngine;
using Zenject;

namespace BoleteHell.Gameplay.Characters.Player
{
    public class ShipExhaust : MonoBehaviour
    {
        [Inject] 
        private IInputDispatcher input;

        [SerializeField] 
        private GameObject shipExhaust;

        private void Update()
        {
            shipExhaust.SetActive(input.IsMoving);
        }
    }
}