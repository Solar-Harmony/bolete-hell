using UnityEngine;
using System.Collections;

public abstract class Powerups : MonoBehaviour
{
    [Header("Powerups Base Class")]
    public AudioClip pickupSound;
    public GameObject powerEffect;

    protected abstract float Duration { get; }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayPickupEffects();
        ApplyEffect(other.gameObject);
        StartCoroutine(RemoveAfterDelay(other.gameObject));
        Destroy(gameObject);
    }

    private void PlayPickupEffects()
    {
        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, gameObject.transform.position);

        if (powerEffect != null)
        {
            var vfx = Instantiate(powerEffect, transform.position, Quaternion.identity);
            Destroy(vfx, Duration);
        }
    }

    private IEnumerator RemoveAfterDelay(GameObject player)
    {
        yield return new WaitForSeconds(Duration);
        RemoveEffect(player);
    }


    protected abstract void ApplyEffect(GameObject player);
    protected abstract void RemoveEffect(GameObject player);
}
