using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TerrainGenerator : MonoBehaviour
{
    private MeshFilter _meshFilter;    
    private int chunkWidth = 16, chunkHeight = 10;

    public GameObject Vertex;
    public bool debug = false;

    void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
        Generate();
        //_meshRenderer.material = (Material)AssetDatabase.LoadAssetAtPath<Material>("Assets/StockMaterial.mat");
    }

    private void Generate()
    {
        int blockCount = 0;
        Vector3[] vertices = new Vector3[8 * 3 * chunkWidth * chunkWidth * chunkHeight];
        int[] triangles = new int[36 * chunkWidth * chunkWidth * chunkHeight];
        Vector2[] uvs = new Vector2[8 * 3 * chunkWidth * chunkWidth * chunkHeight];

        bool[,,] isOpaqueBlock = new bool[chunkWidth, chunkHeight, chunkWidth];

        //First pass, for boolean array
        for (int y = 0; y < chunkHeight; y++)
        {
            for (int z = 0; z < chunkWidth; z++)
            {
                for (int x = 0; x < chunkWidth; x++)
                {
                    isOpaqueBlock[x, y, z] = true;
                }
            }
        }


        //Second pass, actual mesh generation
        for (int y = 0; y < chunkHeight; y++)
        {
            for (int x = 0; x < chunkWidth; x++)
            {
                for (int z = 0; z < chunkWidth; z++)
                {
                    /* 
                    * Assign vertices
                    * Assign triangles
                    */

                    #region verts

                    //First (y)
                    vertices[0 + (blockCount * 24)] = new Vector3(0 - x, 0 + y, 0 + z);
                    vertices[1 + (blockCount * 24)] = new Vector3(0 - x, 0 + y, 1 + z);
                    vertices[2 + (blockCount * 24)] = new Vector3(1 - x, 0 + y, 1 + z);
                    vertices[3 + (blockCount * 24)] = new Vector3(1 - x, 0 + y, 0 + z);

                    vertices[4 + (blockCount * 24)] = new Vector3(0 - x, -1 + y, 0 + z);
                    vertices[5 + (blockCount * 24)] = new Vector3(0 - x, -1 + y, 1 + z);
                    vertices[6 + (blockCount * 24)] = new Vector3(1 - x, -1 + y, 1 + z);
                    vertices[7 + (blockCount * 24)] = new Vector3(1 - x, -1 + y, 0 + z);

                    //Second (z)
                    vertices[8 + (blockCount * 24)] = new Vector3(0 - x, 0 + y, 0 + z);
                    vertices[9 + (blockCount * 24)] = new Vector3(0 - x, 0 + y, 1 + z);
                    vertices[10 + (blockCount * 24)] = new Vector3(1 - x, 0 + y, 1 + z);
                    vertices[11 + (blockCount * 24)] = new Vector3(1 - x, 0 + y, 0 + z);

                    vertices[12 + (blockCount * 24)] = new Vector3(0 - x, -1 + y, 0 + z);
                    vertices[13 + (blockCount * 24)] = new Vector3(0 - x, -1 + y, 1 + z);
                    vertices[14 + (blockCount * 24)] = new Vector3(1 - x, -1 + y, 1 + z);
                    vertices[15 + (blockCount * 24)] = new Vector3(1 - x, -1 + y, 0 + z);

                    //Third (x)
                    vertices[16 + (blockCount * 24)] = new Vector3(0 - x, 0 + y, 0 + z);
                    vertices[17 + (blockCount * 24)] = new Vector3(0 - x, 0 + y, 1 + z);
                    vertices[18 + (blockCount * 24)] = new Vector3(1 - x, 0 + y, 1 + z);
                    vertices[19 + (blockCount * 24)] = new Vector3(1 - x, 0 + y, 0 + z);

                    vertices[20 + (blockCount * 24)] = new Vector3(0 - x, -1 + y, 0 + z);
                    vertices[21 + (blockCount * 24)] = new Vector3(0 - x, -1 + y, 1 + z);
                    vertices[22 + (blockCount * 24)] = new Vector3(1 - x, -1 + y, 1 + z);
                    vertices[23 + (blockCount * 24)] = new Vector3(1 - x, -1 + y, 0 + z);

                    #endregion

                    //Top faces (y+)
                    if (y + 1 < chunkHeight)
                    {
                        if (!isOpaqueBlock[x, y + 1, z])
                        {
                            triangles[0 + (blockCount * 36)] = 0 + (blockCount * 24);
                            triangles[1 + (blockCount * 36)] = 1 + (blockCount * 24);
                            triangles[2 + (blockCount * 36)] = 2 + (blockCount * 24);
                            triangles[3 + (blockCount * 36)] = 2 + (blockCount * 24);
                            triangles[4 + (blockCount * 36)] = 3 + (blockCount * 24);
                            triangles[5 + (blockCount * 36)] = 0 + (blockCount * 24);
                        }
                    } else
                    {
                        triangles[0 + (blockCount * 36)] = 0 + (blockCount * 24);
                        triangles[1 + (blockCount * 36)] = 1 + (blockCount * 24);
                        triangles[2 + (blockCount * 36)] = 2 + (blockCount * 24);
                        triangles[3 + (blockCount * 36)] = 2 + (blockCount * 24);
                        triangles[4 + (blockCount * 36)] = 3 + (blockCount * 24);
                        triangles[5 + (blockCount * 36)] = 0 + (blockCount * 24);
                    }

                    //Front faces (-z)
                    if (z - 1 > -1)
                    {
                        if (!isOpaqueBlock[x, y, z - 1])
                        {
                            triangles[6 + (blockCount * 36)] = 12 + (blockCount * 24);
                            triangles[7 + (blockCount * 36)] = 8 + (blockCount * 24);
                            triangles[8 + (blockCount * 36)] = 11 + (blockCount * 24);
                            triangles[9 + (blockCount * 36)] = 11 + (blockCount * 24);
                            triangles[10 + (blockCount * 36)] = 15 + (blockCount * 24);
                            triangles[11 + (blockCount * 36)] = 12 + (blockCount * 24);
                        }
                    }
                    else
                    {
                        triangles[6 + (blockCount * 36)] = 12 + (blockCount * 24);
                        triangles[7 + (blockCount * 36)] = 8 + (blockCount * 24);
                        triangles[8 + (blockCount * 36)] = 11 + (blockCount * 24);
                        triangles[9 + (blockCount * 36)] = 11 + (blockCount * 24);
                        triangles[10 + (blockCount * 36)] = 15 + (blockCount * 24);
                        triangles[11 + (blockCount * 36)] = 12 + (blockCount * 24);
                    }

                    //Left faces (-x)
                    if (x + 1 < chunkWidth)
                    {
                        if (!isOpaqueBlock[x + 1, y, z])
                        {
                            triangles[12 + (blockCount * 36)] = 21 + (blockCount * 24);
                            triangles[13 + (blockCount * 36)] = 17 + (blockCount * 24);
                            triangles[14 + (blockCount * 36)] = 16 + (blockCount * 24);
                            triangles[15 + (blockCount * 36)] = 16 + (blockCount * 24);
                            triangles[16 + (blockCount * 36)] = 20 + (blockCount * 24);
                            triangles[17 + (blockCount * 36)] = 21 + (blockCount * 24);
                        }
                    }
                    else
                    {
                        triangles[12 + (blockCount * 36)] = 21 + (blockCount * 24);
                        triangles[13 + (blockCount * 36)] = 17 + (blockCount * 24);
                        triangles[14 + (blockCount * 36)] = 16 + (blockCount * 24);
                        triangles[15 + (blockCount * 36)] = 16 + (blockCount * 24);
                        triangles[16 + (blockCount * 36)] = 20 + (blockCount * 24);
                        triangles[17 + (blockCount * 36)] = 21 + (blockCount * 24);
                    }

                    //Right faces (+x)
                    if (x - 1 > -1)
                    {
                        if (!isOpaqueBlock[x - 1, y, z])
                        {
                            triangles[18 + (blockCount * 36)] = 23 + (blockCount * 24);
                            triangles[19 + (blockCount * 36)] = 19 + (blockCount * 24);
                            triangles[20 + (blockCount * 36)] = 18 + (blockCount * 24);
                            triangles[21 + (blockCount * 36)] = 18 + (blockCount * 24);
                            triangles[22 + (blockCount * 36)] = 22 + (blockCount * 24);
                            triangles[23 + (blockCount * 36)] = 23 + (blockCount * 24);
                        }
                    }
                    else
                    {
                        triangles[18 + (blockCount * 36)] = 23 + (blockCount * 24);
                        triangles[19 + (blockCount * 36)] = 19 + (blockCount * 24);
                        triangles[20 + (blockCount * 36)] = 18 + (blockCount * 24);
                        triangles[21 + (blockCount * 36)] = 18 + (blockCount * 24);
                        triangles[22 + (blockCount * 36)] = 22 + (blockCount * 24);
                        triangles[23 + (blockCount * 36)] = 23 + (blockCount * 24);
                    }

                    //Back faces (+z)
                    if (z + 1 < chunkWidth)
                    {
                        if (!isOpaqueBlock[x, y, z + 1])
                        {
                            triangles[24 + (blockCount * 36)] = 14 + (blockCount * 24);
                            triangles[25 + (blockCount * 36)] = 10 + (blockCount * 24);
                            triangles[26 + (blockCount * 36)] = 9 + (blockCount * 24);
                            triangles[27 + (blockCount * 36)] = 9 + (blockCount * 24);
                            triangles[28 + (blockCount * 36)] = 13 + (blockCount * 24);
                            triangles[29 + (blockCount * 36)] = 14 + (blockCount * 24);
                        }
                    }
                    else
                    {
                        triangles[24 + (blockCount * 36)] = 14 + (blockCount * 24);
                        triangles[25 + (blockCount * 36)] = 10 + (blockCount * 24);
                        triangles[26 + (blockCount * 36)] = 9 + (blockCount * 24);
                        triangles[27 + (blockCount * 36)] = 9 + (blockCount * 24);
                        triangles[28 + (blockCount * 36)] = 13 + (blockCount * 24);
                        triangles[29 + (blockCount * 36)] = 14 + (blockCount * 24);
                    }

                    //Bottom faces (-y)
                    if (y - 1 > -1)
                    {
                        if (!isOpaqueBlock[x, y - 1, z])
                        {
                            triangles[30 + (blockCount * 36)] = 7 + (blockCount * 24);
                            triangles[31 + (blockCount * 36)] = 6 + (blockCount * 24);
                            triangles[32 + (blockCount * 36)] = 5 + (blockCount * 24);
                            triangles[33 + (blockCount * 36)] = 5 + (blockCount * 24);
                            triangles[34 + (blockCount * 36)] = 4 + (blockCount * 24);
                            triangles[35 + (blockCount * 36)] = 7 + (blockCount * 24);
                        }
                    } else
                    {
                        triangles[30 + (blockCount * 36)] = 7 + (blockCount * 24);
                        triangles[31 + (blockCount * 36)] = 6 + (blockCount * 24);
                        triangles[32 + (blockCount * 36)] = 5 + (blockCount * 24);
                        triangles[33 + (blockCount * 36)] = 5 + (blockCount * 24);
                        triangles[34 + (blockCount * 36)] = 4 + (blockCount * 24);
                        triangles[35 + (blockCount * 36)] = 7 + (blockCount * 24);
                    }

                    //Top face, pair 1
                    uvs[0 + (blockCount * 24)] = new Vector2(1, 1);
                    uvs[1 + (blockCount * 24)] = new Vector2(1, 0);
                    uvs[2 + (blockCount * 24)] = new Vector2(0, 0);
                    uvs[3 + (blockCount * 24)] = new Vector2(0, 1);

                    //Bottom face, pair 1
                    uvs[4 + (blockCount * 24)] = new Vector2(1, 0);
                    uvs[5 + (blockCount * 24)] = new Vector2(1, 1);
                    uvs[6 + (blockCount * 24)] = new Vector2(0, 1);
                    uvs[7 + (blockCount * 24)] = new Vector2(0, 0);

                    //Front face, pair 2
                    uvs[8 + (blockCount * 24)] = new Vector2(1, 0);
                    uvs[9 + (blockCount * 24)] = new Vector2(1, 1);
                    uvs[10 + (blockCount * 24)] = new Vector2(0, 1);
                    uvs[11 + (blockCount * 24)] = new Vector2(0, 0);

                    //Back face, pair 2
                    uvs[12 + (blockCount * 24)] = new Vector2(1, 1);
                    uvs[13 + (blockCount * 24)] = new Vector2(1, 0);
                    uvs[14 + (blockCount * 24)] = new Vector2(0, 0);
                    uvs[15 + (blockCount * 24)] = new Vector2(0, 1);

                    //Left face, pair 3
                    uvs[16 + (blockCount * 24)] = new Vector2(1, 1);
                    uvs[17 + (blockCount * 24)] = new Vector2(0, 1);
                    uvs[18 + (blockCount * 24)] = new Vector2(1, 1);
                    uvs[19 + (blockCount * 24)] = new Vector2(0, 1);

                    //Right face, pair 3
                    uvs[20 + (blockCount * 24)] = new Vector2(1, 0);
                    uvs[21 + (blockCount * 24)] = new Vector2(0, 0);
                    uvs[22 + (blockCount * 24)] = new Vector2(1, 0);
                    uvs[23 + (blockCount * 24)] = new Vector2(0, 0);

                    blockCount += 1;

                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        _meshFilter.mesh = mesh;

        if (debug)
        {
            foreach (Vector3 vertex in vertices)
            {
                Instantiate(Vertex, vertex, Quaternion.identity);
            }
        }

    }

}
