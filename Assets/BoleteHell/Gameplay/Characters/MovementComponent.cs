using UnityEngine;

namespace BoleteHell.Gameplay.Characters
{
    [DisallowMultipleComponent]
    public class MovementComponent : MonoBehaviour
    {
        [field: SerializeField]
        public float MovementSpeed { get; set; } = 5f;
    }
}