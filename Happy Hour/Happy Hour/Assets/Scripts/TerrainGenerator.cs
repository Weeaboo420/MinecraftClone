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
    private Voxel[,,] voxelData;
    private int chunkWidth = 16, chunkHeight = 10;
    private float artifactOffset = 0.0005f;
    private PlayerData playerData;

    public GameObject VertexPrefab;
    private GameObject vertexObj;
    void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
        playerData = GameObject.Find("PlayerData").GetComponent<PlayerData>();
        Generate();
    }

    private void CheckMesh()
    {
        Destroy(GetComponent<MeshCollider>());
    }

    private int GetBlock(int blockIndex)
    {                
        int count = 0;
        int block = 0;
        for(int y = 0; y < voxelData.GetLength(1); y++)
        {
            for(int x = 0; x < voxelData.GetLength(0); x++)
            {
                for(int z = 0; z < voxelData.GetLength(2); z++)
                {                    
                    if(count == blockIndex)
                    {
                        block = voxelData[x, y, z].Id;
                    }
                    count += 1;
                }
            }
        }        
        return block;
    }

    //Coordinates labeled with "1" refer to the block we're looking at,
    //the ones labeled with "2" refer to the current block that we're trying to
    //determine whether or not to draw a face for.
    private bool CanDraw(int x1, int y1, int z1, int x2, int y2, int z2)
    {
        if(!voxelData[x2, y2, z2].Opaque && voxelData[x1, y1, z1].Id != voxelData[x2, y2, z2].Id && !voxelData[x1, y1, z1].Opaque || !voxelData[x1, y1, z1].Opaque && voxelData[x2, y2, z2].Opaque)
        {            
            return true;
        }

        return false;
    }

    void Update()
    {

        if(Input.GetMouseButtonDown(0))
        {

            float distance = 50f;
            Camera cam = GameObject.Find("Main Camera").GetComponent<Camera>();
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //DEBUG FEATURE
            //Casts a ray from the mouse position, if it hits anything then it will either instantiate
            //a new vertex prefab or move an existing one
            if(Physics.Raycast(ray, out hit, distance))
            {
                if(vertexObj == null)
                {   
                    GameObject go;
                    go = (GameObject)Instantiate(VertexPrefab, hit.point, Quaternion.identity);
                    vertexObj = go;

                } else {
                    vertexObj.transform.position = hit.point;
                }

                //Grab a reference to the chunk that was clicked,
                //calculate the relative position inside the chunk that
                //was clicked, then round that position into integers
                GameObject chunk = hit.transform.gameObject;
                Vector3 posInChunk = hit.point - chunk.transform.position + new Vector3(0, 0.5f, 0);

                posInChunk.x = Mathf.FloorToInt(posInChunk.x);
                posInChunk.y = Mathf.FloorToInt(posInChunk.y);
                posInChunk.z = Mathf.FloorToInt(posInChunk.z);
                chunk.GetComponent<TerrainGenerator>().SetBlock((int) posInChunk.x, (int) posInChunk.y, (int) posInChunk.z, (VoxelData.VoxelNames)playerData.Block);

            }
            
        }
    }

    public void SetBlock(int x, int y, int z, VoxelData.VoxelNames block)
    {
        if(x >= 0 && x < voxelData.GetLength(0) && y >= 0 && y < voxelData.GetLength(1) && z >= 0 && z < voxelData.GetLength(2))
        {
            CheckMesh();            
            voxelData[x, y, z] = VoxelData.GetVoxel(block);
            Generate();
        }
    }

    private void Generate()
    {
        int blockCount = 0;
        Vector3[] vertices = new Vector3[8 * 3 * chunkWidth * chunkWidth * chunkHeight];
        int[] triangles = new int[36 * chunkWidth * chunkWidth * chunkHeight];
        Vector2[] uvs = new Vector2[8 * 3 * chunkWidth * chunkWidth * chunkHeight];
         
        if(voxelData == null)
        {
            //First pass, for voxel data
            voxelData = new Voxel[chunkWidth, chunkHeight, chunkWidth];
            for (int y = 0; y < chunkHeight; y++)
            {
                for (int x = 0; x < chunkWidth; x++)
                {
                    for (int z = 0; z < chunkWidth; z++)
                    {
                        
                        if(y <= 5 && y > 0)
                        {
                            if(Random.Range(0f, 1f) <= 0.935f)
                            {
                                voxelData[x, y, z] = VoxelData.GetVoxel(VoxelData.VoxelNames.Stone);
                            } else {
                                voxelData[x, y, z] = VoxelData.GetVoxel(VoxelData.VoxelNames.Coal);
                            }
                        }

                        if(y == 0)
                        {
                            voxelData[x, y, z] = VoxelData.GetVoxel(VoxelData.VoxelNames.Cage);
                        }

                        if(y < chunkHeight-1 && y > 5)
                        {
                            voxelData[x, y, z] = VoxelData.GetVoxel(VoxelData.VoxelNames.Dirt);
                        } 
                        
                        if(y == chunkHeight - 1)
                        {
                            voxelData[x, y, z] = VoxelData.GetVoxel(VoxelData.VoxelNames.Grass);
                        }

                    }
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
                    vertices[0 + (blockCount * 24)] = new Vector3(0 + x, 0 + y, 0 + z);
                    vertices[1 + (blockCount * 24)] = new Vector3(0 + x, 0 + y, 1 + z);
                    vertices[2 + (blockCount * 24)] = new Vector3(1 + x, 0 + y, 1 + z);
                    vertices[3 + (blockCount * 24)] = new Vector3(1 + x, 0 + y, 0 + z);

                    vertices[4 + (blockCount * 24)] = new Vector3(0 + x, -1 + y, 0 + z);
                    vertices[5 + (blockCount * 24)] = new Vector3(0 + x, -1 + y, 1 + z);
                    vertices[6 + (blockCount * 24)] = new Vector3(1 + x, -1 + y, 1 + z);
                    vertices[7 + (blockCount * 24)] = new Vector3(1 + x, -1 + y, 0 + z);

                    //Second (z)
                    vertices[8 + (blockCount * 24)] = new Vector3(0 + x, 0 + y, 0 + z);
                    vertices[9 + (blockCount * 24)] = new Vector3(0 + x, 0 + y, 1 + z);
                    vertices[10 + (blockCount * 24)] = new Vector3(1 + x, 0 + y, 1 + z);
                    vertices[11 + (blockCount * 24)] = new Vector3(1 + x, 0 + y, 0 + z);

                    vertices[12 + (blockCount * 24)] = new Vector3(0 + x, -1 + y, 0 + z);
                    vertices[13 + (blockCount * 24)] = new Vector3(0 + x, -1 + y, 1 + z);
                    vertices[14 + (blockCount * 24)] = new Vector3(1 + x, -1 + y, 1 + z);
                    vertices[15 + (blockCount * 24)] = new Vector3(1 + x, -1 + y, 0 + z);

                    //Third (x)
                    vertices[16 + (blockCount * 24)] = new Vector3(0 + x, 0 + y, 0 + z);
                    vertices[17 + (blockCount * 24)] = new Vector3(0 + x, 0 + y, 1 + z);
                    vertices[18 + (blockCount * 24)] = new Vector3(1 + x, 0 + y, 1 + z);
                    vertices[19 + (blockCount * 24)] = new Vector3(1 + x, 0 + y, 0 + z);

                    vertices[20 + (blockCount * 24)] = new Vector3(0 + x, -1 + y, 0 + z);
                    vertices[21 + (blockCount * 24)] = new Vector3(0 + x, -1 + y, 1 + z);
                    vertices[22 + (blockCount * 24)] = new Vector3(1 + x, -1 + y, 1 + z);
                    vertices[23 + (blockCount * 24)] = new Vector3(1 + x, -1 + y, 0 + z);

                    #endregion

                    if (voxelData[x, y, z].Id != 0)
                    {

                        //Top faces (y+)
                        if (y + 1 < chunkHeight)
                        {
                            if (CanDraw(x, y + 1, z, x, y, z))
                            {
                                triangles[0 + (blockCount * 36)] = 0 + (blockCount * 24);
                                triangles[1 + (blockCount * 36)] = 1 + (blockCount * 24);
                                triangles[2 + (blockCount * 36)] = 2 + (blockCount * 24);
                                triangles[3 + (blockCount * 36)] = 2 + (blockCount * 24);
                                triangles[4 + (blockCount * 36)] = 3 + (blockCount * 24);
                                triangles[5 + (blockCount * 36)] = 0 + (blockCount * 24);
                            }
                        }
                        else
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
                            if (CanDraw(x, y, z - 1, x, y, z))
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
                        if (x - 1 > -1)
                        {
                            if (CanDraw(x - 1, y, z, x, y, z))
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
                        if (x + 1 < chunkWidth)
                        {
                            if (CanDraw(x + 1, y, z, x, y, z))
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
                            if (CanDraw(x, y, z + 1, x, y, z))
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
                            if (CanDraw(x, y - 1, z, x, y, z))
                            {
                                triangles[30 + (blockCount * 36)] = 7 + (blockCount * 24);
                                triangles[31 + (blockCount * 36)] = 6 + (blockCount * 24);
                                triangles[32 + (blockCount * 36)] = 5 + (blockCount * 24);
                                triangles[33 + (blockCount * 36)] = 5 + (blockCount * 24);
                                triangles[34 + (blockCount * 36)] = 4 + (blockCount * 24);
                                triangles[35 + (blockCount * 36)] = 7 + (blockCount * 24);
                            }
                        }
                        else
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

                    }

                    blockCount += 1;

                }
            }
        }    

        //Scale the texture so that one subtexture fits exactly one side of a block,
        //set the appropriate texture according to the block id
        for(int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(uvs[i].x * 0.1f, uvs[i].y * 0.1f);
        }

        int point = 0;
        for(int i = 0; i < uvs.Length; i++)
        {   
            if(i % 24 == 0)
            {
                int blockId = GetBlock(point / 24);
                if(blockId > 0)
                {
                    for(int j = point; j < point + 24; j++)
                    {
                        if(uvs[j].x == 0.1f)
                        {
                            uvs[j].x = (blockId * 0.1f) - artifactOffset;

                            if(uvs[j].x < 0f)
                            {
                                uvs[j].x = 0f;
                            }

                            if(uvs[j].x > 1f)
                            {
                                uvs[j].x = 1f;
                            }                            

                        }

                        if(uvs[j].x == 0f)
                        {
                            uvs[j].x = (blockId * 0.1f) - 0.1f + artifactOffset;

                            if(uvs[j].x < 0f)
                            {
                                uvs[j].x = 0f;
                            }

                            if(uvs[j].x > 1f)
                            {
                                uvs[j].x = 1f;
                            }                            
                        }

                            if(uvs[j].y == 0f)
                        {
                            uvs[j].y = uvs[j].y + artifactOffset;
                        }

                        if(uvs[j].y == 0.1f)
                        {
                            uvs[j].y = uvs[j].y - artifactOffset;
                        }        

                    }
                }

                point += 24;

            }       

        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;        
        mesh.uv = uvs;
        
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        _meshFilter.mesh = mesh;
        this.gameObject.AddComponent<MeshCollider>();

    }

}
