using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float projectileSpeed = 5f;

    private void Update()
    {
        transform.Translate(new Vector2(projectileSpeed * Time.deltaTime, 0f));
    }
}
