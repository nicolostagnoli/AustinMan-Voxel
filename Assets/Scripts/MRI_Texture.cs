using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MRI_Texture : MonoBehaviour
{
    MeshRenderer mr;
    MeshFilter mf;
    Material mat;
    Texture3D tex3d;

    public GameObject screen;
    Material screenMaterial;
    MeshFilter screenMf;

    public Vector3 scaleUV;
    public Vector3 screenScaleUV;
    public Vector3 offsetUV;

    [SerializeField]
    int width = 187;
    [SerializeField]
    int height = 1876;
    [SerializeField]
    int depth = 342;

    // Start is called before the first frame update
    void Start() {
        mr = GetComponent<MeshRenderer>();
        mf = GetComponent<MeshFilter>();
        mat = mr.material;
        tex3d = new Texture3D(width, height, depth, TextureFormat.RGBA32, false);
        tex3d.wrapMode = TextureWrapMode.Clamp;
        tex3d.filterMode = FilterMode.Trilinear;

        screenMaterial = screen.GetComponent<MeshRenderer>().material;
        screenMf = screen.GetComponent<MeshFilter>();

        createTex3D();
    }

    void createTex3D() {

        Color32[] colors = new Color32[tex3d.width * tex3d.height * tex3d.depth];

        for (int y = 0; y < tex3d.height; y++) {
            string num =(y + 1 - (y % 2)).ToString("D4");
            Texture2D slice = LoadPNG("C:/Users/n/Documents/GitHub/AustinMan-Voxel/Assets/Dataset/AustinMan-2x2x2/AustinMan-v2_6-2x2x2_pngs/" + num + ".png");
            for (int z = 0; z < tex3d.depth; z++) {
                for (int x = 0; x < tex3d.width; x++) {
                    int idx = x + tex3d.width * y + tex3d.width * tex3d.height * z;
                    Color32 temp = (Color32)slice.GetPixel(x, z);
                    colors[idx] = temp;
                }
            }
        }
        tex3d.SetPixels32(colors);
        tex3d.Apply();
        mat.SetTexture("_MainTex", tex3d);
        screenMaterial.SetTexture("_MainTex", tex3d);
    }

    // Update is called once per frame
    void Update() {
        if (transform.hasChanged) {
            calculate3dUVW();

            transform.hasChanged = false;
        }
    }

    void calculate3dUVW() {
        List<Vector3> newVert = new List<Vector3>(mf.mesh.vertices);
        for (int i = 0; i < newVert.Count; i++) {
            newVert[i] = Vector3.Scale(transform.TransformPoint(newVert[i]), scaleUV) - offsetUV;
        }
        List<Vector3> newVertScreen = new List<Vector3>(newVert);
        for (int i = 0; i < newVert.Count; i++) {
            newVertScreen[i] = Vector3.Scale(newVertScreen[i], screenScaleUV);
        }
        mf.mesh.SetUVs(0, newVert);
        screenMf.mesh.SetUVs(0, newVertScreen);
    }


    private static Texture2D LoadPNG(string filePath) {
        Texture2D tex = null;
        byte[] fileData;
        fileData = File.ReadAllBytes(filePath);
        tex = new Texture2D(2, 2);
        tex.filterMode = FilterMode.Point;
        tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        return tex;
    }
}
