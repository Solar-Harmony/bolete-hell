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
    //Simplifier pour l'instant vu que je ne sais pas si on va actually ajouter des faction autre que juste les ennemis(Shrooms) et le player
    //Mais on pourrais facilement faire un FactionManager qui gère les relation entre les faction et bouger le IsAffected et le GetSide la dedans en temps et lieu 
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
