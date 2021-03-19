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
    }
}


//float fireChargeTimestamp;
//bool isFireCharging = false;

//public void Aim(Vector2 targetPosition)
//{
//    Vector2 targetVector = (targetPosition - (Vector2)transform.position).normalized;
//    float rotation = Mathf.Atan2(targetVector.y, targetVector.x) * Mathf.Rad2Deg;
//    if (rotation < -90f) rotation = 180f;
//    else if (rotation < 0f) rotation = 0f;
//    turretPivot.transform.localRotation = Quaternion.Euler(0f, 0f, rotation);
//}


//public void Fire()
//{
//    if (!isFireCharging)
//    {
//        fireChargeTimestamp = Time.time;
//    }
//    else
//    {
//        isFireCharging = false;
//    }

//    Projectile proj = Instantiate(prefabProjectile, turretMuzzle.position, turretMuzzle.rotation);
//    float force = Mathf.Clamp(((Time.time - fireChargeTimestamp) * chargeForcePerSecond), minFireForce, maxFireForce);
//    proj.force = proj.transform.right * force;
//}


//public void ChargeFire()
//{
//    if (!isFireCharging)
//    {
//        isFireCharging = true;
//        fireChargeTimestamp = Time.time;
//    }
//}
