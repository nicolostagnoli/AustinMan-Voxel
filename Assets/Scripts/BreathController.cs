using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathController : MonoBehaviour
{
    Animation anim;
    bool running;
    public MRI_Texture_MeshCollider mri;
    List<GameObject> resetObjects;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animation>();
        running = anim.isPlaying;
        resetObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Bone"));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) {
            if (running) {
                anim.Stop();
                running = false;
                foreach(GameObject go in resetObjects) {
                    go.transform.localScale = Vector3.one;
                }
            }
            else {
                anim.Play();
                running = true;
            }
        }

        if (running) mri.calculate3dUVW();
    }
}
