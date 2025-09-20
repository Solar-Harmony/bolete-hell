using Pathfinding;
using TMPro;
using UnityEngine;
using Zenject;

public class HealthText : MonoBehaviour, IPoolable<Vector3, int>
{
    public Vector3 moveSpeed = new Vector3(0, 75, 0);
    public float timeToFade = 1f;

    RectTransform textTransform;
    TextMeshProUGUI textMeshPro;
   
    private float timeElapsed = 0f;
    private Color startColor;

    [Inject] 
    private Pool _pool;

    private void Awake()
    {
        textTransform = GetComponent<RectTransform>();
        textMeshPro = GetComponent<TextMeshProUGUI>();
        startColor = textMeshPro.color;
    }

    public void OnSpawned(Vector3 position, int damage)
    {
        transform.SetPositionAndRotation(position, Quaternion.identity);
        textMeshPro.color = startColor;
        textMeshPro.text = damage.ToString();
        timeElapsed = 0f; // Reset time when spawned
    }

    public void OnDespawned()
    {
        timeElapsed = 0f; // Reset for next use
    }

    private void Update()
    {
        textTransform.position += moveSpeed * Time.deltaTime;
        timeElapsed += Time.deltaTime;

        if (timeElapsed < timeToFade)
        {
            float FadeAlpha = startColor.a * (1 - (timeElapsed/ timeToFade));
            textMeshPro.color = new Color(startColor.r, startColor.g, startColor.b, FadeAlpha);
        }
        else
        {
            _pool.Despawn(this);
        }
    }

    public class Pool : MonoPoolableMemoryPool<Vector3, int, HealthText>
    {

    }
}
