using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset_position : MonoBehaviour
{
    Vector3 initialPosition;
    Quaternion initialRotation;
    void Start()
    {
        initialPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        initialRotation = new Quaternion(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z, transform.localRotation.w);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            transform.localPosition = initialPosition;
            transform.localRotation = initialRotation;
        }
    }
}
