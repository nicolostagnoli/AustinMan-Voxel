using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MRI_Texture : MonoBehaviour
{
    public GameObject plane;
    MeshFilter planeMf;
    Material planeMaterial;

    public GameObject screen;
    Material screenMaterial;
    MeshFilter screenMf;

    private class BodyCollider {
        public Collider collider;
        public Vector3 originalPos;
        public Quaternion originalRot;
        public GameObject gameObject;

        public BodyCollider(Collider c, Vector3 op, Quaternion or, GameObject go) {
            this.collider = c;
            this.originalPos = op;
            this.originalRot = or;
            this.gameObject = go;
        }
    }

    public List<GameObject> bodyColliderObjects;
    List<BodyCollider> bodyColliders;
    List<Collider> originalBodyColliders;

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

        bodyColliders = new List<BodyCollider>(bodyColliderObjects.Count);
        foreach (GameObject o in bodyColliderObjects) {
            Vector3 pos = new Vector3(o.transform.position.x, o.transform.position.y, o.transform.position.z);
            Quaternion rot = new Quaternion(o.transform.rotation.x, o.transform.rotation.y, o.transform.rotation.z, o.transform.rotation.w);
            Collider coll = o.GetComponent<Collider>();
            BodyCollider bc = new BodyCollider(coll, pos, rot, o);
            bodyColliders.Add(bc);
        }
        originalBodyColliders = new List<Collider>(bodyColliderObjects.Count);
        foreach (GameObject o in bodyColliderObjects) {
            GameObject go = Instantiate(o, o.transform.position, o.transform.rotation);
            Collider c = go.GetComponent<Collider>();
            originalBodyColliders.Add(c);
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
            calculate3dUVW();
            plane.transform.hasChanged = false;
        }
    }

    void calculate3dUVW() {
        List<Vector3> planeUvs = new List<Vector3>(planeMf.mesh.vertices);
        List<Vector3> screenUvs = new List<Vector3>(planeMf.mesh.vertices);
        List<Vector3> planeVerts = new List<Vector3>(planeMf.mesh.vertices);

        for (int i = 0; i < planeUvs.Count; i++) {
            planeUvs[i] = Vector3.Scale(plane.transform.TransformPoint(planeVerts[i]), scaleUV) - offsetUV;

            Vector3 point = plane.transform.TransformPoint(planeVerts[i]);

            foreach (Collider c in originalBodyColliders) { //hide original regions (if the model rig is changed)
                if (c.ClosestPoint(point).Equals(point)) {
                    planeUvs[i] = new Vector3(10, 10, 10);
                }
            }

            foreach (BodyCollider bc in bodyColliders) {  //apply rig deformation to uvw coordinates
                if (bc.collider.ClosestPoint(point).Equals(point)) {
                    
                    Vector3 translation = bc.originalPos - bc.gameObject.transform.position;
                    Quaternion rotation = bc.originalRot * Quaternion.Inverse(bc.gameObject.transform.rotation);

                    point = RotatePointAroundPivot(point, bc.gameObject.transform.position, rotation.eulerAngles);
                    point += translation;

                    planeUvs[i] = Vector3.Scale(point, scaleUV) - offsetUV;
                }

                screenUvs[i] = Vector3.Scale(planeUvs[i], screenScaleUV) - screenOffsetUV;
            }
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
