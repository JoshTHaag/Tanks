using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetMovement : MonoBehaviour
{
    [HideInInspector]
    public Vector3 velocity;

    void Update()
    {
        transform.Translate(velocity * Time.deltaTime);
    }
}
