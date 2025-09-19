using UnityEngine;
using Zenject;
using TMPro;
using BoleteHell.Code.Gameplay.Damage;

public class DamageTextManager : MonoBehaviour
{
    [SerializeField]
    private GameObject damagetextPrefab;

    [SerializeField]
    private Canvas gameCanvas;

    [Inject]
    private HealthText.Pool _pool;

    private void OnEnable()
    {
        Health.OnDamaged += (CharacterTookDamage);
    }

    private void OnDisable()
    {
        Health.OnDamaged -= (CharacterTookDamage);
    }

    private void CharacterTookDamage(GameObject character, int damageAmount)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);
        _pool.Spawn(spawnPosition, damageAmount);
    }
}
