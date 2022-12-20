using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System.IO;

public class VoxelGrid : MonoBehaviour {

    public Vector3Int Dimensions;
    float[,,] Voxels;
    Color[,,] Colors;

    private int sliceDimX = 94;
    private int sliceDimZ = 171;

    private void Awake() {
        Dimensions.x = sliceDimX;
        Dimensions.z = sliceDimZ;
        Voxels = new float[Dimensions.x, Dimensions.y, Dimensions.z];
        Colors = new Color[Dimensions.x, Dimensions.y, Dimensions.z];
        LoadVoxels();
        GenerateMesh();
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

    private void LoadVoxels() {
        for (int y = 1; y < Dimensions.y; y+=4) {
            string num = y.ToString("D4");
            Texture2D slice = LoadPNG(Application.dataPath + "/Dataset/AustinMan-4x4x4/AustinMan-v2_6-4x4x4_pngs/" + num + ".png");
            for (int x = 0; x < Dimensions.x; x++) {
                for (int z = 0; z < Dimensions.z; z++) {
                    Color pixelColor = slice.GetPixel(x, z);
                    float pixelVal = pixelColor.grayscale;
                    Voxels[x, (y - 1) / 4, z] = pixelVal;
                    Colors[x, (y - 1) / 4, z] = pixelColor;
                }
            }
        }
    }

    private void GenerateMesh() {
        float at = Time.realtimeSinceStartup;
        List<int> Triangles = new List<int>();
        List<Vector3> Vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();

        for (int x = 1; x < Dimensions.x - 1; x++)
            for (int y = 1; y < Dimensions.y - 1; y++)
                for (int z = 1; z < Dimensions.z - 1; z++) {
                    Vector3[] VertPos = new Vector3[8]{
                        new Vector3(-1, 1, -1), new Vector3(-1, 1, 1),
                        new Vector3(1, 1, 1), new Vector3(1, 1, -1),
                        new Vector3(-1, -1, -1), new Vector3(-1, -1, 1),
                        new Vector3(1, -1, 1), new Vector3(1, -1, -1),
                    };

                    int[,] Faces = new int[6, 9]{
                        {0, 1, 2, 3, 0, 1, 0, 0, 0},     //top
                        {7, 6, 5, 4, 0, -1, 0, 1, 0},   //bottom
                        {2, 1, 5, 6, 0, 0, 1, 1, 1},     //right
                        {0, 3, 7, 4, 0, 0, -1,  1, 1},   //left
                        {3, 2, 6, 7, 1, 0, 0,  1, 1},    //front
                        {1, 0, 4, 5, -1, 0, 0,  1, 1}    //back
                    };

                    if (Voxels[x, y ,z] != 0)
                        for (int o = 0; o < 6; o++)
                            if (Voxels[x + Faces[o, 4], y + Faces[o, 5], z + Faces[o, 6]] == 0)
                                AddQuad(o, Vertices.Count);

                    void AddQuad(int facenum, int v) {
                        // Add Mesh
                        for (int i = 0; i < 4; i++) Vertices.Add(new Vector3(x, y, z) + VertPos[Faces[facenum, i]] / 2f);
                        Triangles.AddRange(new List<int>() { v, v + 1, v + 2, v, v + 2, v + 3 });

                        // Add uvs
                       // Vector2 bottomleft = new Vector2(Faces[facenum, 7], Faces[facenum, 8]) / 2f;

                        //uv.AddRange(new List<Vector2>() { bottomleft + new Vector2(0, 0.5f), bottomleft + new Vector2(0.5f, 0.5f), bottomleft + new Vector2(0.5f, 0), bottomleft });
                    }
                }

        MeshFilter mf = GetComponent<MeshFilter>();
        mf.mesh = new Mesh();
        mf.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mf.mesh.vertices = Vertices.ToArray();
        mf.mesh.triangles = Triangles.ToArray();

    }

    public void Update() {

    }
}
