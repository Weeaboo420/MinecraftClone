using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TerrainGenerator : MonoBehaviour
{
    private MeshFilter _meshFilter;
    private Voxel[,,] voxelData;
    private int chunkWidth = 16, chunkHeight = 10;
    public int textureAtlasWidth, singleTextureWidth;
    private Texture2D atlas;


    public GameObject VertexPrefab;
    private GameObject _vertexObj;
    public float xOffset, yOffset;
    private float xScale = 0.1f, yScale = 0.1f;

    void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
        Generate();        
        atlas = (Texture2D)AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/atlas.png");
        textureAtlasWidth = atlas.width;
        singleTextureWidth = atlas.width/10;
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
        Debug.Log("Returned block: " + block);
        return block;
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
                if(_vertexObj == null)
                {   
                    GameObject go;
                    go = (GameObject)Instantiate(VertexPrefab, hit.point, Quaternion.identity);
                    _vertexObj = go;

                } else {
                    _vertexObj.transform.position = hit.point;
                }

                //Grab a reference to the chunk that was clicked,
                //calculate the relative position inside the chunk that
                //was clicked, then round that position into integers
                GameObject chunk = hit.transform.gameObject;
                Vector3 posInChunk = hit.point - chunk.transform.position + new Vector3(0, 0.5f, 0);

                posInChunk.x = Mathf.FloorToInt(posInChunk.x);
                posInChunk.y = Mathf.FloorToInt(posInChunk.y);
                posInChunk.z = Mathf.FloorToInt(posInChunk.z);
                chunk.GetComponent<TerrainGenerator>().SetBlock((int) posInChunk.x, (int) posInChunk.y, (int) posInChunk.z, VoxelData.VoxelNames.Air);

            }
            
        }
    }

    public void SetBlock(int x, int y, int z, VoxelData.VoxelNames block)
    {
        if(x >= 0 && x < voxelData.GetLength(0) && y >= 0 && y < voxelData.GetLength(1) && z >= 0 && z < voxelData.GetLength(2))
        {
            CheckMesh();
            Voxel[,,] newVoxels = voxelData;
            newVoxels[x, y, z] = VoxelData.GetVoxel(block);
            Generate(newVoxels);
        }
    }

    private void Generate(Voxel[,,] voxels = null)
    {
        int blockCount = 0;
        Vector3[] vertices = new Vector3[8 * 3 * chunkWidth * chunkWidth * chunkHeight];
        int[] triangles = new int[36 * chunkWidth * chunkWidth * chunkHeight];
        Vector2[] uvs = new Vector2[8 * 3 * chunkWidth * chunkWidth * chunkHeight];
        Voxel[,,] _voxels = new Voxel[chunkWidth, chunkHeight, chunkWidth];

        if(voxels != null)
        {
            _voxels = voxels;
        } 
        
        else 
        {
            //First pass, for voxel data
            for (int y = 0; y < chunkHeight; y++)
            {
                for (int x = 0; x < chunkWidth; x++)
                {
                    for (int z = 0; z < chunkWidth; z++)
                    {
                        if(y != chunkHeight-1)
                        {
                            _voxels[x, y, z] = VoxelData.GetVoxel(VoxelData.VoxelNames.Dirt);
                        } else {
                            _voxels[x, y, z] = VoxelData.GetVoxel(VoxelData.VoxelNames.Grass);
                        }
                    }
                }
            }


        }

        voxelData = _voxels;        
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

                    if (_voxels[x, y, z].Opaque)
                    {

                        //Top faces (y+)
                        if (y + 1 < chunkHeight)
                        {
                            if (!_voxels[x, y + 1, z].Opaque)
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
                            if (!_voxels[x, y, z - 1].Opaque)
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
                            if (!_voxels[x - 1, y, z].Opaque)
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
                            if (!_voxels[x + 1, y, z].Opaque)
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
                            if (!_voxels[x, y, z + 1].Opaque)
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
                            if (!_voxels[x, y - 1, z].Opaque)
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
                            uvs[j].x = (blockId * 0.1f);

                            if(uvs[j].x < 0f)
                            {
                                uvs[j].x = 0f;
                            }

                            if(uvs[j].x > 1f)
                            {
                                uvs[j].x = 1f;
                            }

                            //Debug.Log((blockId * 0.1f) + " (0.1x), " + blockId);

                        }

                        if(uvs[j].x == 0f)
                        {
                            uvs[j].x = (blockId * 0.1f) - 0.1f;

                            if(uvs[j].x < 0f)
                            {
                                uvs[j].x = 0f;
                            }

                            if(uvs[j].x > 1f)
                            {
                                uvs[j].x = 1f;
                            }

                            //Debug.Log((blockId * 0.1f) + " (0x), " + blockId);
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
