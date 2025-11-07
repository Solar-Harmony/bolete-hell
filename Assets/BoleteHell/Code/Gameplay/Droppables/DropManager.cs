using System;
using System.Collections.Generic;
using BoleteHell.Code.Utils;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace BoleteHell.Code.Gameplay.Droppables
{
    public enum DropCategory
    {
        ShieldUpgrade,
        WeaponUpgrade
    }

    public class DropManager : IDropManager
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

        [Serializable]
        public class Config
        {
            public List<GameObject> Droplets;
        }
        
        public DropManager(Config config)
        {
            _config = config;
        }
        
        private readonly Config _config;
        
        [Inject]
        private IObjectInstantiator _instantiator;
        
        public void Drop(GameObject dropSource, DropSettings ctx)
        {
            throw new NotImplementedException();
        }

        public void DropDroplets(GameObject dropSource, DropRangeContext ctx)
        {
            //Pourrais être plus contextuel et dropper selon ce que le joueur manque le plus
            //Devrait aussi positionner les drop aux alentour du dropSource pas directement toute a la même position
            if (Random.value * 10 > ctx.dropChance) return;
            
            for (int i = 0; i < ctx.GetValueInRange(); i++)
            {
                int randomDropIndex = Random.Range(0, _config.Droplets.Count);
                Vector3 position = dropSource.transform.position;
                position.z = -1;
                _instantiator.InstantiateWithInjection(_config.Droplets[randomDropIndex], position, dropSource.transform.rotation, null);
            }
        }

        public void DropGold(GameObject dropSource, DropRangeContext ctx)
        {
            throw new NotImplementedException();
        }
    }
}