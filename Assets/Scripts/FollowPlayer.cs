using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform followTarget;

    public float smoothSpeed = 0.1f;

    private void LateUpdate()
    {
        Vector3 targetPosition = new Vector3(followTarget.position.x, followTarget.position.y, transform.position.z);

        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}
