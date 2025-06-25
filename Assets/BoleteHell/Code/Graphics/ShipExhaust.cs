using Input;
using UnityEngine;

namespace Graphics
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