using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MRI_Texture_MeshCollider : MonoBehaviour
{
    public GameObject plane;
    MeshFilter planeMf;
    Material planeMaterial;

    public GameObject screen;
    Material screenMaterial;
    MeshFilter screenMf;

    Texture3D tex3d;

    public GameObject human;
    SkinnedMeshRenderer smr;
    MeshCollider meshCollider;
    Mesh originalMesh;
    Mesh mesh;

    public Vector3 scaleUV;
    public Vector3 offsetUV;
    public Vector3 screenScaleUV;
    public Vector3 screenOffsetUV;

    public int width = 342;
    public int height = 1876/2;
    public int depth = 342;

    // Start is called before the first frame update
    void Start() {
        planeMaterial = plane.GetComponent<MeshRenderer>().material;
        planeMf = plane.GetComponent<MeshFilter>();

        screenMaterial = screen.GetComponent<MeshRenderer>().material;
        screenMf = screen.GetComponent<MeshFilter>();

        tex3d = new Texture3D(width, height, depth, TextureFormat.RGBA32, false);
        tex3d.wrapMode = TextureWrapMode.Clamp;
        tex3d.filterMode = FilterMode.Trilinear;

        createTex3D();

        /*
        //Create mesh for the mesh collider
        smr = human.GetComponent<SkinnedMeshRenderer>();
        DestroyImmediate(human.GetComponent<MeshCollider>());
        meshCollider = human.AddComponent<MeshCollider>();
        originalMesh = new Mesh();
        smr.BakeMesh(originalMesh);
        List<Vector3> vert = new List<Vector3>(originalMesh.vertices);
        for (int i = 0; i < vert.Count; i++) {
            Vector3 v = vert[i];
            v.x *= 0.75f;
            vert[i] = v;
        }
        originalMesh.SetVertices(vert);
        meshCollider.sharedMesh = originalMesh;
        mesh = originalMesh;
        */
    }

    void createTex3D() {
        Color32[] colors = new Color32[tex3d.width * tex3d.height * tex3d.depth];

        for (int y = 1; y < tex3d.height * 2; y += 2) {
            string num = y.ToString("D4");
            Texture2D slice = LoadPNG(Application.dataPath + "/Dataset/AustinMan-2x2x2/AustinMan-v2_6-2x2x2_pngs/" + num + ".png");

            for (int z = 0; z < tex3d.depth; z++) {
                for (int x = 0; x < tex3d.width; x++) {
                    int tempy = (y - 1) / 2;
                    int idx = x + tex3d.width * tempy + tex3d.width * tex3d.height * z;
                    Color32 temp = (Color32)slice.GetPixel(x, z);
                    colors[idx] = temp;
                }
            }
        }

        tex3d.SetPixels32(colors);
        tex3d.Apply();

        planeMaterial.SetTexture("_MainTex", tex3d);
        screenMaterial.SetTexture("_MainTex", tex3d);
    }

    // Update is called once per frame
    void Update() {
        if (plane.transform.hasChanged) {
            calculate3dUVW();
            plane.transform.hasChanged = false;
        }

        /*
        DestroyImmediate(human.GetComponent<MeshCollider>());
        meshCollider = human.AddComponent<MeshCollider>();
        mesh = new Mesh();
        smr.BakeMesh(mesh);
        List<Vector3> vert = new List<Vector3>(mesh.vertices);
        for(int i = 0; i < vert.Count; i++) {
            Vector3 v = vert[i];
            v.x *= 0.75f;
            vert[i] = v;
        }
        mesh.SetVertices(vert);
        meshCollider.sharedMesh = mesh;
        */
    }

    void calculate3dUVW() {
        List<Vector3> planeUvs = new List<Vector3>(planeMf.mesh.vertices);
        List<Vector3> screenUvs = new List<Vector3>(planeMf.mesh.vertices);
        List<Vector3> planeVerts = new List<Vector3>(planeMf.mesh.vertices);

        for (int i = 0; i < planeUvs.Count; i++) {
            Vector3 point = plane.transform.TransformPoint(planeVerts[i]);

            /*
            //Find index of vertex intersecting between plane and mesh collider
            Vector3 meshPoint = human.transform.InverseTransformPoint(point);
            int idx = new List<Vector3>(mesh.vertices).IndexOf(meshPoint);

            if (idx != -1) {
                Vector3 originalPosition = originalMesh.vertices[idx];

                Vector3 translation = originalPosition - point;
                //Quaternion rotation = bc.originalRot * Quaternion.Inverse(bc.gameObject.transform.rotation);
                //point = RotatePointAroundPivot(point, bc.gameObject.transform.position, rotation.eulerAngles);

                point += translation;
            }
            */

            planeUvs[i] = Vector3.Scale(point, scaleUV) - offsetUV;

            screenUvs[i] = Vector3.Scale(planeUvs[i], screenScaleUV) - screenOffsetUV;
        }

        planeMf.mesh.SetUVs(0, planeUvs);
        screenMf.mesh.SetUVs(0, screenUvs);
    }

    static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
        Vector3 dir = point - pivot;
        dir = Quaternion.Euler(angles) * dir;
        point = dir + pivot;
        return point;
    }

    static Texture2D LoadPNG(string filePath) {
        Texture2D tex = null;
        byte[] fileData;
        fileData = File.ReadAllBytes(filePath);
        tex = new Texture2D(2, 2);
        tex.filterMode = FilterMode.Point;
        tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        return tex;
    }
}
