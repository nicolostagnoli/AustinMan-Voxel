using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class MRI_Texture_MeshCollider : MonoBehaviour
{
    public GameObject plane;
    MeshFilter planeMf;
    Material planeMaterial;

    public GameObject screen;
    Material screenMaterial;
    MeshFilter screenMf;

    public List<GameObject> bones;
    List<Vector3> originalBonesPosition;
    List<Quaternion> originalBonesRotation;
    List<MeshCollider> colliders;

    Texture3D tex3d;

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

        originalBonesPosition = new List<Vector3>(bones.Count);
        originalBonesRotation = new List<Quaternion>(bones.Count);
        foreach (GameObject go in bones) {
            originalBonesPosition.Add(new Vector3(go.transform.position.x, go.transform.position.y, go.transform.position.z));
            originalBonesRotation.Add(new Quaternion(go.transform.rotation.x, go.transform.rotation.y, go.transform.rotation.z, go.transform.rotation.w));
        }
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
            GenerateColliders();
            calculate3dUVW();
            plane.transform.hasChanged = false;
        }
    }

    void calculate3dUVW() {
        List<Vector3> planeUvs = new List<Vector3>(planeMf.mesh.vertices);
        List<Vector3> screenUvs = new List<Vector3>(planeMf.mesh.vertices);
        List<Vector3> planeVerts = new List<Vector3>(planeMf.mesh.vertices);

        for (int i = 0; i < planeUvs.Count; i++) {
            Vector3 point = plane.transform.TransformPoint(planeVerts[i]);  //each plane point in world coordinates

            MeshCollider coll = null;
            foreach(MeshCollider mc in colliders) {
                if (mc.ClosestPoint(point).Equals(point)) {
                    coll = mc;
                    break;
                }
            }

            //if point does not intersect any collier, set color to black
            //(setting uv far away from 0-1 range)
            if(coll == null) {
                point.x = 99999.0f;
            }
            else {
                int idx = colliders.IndexOf(coll);
                Vector3 origPos = originalBonesPosition[idx];
                Quaternion origRot = originalBonesRotation[idx];

                Vector3 translation = origPos - bones[idx].transform.position;
                Quaternion rotation = origRot * Quaternion.Inverse(bones[idx].transform.rotation);

                point = RotatePointAroundPivot(point, bones[idx].transform.position, rotation.eulerAngles);
                point += translation;
            }

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

    private void GenerateColliders() {
        colliders = new List<MeshCollider>(bones.Count);
        foreach (GameObject go in bones) {
            colliders.Add(go.GetComponent<MeshCollider>());
        }
    }
}
