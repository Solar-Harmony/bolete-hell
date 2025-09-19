using UnityEngine;
using Zenject;
using TMPro;
using BoleteHell.Code.Gameplay.Damage;

public class DamageTextManager : MonoBehaviour
{
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
        Vector3 worldPosition = character.transform.position;
        Vector2 canvasPosition = WorldToCanvasPosition(worldPosition);
        
        var healthText = _pool.Spawn(canvasPosition, damageAmount);
      
        healthText.transform.SetParent(gameCanvas.transform, false);
    }

    private Vector2 WorldToCanvasPosition(Vector3 worldPosition)
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(worldPosition);
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            gameCanvas.transform as RectTransform, 
            screenPoint, 
            gameCanvas.worldCamera, 
            out Vector2 localPoint);
            
        return localPoint;
    }
}
