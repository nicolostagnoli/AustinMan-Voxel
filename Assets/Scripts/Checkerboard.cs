using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Checkerboard : MonoBehaviour
{
    MeshRenderer mr;
    MeshFilter mf;
    Material mat;
    Texture2D tex;
    Texture3D tex3d;
    [SerializeField]
    float width = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        mf = GetComponent<MeshFilter>();
        mat = mr.sharedMaterial;
        tex3d = new Texture3D(256, 256, 256, TextureFormat.RGBA32, false);
        tex3d.wrapMode = TextureWrapMode.Clamp;
        tex3d.filterMode = FilterMode.Point;
        createTex3D();
    }

    void createTex3D() {

        Color32[] colors = new Color32[tex3d.width * tex3d.height * tex3d.depth];

        for (int z = 0; z < tex3d.depth; z++) {
            for (int y = 0; y < tex3d.height; y++) {
                for (int x = 0; x < tex3d.width; x++) {
                    int idx = x + tex3d.width * y + tex3d.width * tex3d.height * z;
                    Color32 temp = evalPixel(x, y, z);
                    colors[idx] = temp;
                }
            }
        }
        tex3d.SetPixels32(colors);
        tex3d.Apply();
        mat.SetTexture("_MainTex", tex3d);
    }

    Color evalPixel(int x, int y, int z) {

        float valX = (x % (width * 2.0f)) / (width * 2.0f);
        int vX = 1;
        if(valX < 0.5f) 
            vX = 0;

        float valY = (y % (width * 2.0f)) / (width * 2.0f);
        int vY = 1;
        if (valY < 0.5f) 
            vY = 0;

        float valZ = (z % (width * 2.0f)) / (width * 2.0f);
        int vZ = 1;
        if (valZ < 0.5f)
            vZ = 0;

        byte value = 0;
        if (vX == vY) {
            if (vZ == 1) {
                value = 255;
            }
        }
        else {
            if (vZ == 0) {
                value = 255;
            }
        }

        return new Color32(value, value, value, 255);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.hasChanged) {
            calculate3dUVW();
            transform.hasChanged = false;
        }
    }

    void calculate3dUVW() {
        List<Vector3> newVert = new List<Vector3>(mf.sharedMesh.vertices);
        for(int i = 0; i < newVert.Count; i++) {
            newVert[i] = transform.TransformPoint(newVert[i]);
        }
        mf.mesh.SetUVs(0, newVert);
    }
}
