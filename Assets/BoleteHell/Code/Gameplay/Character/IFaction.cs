namespace BoleteHell.Code.Gameplay.Character
{
    public enum Faction
    {
        Player,
        Enemy
    }
    
    public enum AffectedSide
    {
        Allies,
        Enemies,
        All
    }
    
    public interface IFaction
    {
        public Faction faction { get; set; }
        
        /// <summary>
        /// This: objet qui subit un éffet
        /// Retourne si this devrait être affecté par un effet
        /// </summary>
        /// <param name="affectedSide">Coté affecté par l'éffet</param>
        /// <param name="other">Instigateur de l'éffet</param>
        /// <returns></returns>
        public bool IsAffected(AffectedSide affectedSide, IFaction other)
        {
            if (affectedSide == AffectedSide.All)
                return true;

            return affectedSide == GetSide(other);
        }

        private AffectedSide GetSide(IFaction other)
        {
            return faction == other.faction ? AffectedSide.Allies : AffectedSide.Enemies;
        }
    }
}
