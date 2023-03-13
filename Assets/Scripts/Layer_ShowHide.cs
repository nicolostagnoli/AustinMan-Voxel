using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer_ShowHide : MonoBehaviour
{
    List<GameObject> bodyLayers;
    public GameObject skin;

    // Start is called before the first frame update
    void Start()
    {
        bodyLayers = new List<GameObject>(GameObject.FindGameObjectsWithTag("BodyLayer"));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            bodyLayers[0].SetActive(!bodyLayers[0].activeInHierarchy);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            bodyLayers[1].SetActive(!bodyLayers[1].activeInHierarchy);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            bodyLayers[2].SetActive(!bodyLayers[2].activeInHierarchy);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            bodyLayers[3].SetActive(!bodyLayers[3].activeInHierarchy);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5)) {
            bodyLayers[4].SetActive(!bodyLayers[4].activeInHierarchy);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6)) {
            bodyLayers[5].SetActive(!bodyLayers[5].activeInHierarchy);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7)) {
            bodyLayers[6].SetActive(!bodyLayers[6].activeInHierarchy);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8)) {
            bodyLayers[7].SetActive(!bodyLayers[7].activeInHierarchy);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9)) {
            bodyLayers[8].SetActive(!bodyLayers[8].activeInHierarchy);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            skin.SetActive(!skin.activeInHierarchy);
        }

    }
}
