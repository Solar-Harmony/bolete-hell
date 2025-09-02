using UnityEngine;
using Zenject;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject damagetextPrefab;

    public Canvas gameCanvas; 

    public void CharacterTookDamage(GameObject character, int damageAmount)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);
        TMP_Text tmpText = Instantiate(damagetextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

        tmpText.text = damageAmount.ToString();

    }
}
