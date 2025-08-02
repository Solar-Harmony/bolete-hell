using BoleteHell.Code.Input;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Graphics
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