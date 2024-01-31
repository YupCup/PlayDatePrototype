using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crank : MonoBehaviour
{
    [HideInInspector] public float crankSpeed = 0;

    float lastZAxis = 0;

    void Update()
    {
        crankSpeed = 0;
        if (Input.GetButton("Fire1")) {
            float zAxis = Vector2.SignedAngle(Vector2.up, Input.mousePosition - transform.position) - 90;
            transform.eulerAngles = Vector3.forward * zAxis;

            crankSpeed = transform.eulerAngles.z - lastZAxis;
            lastZAxis = zAxis;
        }
    }
}
