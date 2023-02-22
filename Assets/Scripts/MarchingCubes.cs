using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class MarchingCubes : MonoBehaviour
{
	public Vector3Int gridSize;

	public float isoLevel = 0.5f;

	public MeshFilter MeshFilter;
	Mesh mesh;

	float[,,] pointValues;

	Vector3 currentCubePosition;

	List<Vector3> vertices = new List<Vector3>();
	List<int> triangles = new List<int>();

	// Start is called before the first frame update
	private void Start() {
		mesh = new Mesh();
		CreateGrid();
		March();
	}

	// Update is called once per frame
	void Update()
    {
        
    }

	void CreateGrid() {
		pointValues = new float[gridSize.x, gridSize.y, gridSize.z];

		for (int y = 0; y < gridSize.y; y++) {
			string num = (y*4 + 1).ToString("D4");
			Texture2D slice = LoadPNG(Application.dataPath + "/Dataset/AustinMan-4x4x4/AustinMan-v2_6-4x4x4_pngs/" + num + ".png");
			for (int x = 0; x < gridSize.x; x++) {
				for (int z = 0; z < gridSize.z; z++) {
					pointValues[x, y, z] = 1.0f - slice.GetPixel(x, z).grayscale;
				}
			}
		}
	}

	void AddTriangle(Vector3 a, Vector3 b, Vector3 c) {
		int triIndex = triangles.Count;
		vertices.Add(a);
		vertices.Add(b);
		vertices.Add(c);
		triangles.Add(triIndex);
		triangles.Add(triIndex + 1);
		triangles.Add(triIndex + 2);
	}

	Vector3 Interp(Vector3 edgeVertex1, float valueAtVertex1, Vector3 edgeVertex2, float valueAtVertex2) {
		return (edgeVertex1 + (isoLevel - valueAtVertex1) * (edgeVertex2 - edgeVertex1) / (valueAtVertex2 - valueAtVertex1));
	}

	void March() {

		for (int x = 0; x < gridSize.x - 1; x++) {
			for (int y = 0; y < gridSize.y - 1; y++) {
				for (int z = 0; z < gridSize.z - 1; z++) {

					currentCubePosition = new Vector3(x + .5f, y + .5f, z + .5f);

					// Set values at the corners of the cube
					float[] cubeValues = new float[] {
						pointValues[x, y, z + 1],
						pointValues[x + 1, y, z + 1],
						pointValues[x + 1, y, z],
						pointValues[x, y, z],
						pointValues[x, y + 1, z + 1],
						pointValues[x + 1, y + 1, z + 1],
						pointValues[x + 1, y + 1, z],
						pointValues[x, y + 1, z]
					};

					// Find the triangulation index
					int cubeIndex = 0;
					if (cubeValues[0] < isoLevel) cubeIndex |= 1;
					if (cubeValues[1] < isoLevel) cubeIndex |= 2;
					if (cubeValues[2] < isoLevel) cubeIndex |= 4;
					if (cubeValues[3] < isoLevel) cubeIndex |= 8;
					if (cubeValues[4] < isoLevel) cubeIndex |= 16;
					if (cubeValues[5] < isoLevel) cubeIndex |= 32;
					if (cubeValues[6] < isoLevel) cubeIndex |= 64;
					if (cubeValues[7] < isoLevel) cubeIndex |= 128;

					// Get the intersecting edges
					int[] edges = MarchingCubesTables.triTable[cubeIndex];

					Vector3 worldPos = new Vector3(x, y, z);

					int triCount = triangles.Count;

					// Triangulate
					for (int i = 0; edges[i] != -1; i += 3) {
						int e00 = MarchingCubesTables.edgeConnections[edges[i]][0];
						int e01 = MarchingCubesTables.edgeConnections[edges[i]][1];

						int e10 = MarchingCubesTables.edgeConnections[edges[i + 1]][0];
						int e11 = MarchingCubesTables.edgeConnections[edges[i + 1]][1];

						int e20 = MarchingCubesTables.edgeConnections[edges[i + 2]][0];
						int e21 = MarchingCubesTables.edgeConnections[edges[i + 2]][1];

						Vector3 a = Interp(MarchingCubesTables.cubeCorners[e00], cubeValues[e00], MarchingCubesTables.cubeCorners[e01], cubeValues[e01]) + worldPos;
						Vector3 b = Interp(MarchingCubesTables.cubeCorners[e10], cubeValues[e10], MarchingCubesTables.cubeCorners[e11], cubeValues[e11]) + worldPos;
						Vector3 c = Interp(MarchingCubesTables.cubeCorners[e20], cubeValues[e20], MarchingCubesTables.cubeCorners[e21], cubeValues[e21]) + worldPos;

						AddTriangle(a, b, c);
					}
				}
			}
		}

		mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
		mesh.SetVertices(vertices);
		mesh.SetTriangles(triangles, 0);
		mesh.RecalculateNormals();
		mesh.triangles = mesh.triangles.Reverse().ToArray();
		MeshFilter.mesh = mesh;
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

	private void OnDrawGizmos() {
		if (pointValues == null) {
			return;
		}
		for (int x = 0; x < gridSize.x; x++) {
			for (int y = 0; y < gridSize.y; y++) {
				for (int z = 0; z < gridSize.z; z++) {
					Gizmos.color = Color.Lerp(Color.black, Color.white, pointValues[x, y, z]);
					Gizmos.DrawCube(new Vector3(x, y, z), Vector3.one * .1f);
				}
			}
		}

		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(currentCubePosition, Vector3.one);
	}
}
