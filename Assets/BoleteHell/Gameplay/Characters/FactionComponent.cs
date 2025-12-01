using UnityEngine;

namespace BoleteHell.Gameplay.Characters
{
    public enum FactionType
    {
        Player,
        Enemy
    }
    
    public enum AffectedSide
    {
        All,
        Allies,
        Enemies,
    }
    
    [DisallowMultipleComponent]
    public class FactionComponent : MonoBehaviour
    {
        [field: SerializeField]
        public FactionType Type { get; private set; }
        
        /// <summary>
        /// This: objet qui subit un éffet
        /// Retourne si this devrait être affecté par un effet
        /// </summary>
        /// <param name="affectedSide">Coté affecté par l'éffet</param>
        /// <param name="other">Instigateur de l'éffet</param>
        /// <returns></returns>
        public bool IsAffected(AffectedSide affectedSide, FactionComponent other)
        {
            if (affectedSide == AffectedSide.All)
                return true;

            return affectedSide == GetSide(other);
        }

        private AffectedSide GetSide(FactionComponent other)
        {
            return Type == other.Type ? AffectedSide.Allies : AffectedSide.Enemies;
        }
    }
}