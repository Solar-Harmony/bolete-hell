using BoleteHell.Gameplay.SpawnManager;
using System;
using System.Collections.Generic;
using System.Linq;
using BoleteHell.AI.Services;
using BoleteHell.Code.Core;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Random = UnityEngine.Random;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SpawnEnemiesAtLocations", story: "Spawn [enemies] at [locations]", category: "Bolete Hell", id: "894040d67a92884adf451f741b9f363c")]
public partial class SpawnEnemiesAtLocationsAction : Action
{
    [SerializeReference] public BlackboardVariable<CostTable> Enemies;
    [SerializeReference] public BlackboardVariable<List<GameObject>> Locations;

    private ISpawnService _spawnService;

    protected override Status OnStart()
    {
        ServiceLocator.Get(out _spawnService);

        foreach (GameObject enemy in Enemies.Value.Entries.Select(e => e.Prefab))
        {
            int randomLocationIndex = Random.Range(0, Locations.Value.Count);
            _spawnService.SpawnAt(new SpawnParams(enemy, Locations.Value[randomLocationIndex].transform.position, 0));
        }

        Debug.Log("Spawned enemies");
        return Status.Success;
    }
}

