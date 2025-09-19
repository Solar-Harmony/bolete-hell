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
        TMP_Text tmpText = Instantiate(damagetextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

        tmpText.text = damageAmount.ToString();
    }
}
