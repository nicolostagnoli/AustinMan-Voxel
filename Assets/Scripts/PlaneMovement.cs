using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneMovement : MonoBehaviour
{
    public float movement_speed;
    public float rotation_speed;
    float scale_speed = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = transform.localPosition;
        if (Input.GetKey(KeyCode.UpArrow)) {
            transform.localPosition = new Vector3(position.x, position.y, position.z + movement_speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.DownArrow)) {
            transform.localPosition = new Vector3(position.x, position.y, position.z - movement_speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.LeftArrow)) {
            transform.localPosition = new Vector3(position.x, position.y - movement_speed * Time.deltaTime, position.z);
        }
        if (Input.GetKey(KeyCode.RightArrow)) {
            transform.localPosition = new Vector3(position.x, position.y + movement_speed * Time.deltaTime, position.z);
        }
        if (Input.GetKey(KeyCode.RightShift)) {
            transform.localPosition = new Vector3(position.x - movement_speed * Time.deltaTime, position.y, position.z);
        }
        if (Input.GetKey(KeyCode.RightControl)) {
            transform.localPosition = new Vector3(position.x + movement_speed * Time.deltaTime, position.y, position.z);
        }

        if (Input.GetKey(KeyCode.J)) {
            Vector3 scale = transform.localScale;
            scale.x -= Time.deltaTime * scale_speed;
            scale.y -= Time.deltaTime * scale_speed;
            transform.localScale = scale;
        }
        if (Input.GetKey(KeyCode.K)) {
            Vector3 scale = transform.localScale;
            scale.x += Time.deltaTime * scale_speed;
            scale.y += Time.deltaTime * scale_speed;
            transform.localScale = scale;
        }

        Vector3 angles = transform.localRotation.eulerAngles;
        if (Input.GetKey(KeyCode.E)) {
            transform.localRotation = Quaternion.Euler(angles.x + rotation_speed * Time.deltaTime, angles.y, angles.z);
        }
        if (Input.GetKey(KeyCode.Q)) {
            transform.localRotation = Quaternion.Euler(angles.x - rotation_speed * Time.deltaTime, angles.y, angles.z);
        }
        if (Input.GetKey(KeyCode.R)) {
            transform.localRotation = Quaternion.Euler(angles.x, angles.y - rotation_speed * Time.deltaTime, angles.z);
        }
        if (Input.GetKey(KeyCode.F)) {
            transform.localRotation = Quaternion.Euler(angles.x, angles.y + rotation_speed * Time.deltaTime, angles.z);
        }
    }
}
