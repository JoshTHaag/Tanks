using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct StateMessage
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 velocity;
    public Vector3 angularVelocity;
    public int tickNumber;
}
