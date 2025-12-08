using BoleteHell.Gameplay.SpawnManager;
using System;
using System.Collections.Generic;
using BoleteHell.Code.Core;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Zenject;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SpawnEnemiesAtLocations", story: "Spawn [enemies] at [locations]", category: "Bolete Hell", id: "894040d67a92884adf451f741b9f363c")]
public partial class SpawnEnemiesAtLocationsAction : Action
{
    [SerializeReference] public BlackboardVariable<SpawnList> Enemies;
    [SerializeReference] public BlackboardVariable<List<GameObject>> Locations;

    private ISpawnService _spawnService;

    protected override Status OnStart()
    {
        ServiceLocator.Get(out _spawnService);
        
        foreach (GameObject location in Locations.Value)
        {
            _spawnService.SpawnAt(new SpawnParams(Enemies.Value.allowedEnemies[0], location.transform.position, 0));
        }

        Debug.Log("Spawned unit");
        return Status.Success;
    }
}

