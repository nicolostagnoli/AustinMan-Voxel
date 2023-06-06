using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset_position : MonoBehaviour
{
    Vector3 initialPosition;
    Quaternion initialRotation;
    Vector3 initialScale;
    void Start()
    {
        initialPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        initialRotation = new Quaternion(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z, transform.localRotation.w);
        initialScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X)) {
            ResetPositions();
        }
    }

    public void ResetPositions() {
        transform.localPosition = initialPosition;
        transform.localRotation = initialRotation;
        transform.localScale = initialScale;
    }
}
