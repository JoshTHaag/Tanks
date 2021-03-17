using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Tank : MonoBehaviour
{
    public float groundSpeed;
    public float maxGroundTorque;

    [SerializeField] Animator animator;

    [SerializeField, ReadOnly] Rigidbody2D rb;
    [SerializeField, ReadOnly] WheelJoint2D[] wheels;

    Vector2 debugForce;

    float driveInput = 0f;

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

    private void OnValidate()
    {
        rb = GetComponent<Rigidbody2D>();
        wheels = GetComponentsInChildren<WheelJoint2D>();
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(transform.position, (Vector2)transform.position + debugForce);
    }
}
