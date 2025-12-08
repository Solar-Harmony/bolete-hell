using System;
using UnityEngine;

public class CapturePointComponent : MonoBehaviour
{
    [Range(0, 59)]
    public int minutes;

    [Range(0, 59)]
    public int seconds;

    private float totalSeconds => minutes * 60f + seconds;

    private float remainingTime;
    
    private Collider2D _collider2D;
    private bool timerIsRunning;

    public static event Action OnCaptured;
    private void Awake()
    {
        remainingTime = totalSeconds;
        _collider2D = GetComponent<Collider2D>();
        Debug.Assert(_collider2D && _collider2D.isTrigger, "Capture point is missing a trigger collider");
    }

    private void Update()
    {
        if (timerIsRunning)
        {
            remainingTime -= Time.deltaTime;
            
            if (remainingTime <= 0)
            {
                OnCaptured?.Invoke();
            }
        }
        else
        {
            if (remainingTime <= totalSeconds)
            {
                remainingTime += Time.deltaTime*2;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            timerIsRunning = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            timerIsRunning = false;
        }
    }
}
