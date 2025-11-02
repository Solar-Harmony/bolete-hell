using System.Collections.Generic;
using BoleteHell.Code.Utils;
using UnityEngine;
using Zenject;

public enum DropCategory
{
    ShieldUpgrade,
    WeaponUpgrade
}

public class DropManager : MonoBehaviour, IDropManager
{
    //Peut-être faire un dictionnaire <DropCategory, droplist>
    //Permet d'associer une valeur d'enum a une drop list
    //Chaque drop list contient les drops de base de chaque catégories 
        //Il faudrait une manière de séparer les drop par rareté
            //Peut-être une liste par rareté ou des tags sur les drops 
    //Quand un ennemi meurt il call drop avec son contexte
        //permet de décider quel catégories il peut dropper (Avec le DropCategory enum)  
        //Permet d'ajouter des objets aux catégories
            //Donc un ennemis pourrais avoir des chances de dropper un objet spécifique que seul ce type d'ennemis peut dropper
    
    
    [SerializeField]
    private List<GameObject> droplets;

    [Inject]
    private IObjectInstantiator _instantiator;


    public void Drop(GameObject dropSource, DropContext ctx)
    {
        //Non implémenter pour l'instant pourras être fait quand on va vraiment travailler sur le system d'upgrades 
    }

    public void DropDroplets(GameObject dropSource, DropRangeContext ctx)
    {
        //Pourrais être plus contextuel et dropper selon ce que le joueur manque le plus
        //Devrait aussi positionner les drop aux alentour du dropSource pas directement toute a la même position
        if (Random.value * 10 > ctx.dropChance) return;
        
        
        for (int i = 0; i < ctx.GetValueInRange(); i++)
        {
            int randomDropIndex = Random.Range(0, droplets.Count);
            _instantiator.InstantiateWithInjection(droplets[randomDropIndex], dropSource.transform.position, dropSource.transform.rotation, null);
        }
    }

    public void DropGold(GameObject dropSource, DropRangeContext ctx)
    {
        //Non implémenter pour l'instant pourras être fait quand on va vraiment travailler sur le system de shop 

    }
}
