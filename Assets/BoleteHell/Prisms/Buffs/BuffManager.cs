using System;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    private List<PrismBuff> prismBuffs = new();

    private List<RayBuff> rayBuffs = new();
    //Devrait loader tout les buffs et avoir des méthode pour get un buff random qui est valide pour les armes équipé (ne devrait aps avoir de drops pour des armes que le joueur n'a pas)
    private void Awake()
    {
        
    }

    
}
