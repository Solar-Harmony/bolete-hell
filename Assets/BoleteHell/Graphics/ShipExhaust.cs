using BoleteHell.Input;
using BoleteHell.Utils;
using Input;
using UnityEngine;

namespace BoleteHell.Graphics
{
    public class ShipExhaust : MonoBehaviour
    {
        [SerializeField]
        private InputController input;  
        
        [SerializeField]
        private GameObject shipExhaust;
        
        private void Update()
        {
            shipExhaust.SetActive(input.IsMoving);
        }
    }
}