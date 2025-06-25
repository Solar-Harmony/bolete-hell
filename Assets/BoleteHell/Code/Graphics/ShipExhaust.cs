using BoleteHell.Code.Input;
using UnityEngine;

namespace BoleteHell.Code.Graphics
{
    public class ShipExhaust : MonoBehaviour
    {
        [SerializeField] private InputController input;

        [SerializeField] private GameObject shipExhaust;

        private void Update()
        {
            shipExhaust.SetActive(input.IsMoving);
        }
    }
}