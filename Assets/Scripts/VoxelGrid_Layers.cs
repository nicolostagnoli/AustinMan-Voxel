using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Mathematics;
using System.IO;
using System;
using Autodesk.Fbx;
using UnityEditor;


public class VoxelGrid_Layers : MonoBehaviour {

    public Vector3Int Dimensions;
    int[,,] Voxels;
    Color[,,] Colors;

    static int[] layers = new int[] {0, 2, 4, 6, 9, 10, 11, 12, 15, 16, 20, 24, 25, 27, 28, 32, 44,
        52, 61, 62, 72, 76, 80, 84, 88, 92, 96, 99, 100, 108, 110, 111, 112, 116,
        124, 128, 132, 136, 144, 148, 152, 156, 160, 164, 165, 168, 172, 179, 180,
        181, 184, 188, 192, 196, 200, 201, 204, 208, 214, 215, 216, 218, 219, 220
    };

    static string[] layerNames = new string[] { "ExternalAir","InternalAir","Colon","ColonInternal","Pons","Medulla","Midbrain","BrainGreyMatter","Urethra","Bladder",
        "Aorta","Duodenum","DuodenumInternal","VentricleAtrium","BloodVessel","BrainWhiteMatter","GallBladder","Cerebellum",
        "HeartMuscle","HeartDCT","Cornea","Prostate","CerebroSpinalFluid","BoneCortical","Kidney","BoneMarrow","EyeSclera",
        "DuctusDeferens","Testis","Liver","PeritonealCavity","Meniscus","Cartilage","Lens","Dura","LungInflated","Nail","Fat",
        "Esophagus","Pancreas","Gland","Tongue","Thymus","SmallIntestine","SmallIntestineInternal","Lymph","Tooth","ErectileTissue",
        "Muscle","Diaphragm","SpinalCord","Spleen","Nerve","VitreousHumor","Stomach","StomachInternal","SkinDry","MucousMembrane",
        "Fascia","ITBand","Trachea","DarkTendon","DCT","Tendon"
    };

    [Range(0, 63)]
    [SerializeField]
    public int selector = 1;
    int lastSelector = 1;

    public TMP_Text text;

    List<List<int>> Triangles = new List<List<int>>(layers.Length);
    List<List<Vector3>> Vertices = new List<List<Vector3>>(layers.Length);
    List<Mesh> Meshes = new List<Mesh>();
    MeshFilter mf;

    private void Awake() {
        Voxels = new int[Dimensions.x, Dimensions.y, Dimensions.z];
        mf = GetComponent<MeshFilter>();
        LoadVoxels();
        GenerateMesh();
    }

    private static Texture2D LoadPNG(string filePath) {
        Texture2D tex = null;
        byte[] fileData;
        fileData = File.ReadAllBytes(filePath);
        tex = new Texture2D(2, 2);
        tex.filterMode = FilterMode.Trilinear;
        tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        tex.wrapMode = TextureWrapMode.Clamp;
        return tex;
    }

    private void LoadVoxels() {
        for (int y = 0; y < Dimensions.y; y+=1) {
            string num = (y*2+1).ToString("D4");
            Texture2D slice = LoadPNG(Application.dataPath + "/Dataset/AustinMan-2x2x2/AustinMan-v2_6-2x2x2_pngs/" + num + ".png");
            for (int x = 0; x < Dimensions.x; x++) {
                for (int z = 0; z < Dimensions.z; z++) {
                    Color pixelColor = slice.GetPixel(x, z);
                    int pixelVal = (int)(pixelColor.grayscale * 255);
                    Voxels[x, y, z] = pixelVal;
                }
            }
        }
    }

    private void GenerateMesh() {

        for(int i = 0; i < layers.Length; i++) {
            Triangles.Add(new List<int>());
            Vertices.Add(new List<Vector3>());
        }

        int[,] Faces = new int[6, 9]{
            {0, 1, 2, 3, 0, 1, 0, 0, 0},     //top
            {7, 6, 5, 4, 0, -1, 0, 1, 0},    //bottom
            {2, 1, 5, 6, 0, 0, 1, 1, 1},     //right
            {0, 3, 7, 4, 0, 0, -1,  1, 1},   //left
            {3, 2, 6, 7, 1, 0, 0,  1, 1},    //front
            {1, 0, 4, 5, -1, 0, 0,  1, 1}    //back
        };

        Vector3[] VertPos = new Vector3[8]{
            new Vector3(-1, 1, -1), new Vector3(-1, 1, 1),
            new Vector3(1, 1, 1), new Vector3(1, 1, -1),
            new Vector3(-1, -1, -1), new Vector3(-1, -1, 1),
            new Vector3(1, -1, 1), new Vector3(1, -1, -1),
        };

        for (int x = 1; x < Dimensions.x - 1; x++) {
            for (int y = 1; y < Dimensions.y - 1; y++) {
                for (int z = 1; z < Dimensions.z - 1; z++) {

                    int layer = Voxels[x, y, z];
                    int meshIndex = Array.IndexOf(layers, layer);
                    for (int o = 0; o < 6; o++)
                        if (Voxels[x + Faces[o, 4], y + Faces[o, 5], z + Faces[o, 6]] != layer)
                            AddQuad(o, Vertices[meshIndex].Count, meshIndex);

                    void AddQuad(int facenum, int v, int meshIndex) {
                        // Add Mesh
                        for (int i = 0; i < 4; i++) Vertices[meshIndex].Add(new Vector3(x, y, z) + VertPos[Faces[facenum, i]] / 2f);
                        Triangles[meshIndex].AddRange(new List<int>() { v, v + 1, v + 2, v, v + 2, v + 3 });

                        // Add uvs
                        // Vector2 bottomleft = new Vector2(Faces[facenum, 7], Faces[facenum, 8]) / 2f;
                        //uv.AddRange(new List<Vector2>() { bottomleft + new Vector2(0, 0.5f), bottomleft + new Vector2(0.5f, 0.5f), bottomleft + new Vector2(0.5f, 0), bottomleft });
                    }
                }
            }
        }
    }

    public void LateUpdate() {
        if(selector != lastSelector) {
            mf.mesh = new Mesh();
            mf.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mf.mesh.vertices = Vertices[selector].ToArray();
            mf.mesh.triangles = Triangles[selector].ToArray();

            lastSelector = selector;

            text.SetText(layerNames[selector]);
        }
    }
}
