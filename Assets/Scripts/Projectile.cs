using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField, HideInInspector] Rigidbody2D rb;

    [Header("Settings")]
    public float maxLifeTime = 10f;

    [Header("")]
    public Vector2 force;

    float startTime;

    public void Start()
    {
        rb.AddForce(force, ForceMode2D.Impulse);
        startTime = Time.time;
    }

    private void FixedUpdate()
    {
        Vector2 windForce = Vector2.right;
        windForce.x *= GameManager.Instance.Wind;
        rb.AddForce(windForce * 0.05f);

        if (Time.time > startTime + maxLifeTime)
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        GameManager.Instance.ChangeWind();
    }

    private void OnValidate()
    {
        if (!rb)
            rb = GetComponent<Rigidbody2D>();
    }
}
