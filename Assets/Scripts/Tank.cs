using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Tank : MonoBehaviour
{
    [Header("Ground Movement")]
    public float groundSpeed;
    public float maxGroundTorque;

    [Header("Weapons")]
    public float chargeForcePerSecond;
    public float minFireForce;
    public float maxFireForce;

    [Header("Links")]
    [SerializeField] Animator animator;
    [SerializeField] Transform turretPivot;
    [SerializeField] Transform turretMuzzle;
    [SerializeField] Projectile prefabProjectile;

    [SerializeField, ReadOnly] Rigidbody2D rb;
    [SerializeField, ReadOnly] WheelJoint2D[] wheels;

    float driveInput = 0f;

    float power;

    private void Awake()
    {
        Collider2D bodyCollider = GetComponent<Collider2D>();
        rb.centerOfMass = rb.centerOfMass - (Vector2.up * bodyCollider.bounds.extents);
    }

    private void Update()
    {
        rb.AddForce(Vector2.down);

        animator.SetFloat("Speed", driveInput * groundSpeed * Time.deltaTime);
    }

    public void Drive(float x)
    {
        driveInput = x;

        JointMotor2D jm = new JointMotor2D();
        jm.motorSpeed = -x * groundSpeed;
        jm.maxMotorTorque = maxGroundTorque;

        for (int i = 0; i < wheels.Length; ++i)
        {
            wheels[i].motor = jm;
        }
    }


    public void Aim(float rotation)
    {
        if (rotation < -90f) rotation = 180f;
        else if (rotation < 0f) rotation = 0f;
        turretPivot.transform.localRotation = Quaternion.Euler(0f, 0f, rotation);
    }

    public void SetPower(float power)
    {
        this.power = power;
    }

    public void Fire()
    {
        Projectile proj = Instantiate(prefabProjectile, turretMuzzle.position, turretMuzzle.rotation);
        float force = Mathf.Clamp(power, minFireForce, maxFireForce);
        proj.force = proj.transform.right * force;
    }

    private void OnValidate()
    {
        rb = GetComponent<Rigidbody2D>();
        wheels = GetComponentsInChildren<WheelJoint2D>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + (Vector3)rb.centerOfMass, 0.1f);
    }
}
