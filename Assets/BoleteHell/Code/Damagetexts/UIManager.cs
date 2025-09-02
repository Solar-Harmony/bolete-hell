using UnityEngine;
using Zenject;
using TMPro;
using BoleteHell.Code.Gameplay.Damage;

public class UIManager : MonoBehaviour
{
    public GameObject damagetextPrefab;

    public Canvas gameCanvas;

    public void Awake()
    {
        gameCanvas = FindFirstObjectByType<Canvas>();
    }

    private void OnEnable()
    {
        Health.onDamaged += (CharacterTookDamage);
    }

    private void OnDisable()
    {
        Health.onDamaged -= (CharacterTookDamage);
    }

    public void CharacterTookDamage(GameObject character, int damageAmount)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);
        TMP_Text tmpText = Instantiate(damagetextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

        tmpText.text = damageAmount.ToString();

    }
}
