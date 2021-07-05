using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static List<Transform> targets = new List<Transform>();
    private Vector3 camPos;

    private void Update()
    {
        Vector3 totalPos = Vector3.zero;
        Vector2 minVec = targets[0].transform.position;
        Vector2 maxVec = targets[0].transform.position;

        foreach (Transform target in targets)
        {
            if (target.position.x < minVec.x)
            {
                minVec.x = target.position.x;
            }
            if (target.position.y < minVec.y)
            {
                minVec.y = target.position.y;
            }

            if (target.position.x > maxVec.x)
            {
                maxVec.x = target.position.x;
            }
            if (target.position.y > maxVec.y)
            {
                maxVec.y = target.position.y;
            }
        }

        totalPos.x = (minVec.x + maxVec.x) / 2f;
        totalPos.y = (minVec.y + maxVec.y) / 2f;
        totalPos.z = -(maxVec - minVec).magnitude - 5f;

        camPos = totalPos;
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, camPos, Time.fixedDeltaTime);
    }
}
