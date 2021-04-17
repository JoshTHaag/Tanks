using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : NetworkBehaviour
{
    [SerializeField, HideInInspector] Rigidbody2D rb;

    [Header("Settings")]
    public float maxLifeTime = 10f;

    [Header("")]
    public NetworkVariable<Vector2> force = new NetworkVariable<Vector2>(new NetworkVariableSettings() {
        SendTickrate = -1f
    });
    
    float startTime;

    public void Start()
    {
        rb.AddForce(force.Value, ForceMode2D.Impulse);
        startTime = Time.time;
    }

    public void HostInit(float force)
    {
        this.force.Value = transform.right * force;
        NetworkObject.Spawn(null, true);
    }

    private void FixedUpdate()
    {
        Vector2 windForce = Vector2.right;
        windForce.x *= GameManager.Instance.Wind.Value;
        rb.AddForce(windForce * 0.05f);

        if (Time.time > startTime + maxLifeTime)
            DestroyProjectile();
    }

    private void OnDestroy()
    {
        if(IsHost)
            GameManager.Instance.ChangeWind_ServerRpc();
    }

    public void DestroyProjectile()
    {
        if (IsHost)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }

    private void OnValidate()
    {
        if (!rb)
            rb = GetComponent<Rigidbody2D>();
    }
}
