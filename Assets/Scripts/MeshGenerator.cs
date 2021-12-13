using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;
    Color[] colours;

    public float scale = 1f;
    //public float noise = 0.5f;

    public float offsetX;
    public float offsetZ;

    public int xSize = 20;
    public int zSize = 20;

    public Gradient gradient;

    private float minHeight;
    private float maxHeight;

    public float octaveLayers = 1;
    public float frequency = 1;
    public float layer1Amplitude = 1;
    public float layer2Frequencies = 1;
    public float layer2Amplitude = 1;

    public float MountainFrequencies = 4;
    public float MountainAmplitude = 2;

    public Slider octaveSlider;
    public Slider frequencySlider;
    public Slider scaleSlider;
    public GameObject loadingText;

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
        octaveLayers = octaveSlider.value;
        frequency = frequencySlider.value;
        scale = scaleSlider.value;

        if (Input.GetKeyDown(KeyCode.E))
        {
            loadingText.SetActive(true);

            if (loadingText.activeInHierarchy)
            {
                CalculateOffsets();
                CreateShape();
                UpdateMesh();
                loadingText.SetActive(false);
            }
        }

        
    }

    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        minHeight = 0;
        maxHeight = 0;

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = CalculateHeight(x, z, frequency, scale);
                vertices[i] = new Vector3(x, y, z);

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
                Debug.Log("min: " + minHeight);
                Debug.Log("max: " + maxHeight);
                Debug.Log("height: " + height);
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

    public float CalculateHeight(float x, float z, float freq, float amp)
    {
        float xCoord = (float)(x / xSize);// + offsetX;
        float zCoord = (float)(z / zSize);// + offsetZ;

        float noise = PerlinNoise2D(xCoord, zCoord, freq, amp); ;

        for (int i = 1, power = 2; i <= octaveLayers; i++)
        {
            noise += PerlinNoise2D(xCoord, zCoord, freq * power, amp / power);
            power = power * power;
        }

        noise = Mathf.Pow(noise, scale);
        return noise;
    }

    public float PerlinNoise2D(float x, float z, float freq, float amp)
    {
        float noise = Mathf.PerlinNoise(x * freq, z * freq) * amp;
        return noise;
    }

    public void CalculateOffsets()
    {
        offsetX = Random.Range(0f, 99999f);
        offsetZ = Random.Range(0f, 99999f);
    }

    public void Noise(float nx, float ny)
    {
        //return CalculateHeight(nx, ny) / 2 + 0.5;
    
    }

    public void OnClickPreset()
    {
        octaveSlider.value = 1;
        frequencySlider.value = 3;
        scaleSlider.value = 2;
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
