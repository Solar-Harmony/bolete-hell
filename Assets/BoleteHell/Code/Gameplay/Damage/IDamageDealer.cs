using System.Collections.Generic;
using BoleteHell.Code.Gameplay.Character;

namespace BoleteHell.Code.Gameplay.Damage
{
  public interface IDamageDealer
  {
    public float GeneralDamageMultiplier { get; set; }

    public Dictionary<Faction, float> factionDamageMultiplier { get; }

    public float GetDamageMultiplier(Faction hitTargetFaction)
    {
      float factionMultiplier = factionDamageMultiplier.GetValueOrDefault(hitTargetFaction, 1f);
      return GeneralDamageMultiplier * factionMultiplier;
    }

    
    //Si je multiplie les multiplier il deviennent explonentiel alors que ce que je voudrais est que deux buff de 1.5 donne un buff de 2.0 pas 2.25
    //Donc quand on créé un multiplier dans le dictionnaire on l'initie a 1 puis on retire 1 a tout les ajout(Puisque les ajout son en relation avec 1)
    //ajouter deux fois 50% a une faction donne un bonus de 100% plutot que 125%
    public void AddDamageMultiplier(Faction? hitTargetFaction, float multiplier)
    {
      float actuallyAddedMultiplier = multiplier - 1;
      
      if (hitTargetFaction == null)
      {
        GeneralDamageMultiplier += actuallyAddedMultiplier;
      }
      else
      {
        Faction faction = hitTargetFaction.Value;
        
        factionDamageMultiplier.TryAdd(faction, 1f);
        
        factionDamageMultiplier[faction] += actuallyAddedMultiplier;
      }
    }

    public void RemoveDamageMultiplier(Faction? hitTargetFaction, float multiplier)
    {
      float actuallyAddedMultiplier = multiplier - 1;

      if (hitTargetFaction == null)
      {
        GeneralDamageMultiplier -= actuallyAddedMultiplier;
      }
      else
      {
        Faction faction = hitTargetFaction.Value;
        factionDamageMultiplier[faction] -= actuallyAddedMultiplier;
      }
    }
  }
}
