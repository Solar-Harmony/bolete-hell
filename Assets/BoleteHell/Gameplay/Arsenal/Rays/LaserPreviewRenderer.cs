using UnityEngine;
using Zenject;

public class LaserPreviewRenderer : MonoBehaviour, IPoolable<Vector3, Vector3, Color, float>

{
    private LineRenderer lineRenderer;

    private float chargeTime = 0f;

    private float minWidth = 0f;
    private float maxWidth = 0.1f;
    
    [Inject]
    private Pool pool;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void OnDespawned()
    {
        // lineRenderer.positionCount = 0;
        // lineRenderer.startWidth = minWidth;
        // lineRenderer.endWidth = minWidth;
    }

    public void OnSpawned(Vector3 startPos, Vector3 endPos, Color lineColor, float totalChargeTime)
    {
        chargeTime = totalChargeTime;
        lineRenderer.positionCount = 2;
        
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
        
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        
        lineRenderer.startWidth = minWidth;
        lineRenderer.endWidth = minWidth;
    }

    //updater le endPos au cas ou il bougerais
    //Faire grossir le linerenderer selon le lerp du temps courant entre 0 et chargeTime
    public void UpdatePreview(Vector3 startPos, Vector3 endPos, float currentTime)
    {
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        float chargeRatio = Mathf.Clamp01(currentTime / chargeTime);
        float width = Mathf.Lerp(minWidth, maxWidth, chargeRatio);
        
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
    }

    public void Despawn()
    {
        pool.Despawn(this);
    }

    public class Pool : MonoPoolableMemoryPool<Vector3, Vector3, Color, float, LaserPreviewRenderer>
    {

    }
}
