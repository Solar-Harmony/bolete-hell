using UnityEngine;

namespace BoleteHell.Code.AI.Agents
{
    [RequireComponent(typeof(Arsenal.Arsenal))]
    public class Enemy : MonoBehaviour
    {
        private Arsenal.Arsenal _weapon;
        
        public void Start()
        {
            _weapon =  GetComponent<Arsenal.Arsenal>();
        }

        public void Shoot(Vector3 direction)
        {
            _weapon.Shoot( direction);
        }
        
        public float GetProjectileSpeed()
        {
            if (!_weapon) return 0.0f;
            return _weapon.GetSelectedWeapon().cannonData.projectileSpeed;
        }
    }
}