using System;
using BoleteHell.Utils.Progress;
using UnityEngine;

public class CapturePointComponent : MonoBehaviour
{
    [Range(0, 360)]
    public int totalSeconds;
    
    [SerializeField]
    private BossTimer visualTimer;

    private float remainingTime;
    
    private Collider2D _collider2D;
    private bool timerIsRunning;


    public static event Action OnCaptured;
    
    private void Awake()
    {
        remainingTime = totalSeconds;
        _collider2D = GetComponent<Collider2D>();
        Debug.Assert(_collider2D && _collider2D.isTrigger, "Capture point is missing a trigger collider");
        visualTimer.Indeterminate = false;
        visualTimer.Progress = 0f;
    }

    private void Update()
    {
        if (timerIsRunning)
        {
            UpdateRemainingTime(-Time.deltaTime);
        }
        else
        {
            if (remainingTime < totalSeconds)
            {
                UpdateRemainingTime(Time.deltaTime);
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

    private void UpdateRemainingTime(float delta)
    {
        remainingTime += delta;
        visualTimer.Progress = (totalSeconds % remainingTime) / totalSeconds;
            
        if (remainingTime <= 0)
        {
            OnCaptured?.Invoke();
        }
    }
}
