using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField, HideInInspector] Rigidbody2D rb;

    public Vector2 force;

    public void Start()
    {
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    private void OnValidate()
    {
        if (!rb)
            rb = GetComponent<Rigidbody2D>();
    }
}
