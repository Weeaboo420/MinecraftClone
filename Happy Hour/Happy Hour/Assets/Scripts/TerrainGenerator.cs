using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TerrainGenerator : MonoBehaviour
{
    //Holds every block in this chunk, each block is represented by a byte which in turn
    //represents a block id, 0 is air, 1 is dirt and so on...
    private byte[,,] voxelData;


    private bool isLoaded = false;
    private MeshFilter _meshFilter;    
    private int chunkWidth, chunkHeight;
    private World world;
    private float artifactOffset;
    private List<TerrainGenerator> neighbors;

    void Awake()
    {        
        _meshFilter = GetComponent<MeshFilter>();        

        //Get the settings from WorldSettings
        chunkWidth = WorldSettings.ChunkWidth;
        chunkHeight = WorldSettings.ChunkHeight;

        artifactOffset = VoxelData.ArtifactOffset;
        neighbors = new List<TerrainGenerator>();        
    }

    //Used by the world script to assign itself as the world instance of every chunk,
    //skips having to find the "World" game object every time we need something from it.
    public void SetWorld(World newWorld)
    {
        world = newWorld;
    }

    //We don't want to modify voxelData when accessing it from other scripts,
    //so we only return it
    public byte[,,] Data
    {
        get
        {
            return voxelData;
        }
    }

    public Vector3 Position
    {
        get
        {
            return transform.position;
        }
    }

    public bool Loaded
    {
        get
        {
            return isLoaded;
        }
    }

    private void CheckMesh()
    {
        if(this.gameObject.TryGetComponent(out MeshCollider mc))
        {            
            Destroy(GetComponent<MeshCollider>());
        }
    }

    private byte GetBlockId(int blockIndex)
    {                
        int count = 0;
        byte block = 0;
        for(int y = 0; y < voxelData.GetLength(1); y++)
        {
            for(int x = 0; x < voxelData.GetLength(0); x++)
            {
                for(int z = 0; z < voxelData.GetLength(2); z++)
                {                    
                    if(count == blockIndex)
                    {
                        block = VoxelData.GetVoxel(voxelData[x, y, z]).Id;
                    }
                    count += 1;
                }
            }
        }        
        return block;
    }

    private Voxel GetBlock(byte id)
    {
        return VoxelData.GetVoxel(id);
    }

    private List<TerrainGenerator> GetNeighbors()
    {
        List<TerrainGenerator> neighbors = new List<TerrainGenerator>();
        foreach(TerrainGenerator chunk in world.Chunks)
        {
            if(chunk.Position.x == transform.position.x + WorldSettings.ChunkWidth && chunk.Position.z == transform.position.z  || chunk.Position.x == transform.position.x - WorldSettings.ChunkWidth && chunk.Position.z == transform.position.z || chunk.Position.z == transform.position.z + WorldSettings.ChunkWidth && chunk.Position.x == transform.position.x || chunk.Position.z == transform.position.z - WorldSettings.ChunkWidth && chunk.Position.x == transform.position.x)
            {
                if(chunk != this)
                {
                    neighbors.Add(chunk);
                }
            }
        }

        return neighbors;

    }

    //Checks every neighboring chunk for an air block at a certain position,
    //localOffset represents where the neighboring chunk is in relation to
    //this chunk
    private bool BlockInNeighbor(Vector3 pos, Vector3 localOffset)
    {
        int code = 0;
        bool result = false;

        if(neighbors.Count > 0)
        {
            if(neighbors.Any(neighbor => neighbor.Position == localOffset))
            {
                TerrainGenerator neighbor = neighbors.Find(n => n.Position == localOffset);
                if(neighbor.Data != null)
                {
                    if(neighbor.Data[(int)pos.x, (int)pos.y, (int)pos.z] == 0)
                    {
                        result = true;                        
                    }
                } 
                
                else 
                {
                    result = false;
                    code = 1;
                }
            } 
            
            else 
            {                
                result = false;
                code = 2;
                Debug.Log("neighbors: " + neighbors.Count + ", code: " + code);
                Debug.Log("my pos: " + transform.position);
                Debug.Log("localOffset: " + localOffset);
                foreach(TerrainGenerator n in neighbors)
                {
                    Debug.Log("neighbor: " + n.Position);
                }
                Debug.Log("");
            }
        }

        return result;
    }

    //Coordinates labeled with "1" refer to the block we're looking at,
    //the ones labeled with "2" refer to the current block that we're trying to
    //determine whether or not to draw a face for.
    private bool CanDraw(int x1, int y1, int z1, int x2, int y2, int z2)
    {
        if(x1 > -1 && x1 < chunkWidth && y1 > -1 && y1 < chunkHeight && z1 > -1 && z1 < chunkWidth)
        {
            if(!GetBlock(voxelData[x2, y2, z2]).Opaque && GetBlock(voxelData[x1, y1, z1]).Id 
            != GetBlock(voxelData[x2, y2, z2]).Id && !GetBlock(voxelData[x1, y1, z1]).Opaque || 
            !GetBlock(voxelData[x1, y1, z1]).Opaque && GetBlock(voxelData[x2, y2, z2]).Opaque)
            {
                return true;
            }
        }

        //Check against neighboring chunks
        else 
        {
            //NEED TO ADD SUPPORT FOR VERTICAL CHUNKS

            //Right faces
            if(x1 == WorldSettings.ChunkWidth)
            {                
                return BlockInNeighbor(new Vector3(0, y2, z2), new Vector3(transform.position.x + WorldSettings.ChunkWidth, 0, transform.position.z));
            }

            //Left faces
            if(x1 == -1)
            {                
                return BlockInNeighbor(new Vector3(WorldSettings.ChunkWidth - 1, y2, z2), new Vector3(transform.position.x - WorldSettings.ChunkWidth, 0, transform.position.z));
            }

            //Front faces
            if(z1 == -1)
            {                
                return BlockInNeighbor(new Vector3(x2, y2, WorldSettings.ChunkWidth - 1), new Vector3(transform.position.x, 0, transform.position.z - WorldSettings.ChunkWidth));
            }

            //Back faces
            if(z1 == WorldSettings.ChunkWidth)
            {                
                return BlockInNeighbor(new Vector3(x2, y2, 0), new Vector3(transform.position.x, 0, transform.position.z + WorldSettings.ChunkWidth));
            }

        }

        return false;

    }

    public void SetBlock(int x, int y, int z, VoxelData.VoxelNames block)
    {
        if(x >= 0 && x < voxelData.GetLength(0) && y >= 0 && y < voxelData.GetLength(1) && z >= 0 && z < voxelData.GetLength(2))
        {                        
            //Update the block we're currently looking at
            CheckMesh();
            voxelData[x, y, z] = (byte)VoxelData.GetVoxel(block).Id;
            GenerateMesh();            

        }
    }

    public void SetData()
    {
        Array names = Enum.GetValues(typeof(VoxelData.VoxelNames));
        System.Random rand = new System.Random();
        voxelData = null;

        if(voxelData == null)
        {
            //First pass, for voxel data
            voxelData = new byte[chunkWidth, chunkHeight, chunkWidth];            
            for (int y = 0; y < chunkHeight; y++)
            {
                for (int x = 0; x < chunkWidth; x++)
                {
                    for (int z = 0; z < chunkWidth; z++)
                    {

                        //Get some actual noise value so that we can build some interesting-looking terrain                           
                        int groundHeight = world.SampleNoise(x, z, transform.position);

                        if(y > groundHeight)
                        {
                            voxelData[x, y, z] = VoxelData.GetVoxel(VoxelData.VoxelNames.Air).Id;
                        }

                        else
                        {                            
                            
                            //Highest position in chunk, aka grass
                            if(y == groundHeight)
                            {
                                voxelData[x, y, z] = VoxelData.GetVoxel(VoxelData.VoxelNames.Grass).Id;                                
                            }

                            else if(y <= groundHeight - 1 && y >= groundHeight - 3)
                            {
                                voxelData[x, y, z] = VoxelData.GetVoxel(VoxelData.VoxelNames.Dirt).Id;
                            }

                            else
                            {
                                if(UnityEngine.Random.Range(0f, 1f) <= 0.935f)
                                {
                                    voxelData[x, y, z] = VoxelData.GetVoxel(VoxelData.VoxelNames.Stone).Id;
                                } else {
                                    voxelData[x, y, z] = VoxelData.GetVoxel(VoxelData.VoxelNames.Coal).Id;
                                }
                            }

                        }

                    }
                }
            }

        }

        isLoaded = true;

    }

    //Converts a position to a block index in the chunk
    private int ConvertToBlockIndex(int x, int y, int z)
    {
        int index = 0;
        //Coordinates have a "c" as suffix because we want to distinguish between the values
        //in the loops and the input arguments to this function.
        for(int yc = 0; yc < chunkHeight; yc ++)
        {
            for(int xc = 0; xc < chunkWidth; xc ++)
            {
                for(int zc = 0; zc < chunkWidth; zc ++)
                {
                    if(xc == x && yc == y && zc == z)
                    {                        
                        return index;                        
                    }

                    index += 1;

                }
            }
        }

        return index;

    }

    public void GenerateMesh()
    {
        if(isLoaded)
        {            
            int blockCount = 0;
            Vector3[] vertices = new Vector3[8 * 3 * chunkWidth * chunkWidth * chunkHeight];
            int[] triangles = new int[36 * chunkWidth * chunkWidth * chunkHeight];
            Vector2[] uvs = new Vector2[8 * 3 * chunkWidth * chunkWidth * chunkHeight];
            neighbors = GetNeighbors();

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
                        
                        if(voxelData[x, y, z] != 0)
                        {

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
                            if (CanDraw(x, y, z - 1, x, y, z))
                            {
                                triangles[6 + (blockCount * 36)] = 12 + (blockCount * 24);
                                triangles[7 + (blockCount * 36)] = 8 + (blockCount * 24);
                                triangles[8 + (blockCount * 36)] = 11 + (blockCount * 24);
                                triangles[9 + (blockCount * 36)] = 11 + (blockCount * 24);
                                triangles[10 + (blockCount * 36)] = 15 + (blockCount * 24);
                                triangles[11 + (blockCount * 36)] = 12 + (blockCount * 24);

                            }
                            

                            //Left faces (-x)
                            if (CanDraw(x - 1, y, z, x, y, z))
                            {
                                triangles[12 + (blockCount * 36)] = 21 + (blockCount * 24);
                                triangles[13 + (blockCount * 36)] = 17 + (blockCount * 24);
                                triangles[14 + (blockCount * 36)] = 16 + (blockCount * 24);
                                triangles[15 + (blockCount * 36)] = 16 + (blockCount * 24);
                                triangles[16 + (blockCount * 36)] = 20 + (blockCount * 24);
                                triangles[17 + (blockCount * 36)] = 21 + (blockCount * 24);
                            }


                            //Right faces (+x)
                            if (CanDraw(x + 1, y, z, x, y, z))
                            {
                                triangles[18 + (blockCount * 36)] = 23 + (blockCount * 24);
                                triangles[19 + (blockCount * 36)] = 19 + (blockCount * 24);
                                triangles[20 + (blockCount * 36)] = 18 + (blockCount * 24);
                                triangles[21 + (blockCount * 36)] = 18 + (blockCount * 24);
                                triangles[22 + (blockCount * 36)] = 22 + (blockCount * 24);
                                triangles[23 + (blockCount * 36)] = 23 + (blockCount * 24);
                            }

                            //Back faces (+z)
                            if (CanDraw(x, y, z + 1, x, y, z))
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
                            uvs[8 + (blockCount * 24)] = new Vector2(1, 1);
                            uvs[9 + (blockCount * 24)] = new Vector2(0, 1);
                            uvs[10 + (blockCount * 24)] = new Vector2(1, 1);
                            uvs[11 + (blockCount * 24)] = new Vector2(0, 1);

                            //Back face, pair 2
                            uvs[12 + (blockCount * 24)] = new Vector2(1, 0);
                            uvs[13 + (blockCount * 24)] = new Vector2(0, 0);
                            uvs[14 + (blockCount * 24)] = new Vector2(1, 0);
                            uvs[15 + (blockCount * 24)] = new Vector2(0, 0);

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

            int currentBlock = 0;
            for(int i = 0; i < uvs.Length; i++)
            {   
                if(i % 24 == 0)
                {
                    byte blockId = GetBlockId(currentBlock / 24);
                    Voxel block = VoxelData.GetVoxel(blockId);

                    //This tells us which face we are setting the texture for,
                    //Top, bottom, front, back, left, right
                    //0,   1,      2,     3,    4,    5
                    int blockFace = 0;                

                    if(blockId > 0)
                    {
                        for(int j = currentBlock; j < currentBlock + 24; j++)
                        {
                            uvs[j] = Utilities.GetUvCoordinates(block, blockFace, uvs[j].x, uvs[j].y);

                            //Block face handling
                            if((j + 1) % 4 == 0 && blockFace < 5)
                            {
                                blockFace += 1;                        
                            }                           

                        }
                    }

                    currentBlock += 24;

                }       

            }

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;        
            mesh.uv = uvs;
            
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();
            
            _meshFilter.mesh = mesh; 
            this.gameObject.AddComponent<MeshCollider>();
            neighbors.Clear();

        }

    }

}
