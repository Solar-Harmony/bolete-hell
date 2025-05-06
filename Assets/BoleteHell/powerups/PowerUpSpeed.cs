using UnityEngine;

public class PowerUpSpeed : Powerups
{
    [SerializeField] private float speedMultiplier = 0f;
    [SerializeField] private float duration = 0f;
    private float originalSpeed;
    protected override float Duration => duration;
    protected override void ApplyEffect(GameObject player)
    {
        var move = player.GetComponent<PlayerMovement>();
        if (move == null) return;
        {
            originalSpeed = move.MoveSpeed;
            move.MoveSpeed = originalSpeed * speedMultiplier;
        }
    }
    protected override void RemoveEffect(GameObject player)
    {
        var move = player.GetComponent<PlayerMovement>();
        if (move == null) return;
        {
            move.MoveSpeed = originalSpeed;
        }
    }
}

