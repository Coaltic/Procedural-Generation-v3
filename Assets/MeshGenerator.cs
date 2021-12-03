using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;
    Color[] colours;

    public float scale = 20f;

    public float offsetX;
    public float offsetZ;

    public int xSize = 20;
    public int zSize = 20;

    public Gradient gradient;

    private float minHeight;
    private float maxHeight;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CalculateOffsets();

        CreateShape();
        UpdateMesh();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CalculateOffsets();
            CreateShape();
            UpdateMesh();
        }
    }

    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = CalculateHeight(x, z);
                if (y > 0) { y = -(y); }
                vertices[i] = new Vector3(x, y * scale, z);

                if (y > maxHeight) { maxHeight = y; }
                if (y < minHeight) { minHeight = y; }

                i++;
            }
            
        }

        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;

                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        colours = new Color[vertices.Length];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(minHeight, maxHeight, vertices[i].y);
                colours[i] = gradient.Evaluate(height);
                i++;
            }
            
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colours;

        mesh.RecalculateNormals();
    }

    public float CalculateHeight(int x, int z)
    {
        float xCoord = (float)x / scale + offsetX;
        float zCoord = (float)z / scale + offsetZ;

        return Mathf.PerlinNoise(xCoord, zCoord);
    }

    public void CalculateOffsets()
    {
        offsetX = Random.Range(0f, 99999f);
        offsetZ = Random.Range(0f, 99999f);
    }

    private void OnDrawGizmos()
    {
        if (vertices == null)
            return;

        for (int i = 0; i <= vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }
}
