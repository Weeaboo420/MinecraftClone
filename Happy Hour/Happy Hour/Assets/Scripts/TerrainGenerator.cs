using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Block
{
    private byte _blockID;
    private int _blockIndex;
    public Block(byte blockID, int blockIndex)
    {
        _blockID = blockID;
        _blockIndex = blockIndex;
    }

    public byte Id
    {
        get
        {
            return _blockID;
        }

        set
        {
            _blockID = value;
        }
    }

    public int Index
    {
        get
        {
            return _blockIndex;
        }

        set
        {
            _blockIndex = value;
        }
    }

}

[System.Serializable]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TerrainGenerator : MonoBehaviour
{
    //Holds every block in this chunk, each block is represented by a byte which in turn
    //represents a block id, 0 is air, 1 is dirt and so on...
    public Dictionary<string, Block> voxelData;

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
    public Dictionary<string, Block> Data
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


    //Allows for updating the triangles of a singular block, increasing
    //performance drastically by not having to loop through every block
    //in the chunk when updating just one block.
    public void SetBlockMesh(int x, int y, int z, bool ignoreNeighbors = false, bool draw = false, bool onlyOwnFaces = false)
    {

        int blockIndex = voxelData[Utilities.FormatKey(new Vector3(x, y, z))].Index;

        //Calculate block indexes of surrounding blocks
        int blockIndex_above = voxelData[Utilities.FormatKey(new Vector3(x, y + 1, z))].Index;
        int blockIndex_below = voxelData[Utilities.FormatKey(new Vector3(x, y - 1, z))].Index;

        int blockIndex_left = voxelData[Utilities.FormatKey(new Vector3(x - 1, y, z))].Index;
        int blockIndex_right = voxelData[Utilities.FormatKey(new Vector3(x + 1, y, z))].Index;

        int blockIndex_front = voxelData[Utilities.FormatKey(new Vector3(x, y, z - 1))].Index;
        int blockIndex_back = voxelData[Utilities.FormatKey(new Vector3(x, y, z + 1))].Index;

        CheckMesh();
        Vector3[] vertices = _meshFilter.mesh.vertices;
        int[] triangles = _meshFilter.mesh.triangles;
        Vector2[] uvs = _meshFilter.mesh.uv;

#region own faces
        //Top faces (y+)
        if (CanDraw(x, y + 1, z, x, y, z, false, false, draw))
        {
            triangles[0 + (blockIndex * 36)] = 0 + (blockIndex * 24);
            triangles[1 + (blockIndex * 36)] = 1 + (blockIndex * 24);
            triangles[2 + (blockIndex * 36)] = 2 + (blockIndex * 24);
            triangles[3 + (blockIndex * 36)] = 2 + (blockIndex * 24);
            triangles[4 + (blockIndex * 36)] = 3 + (blockIndex * 24);
            triangles[5 + (blockIndex * 36)] = 0 + (blockIndex * 24);            

        } else {
            triangles[0 + (blockIndex * 36)] = 0;
            triangles[1 + (blockIndex * 36)] = 0;
            triangles[2 + (blockIndex * 36)] = 0;
            triangles[3 + (blockIndex * 36)] = 0;
            triangles[4 + (blockIndex * 36)] = 0;
            triangles[5 + (blockIndex * 36)] = 0;

        }

        //Update the block face below us as well
        if(CanDraw(x, y, z, x, y - 1, z, false, false, draw) && !onlyOwnFaces)
        {
            triangles[0 + (blockIndex_below * 36)] = 0 + (blockIndex_below * 24);
            triangles[1 + (blockIndex_below * 36)] = 1 + (blockIndex_below * 24);
            triangles[2 + (blockIndex_below * 36)] = 2 + (blockIndex_below * 24);
            triangles[3 + (blockIndex_below * 36)] = 2 + (blockIndex_below * 24);
            triangles[4 + (blockIndex_below * 36)] = 3 + (blockIndex_below * 24);
            triangles[5 + (blockIndex_below * 36)] = 0 + (blockIndex_below * 24);
        } else {
            triangles[0 + (blockIndex_below * 36)] = 0;
            triangles[1 + (blockIndex_below * 36)] = 0;
            triangles[2 + (blockIndex_below * 36)] = 0;
            triangles[3 + (blockIndex_below * 36)] = 0;
            triangles[4 + (blockIndex_below * 36)] = 0;
            triangles[5 + (blockIndex_below * 36)] = 0;
        }



        //Front faces (-z)
        if (CanDraw(x, y, z - 1, x, y, z, false, false, draw))
        {
            triangles[6 + (blockIndex * 36)] = 12 + (blockIndex * 24);
            triangles[7 + (blockIndex * 36)] = 8 + (blockIndex * 24);
            triangles[8 + (blockIndex * 36)] = 11 + (blockIndex * 24);
            triangles[9 + (blockIndex * 36)] = 11 + (blockIndex * 24);
            triangles[10 + (blockIndex * 36)] = 15 + (blockIndex * 24);
            triangles[11 + (blockIndex * 36)] = 12 + (blockIndex * 24);

        } else {
            triangles[6 + (blockIndex * 36)] = 0;
            triangles[7 + (blockIndex * 36)] = 0;
            triangles[8 + (blockIndex * 36)] = 0;
            triangles[9 + (blockIndex * 36)] = 0;
            triangles[10 + (blockIndex * 36)] = 0;
            triangles[11 + (blockIndex * 36)] = 0;

        }
        
        
        //Update the block face in front as well
        if(CanDraw(x, y, z, x, y, z + 1, false, false, draw) && !onlyOwnFaces)
        {
            triangles[6 + (blockIndex_back * 36)] = 12 + (blockIndex_back * 24);
            triangles[7 + (blockIndex_back * 36)] = 8 + (blockIndex_back * 24);
            triangles[8 + (blockIndex_back * 36)] = 11 + (blockIndex_back * 24);
            triangles[9 + (blockIndex_back * 36)] = 11 + (blockIndex_back * 24);
            triangles[10 + (blockIndex_back * 36)] = 15 + (blockIndex_back * 24);
            triangles[11 + (blockIndex_back * 36)] = 12 + (blockIndex_back * 24);
        } else {
            triangles[6 + (blockIndex_back * 36)] = 0;
            triangles[7 + (blockIndex_back * 36)] = 0;
            triangles[8 + (blockIndex_back * 36)] = 0;
            triangles[9 + (blockIndex_back * 36)] = 0;
            triangles[10 + (blockIndex_back * 36)] = 0;
            triangles[11 + (blockIndex_back * 36)] = 0;
        }

        //Left faces (-x)
        if (CanDraw(x - 1, y, z, x, y, z, false, false, draw))
        {            
            triangles[12 + (blockIndex * 36)] = 21 + (blockIndex * 24);
            triangles[13 + (blockIndex * 36)] = 17 + (blockIndex * 24);
            triangles[14 + (blockIndex * 36)] = 16 + (blockIndex * 24);
            triangles[15 + (blockIndex * 36)] = 16 + (blockIndex * 24);
            triangles[16 + (blockIndex * 36)] = 20 + (blockIndex * 24);
            triangles[17 + (blockIndex * 36)] = 21 + (blockIndex * 24);

        } else {
            triangles[12 + (blockIndex * 36)] = 0;
            triangles[13 + (blockIndex * 36)] = 0;
            triangles[14 + (blockIndex * 36)] = 0;
            triangles[15 + (blockIndex * 36)] = 0;
            triangles[16 + (blockIndex * 36)] = 0;
            triangles[17 + (blockIndex * 36)] = 0;
        }

        if(CanDraw(x, y, z, x + 1, y, z, false, false, draw) && !onlyOwnFaces)
        {            
            triangles[12 + (blockIndex_right * 36)] = 21 + (blockIndex_right * 24);
            triangles[13 + (blockIndex_right * 36)] = 17 + (blockIndex_right * 24);
            triangles[14 + (blockIndex_right * 36)] = 16 + (blockIndex_right * 24);
            triangles[15 + (blockIndex_right * 36)] = 16 + (blockIndex_right * 24);
            triangles[16 + (blockIndex_right * 36)] = 20 + (blockIndex_right * 24);
            triangles[17 + (blockIndex_right * 36)] = 21 + (blockIndex_right * 24);
        } else {            
            triangles[12 + (blockIndex_right * 36)] = 0;
            triangles[13 + (blockIndex_right * 36)] = 0;
            triangles[14 + (blockIndex_right * 36)] = 0;
            triangles[15 + (blockIndex_right * 36)] = 0;
            triangles[16 + (blockIndex_right * 36)] = 0;
            triangles[17 + (blockIndex_right * 36)] = 0;
        }

        //Right faces (+x)
        if (CanDraw(x + 1, y, z, x, y, z, false, false, draw))
        {
            triangles[18 + (blockIndex * 36)] = 23 + (blockIndex * 24);
            triangles[19 + (blockIndex * 36)] = 19 + (blockIndex * 24);
            triangles[20 + (blockIndex * 36)] = 18 + (blockIndex * 24);
            triangles[21 + (blockIndex * 36)] = 18 + (blockIndex * 24);
            triangles[22 + (blockIndex * 36)] = 22 + (blockIndex * 24);
            triangles[23 + (blockIndex * 36)] = 23 + (blockIndex * 24);
        } else {
            triangles[18 + (blockIndex * 36)] = 0;
            triangles[19 + (blockIndex * 36)] = 0;
            triangles[20 + (blockIndex * 36)] = 0;
            triangles[21 + (blockIndex * 36)] = 0;
            triangles[22 + (blockIndex * 36)] = 0;
            triangles[23 + (blockIndex * 36)] = 0;

        }

        if(CanDraw(x, y, z, x - 1, y, z, false, false, draw) && !onlyOwnFaces)
        {
            triangles[18 + (blockIndex_left * 36)] = 23 + (blockIndex_left * 24);
            triangles[19 + (blockIndex_left * 36)] = 19 + (blockIndex_left * 24);
            triangles[20 + (blockIndex_left * 36)] = 18 + (blockIndex_left * 24);
            triangles[21 + (blockIndex_left * 36)] = 18 + (blockIndex_left * 24);
            triangles[22 + (blockIndex_left * 36)] = 22 + (blockIndex_left * 24);
            triangles[23 + (blockIndex_left * 36)] = 23 + (blockIndex_left * 24);
        } else {
            triangles[18 + (blockIndex_left * 36)] = 0;
            triangles[19 + (blockIndex_left * 36)] = 0;
            triangles[20 + (blockIndex_left * 36)] = 0;
            triangles[21 + (blockIndex_left * 36)] = 0;
            triangles[22 + (blockIndex_left * 36)] = 0;
            triangles[23 + (blockIndex_left * 36)] = 0;
        }

        //Back faces (+z)
        if (CanDraw(x, y, z + 1, x, y, z, false, false, draw))
        {
            triangles[24 + (blockIndex * 36)] = 14 + (blockIndex * 24);
            triangles[25 + (blockIndex * 36)] = 10 + (blockIndex * 24);
            triangles[26 + (blockIndex * 36)] = 9 + (blockIndex * 24);
            triangles[27 + (blockIndex * 36)] = 9 + (blockIndex * 24);
            triangles[28 + (blockIndex * 36)] = 13 + (blockIndex * 24);
            triangles[29 + (blockIndex * 36)] = 14 + (blockIndex * 24);
        } else {
            triangles[24 + (blockIndex * 36)] = 0;
            triangles[25 + (blockIndex * 36)] = 0;
            triangles[26 + (blockIndex * 36)] = 0;
            triangles[27 + (blockIndex * 36)] = 0;
            triangles[28 + (blockIndex * 36)] = 0;
            triangles[29 + (blockIndex * 36)] = 0;

        }

        if(CanDraw(x, y, z, x, y, z - 1, false, false, draw) && !onlyOwnFaces)
        {
            triangles[24 + (blockIndex_front * 36)] = 14 + (blockIndex_front * 24);
            triangles[25 + (blockIndex_front * 36)] = 10 + (blockIndex_front * 24);
            triangles[26 + (blockIndex_front * 36)] = 9 + (blockIndex_front * 24);
            triangles[27 + (blockIndex_front * 36)] = 9 + (blockIndex_front * 24);
            triangles[28 + (blockIndex_front * 36)] = 13 + (blockIndex_front * 24);
            triangles[29 + (blockIndex_front * 36)] = 14 + (blockIndex_front * 24);
        } else {
            triangles[24 + (blockIndex_front * 36)] = 0;
            triangles[25 + (blockIndex_front * 36)] = 0;
            triangles[26 + (blockIndex_front * 36)] = 0;
            triangles[27 + (blockIndex_front * 36)] = 0;
            triangles[28 + (blockIndex_front * 36)] = 0;
            triangles[29 + (blockIndex_front * 36)] = 0;
        }
        
        //Bottom faces (-y)
        if (CanDraw(x, y - 1, z, x, y, z, false, false, draw))
        {
            triangles[30 + (blockIndex * 36)] = 7 + (blockIndex * 24);
            triangles[31 + (blockIndex * 36)] = 6 + (blockIndex * 24);
            triangles[32 + (blockIndex * 36)] = 5 + (blockIndex * 24);
            triangles[33 + (blockIndex * 36)] = 5 + (blockIndex * 24);
            triangles[34 + (blockIndex * 36)] = 4 + (blockIndex * 24);
            triangles[35 + (blockIndex * 36)] = 7 + (blockIndex * 24);
        } else {
            triangles[30 + (blockIndex * 36)] = 0;
            triangles[31 + (blockIndex * 36)] = 0;
            triangles[32 + (blockIndex * 36)] = 0;
            triangles[33 + (blockIndex * 36)] = 0;
            triangles[34 + (blockIndex * 36)] = 0;
            triangles[35 + (blockIndex * 36)] = 0;

        }

        if(CanDraw(x, y, z, x, y + 1, z, false, false, draw) && !onlyOwnFaces)
        {
            triangles[30 + (blockIndex_above * 36)] = 7 + (blockIndex_above * 24);
            triangles[31 + (blockIndex_above * 36)] = 6 + (blockIndex_above * 24);
            triangles[32 + (blockIndex_above * 36)] = 5 + (blockIndex_above * 24);
            triangles[33 + (blockIndex_above * 36)] = 5 + (blockIndex_above * 24);
            triangles[34 + (blockIndex_above * 36)] = 4 + (blockIndex_above * 24);
            triangles[35 + (blockIndex_above * 36)] = 7 + (blockIndex_above * 24);
        } else {
            triangles[30 + (blockIndex_above * 36)] = 0;
            triangles[31 + (blockIndex_above * 36)] = 0;
            triangles[32 + (blockIndex_above * 36)] = 0;
            triangles[33 + (blockIndex_above * 36)] = 0;
            triangles[34 + (blockIndex_above * 36)] = 0;
            triangles[35 + (blockIndex_above * 36)] = 0;
        }

#endregion


        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;        
        mesh.uv = uvs;
        
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();

        _meshFilter.mesh = mesh;
                    
        this.gameObject.AddComponent<MeshCollider>();

    }

    private byte GetBlockId(int blockIndex)
    {                
        int count = 0;
        byte block = 0;
        for(int y = 0; y < WorldSettings.ChunkHeight; y++)
        {
            for(int x = 0; x < WorldSettings.ChunkWidth; x++)
            {
                for(int z = 0; z < WorldSettings.ChunkWidth; z++)
                {                    
                    if(count == blockIndex)
                    {
                        block = voxelData[Utilities.FormatKey(new Vector3(x, y, z))].Id;
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
    //this chunk. This function is semantically incorrect, we return true
    //if we can draw a specified face even though we should be returning
    //false since the function is called "BlockInNeighbor" and we are actually
    //returning the opposite if there is a block there.
    private bool BlockInNeighbor(Vector3 pos, Vector3 localOffset)
    {        

        if(neighbors.Count > 0)
        {
            if(neighbors.Any(neighbor => neighbor.Position == localOffset))
            {                
                TerrainGenerator neighbor = neighbors.Find(n => n.Position == localOffset);
                if(neighbor.Data != null)
                {
                    //If the block in the neighbor is an air block, then we can draw
                    if(neighbor.Data[Utilities.FormatKey(pos)].Id == 0)
                    {                        
                        return true;
                    }
                } 
                
                //If, for some reason, the voxel data in the neighbor is null, then we
                //just draw
                else 
                {
                    return true;
                }

            } 

            //If there is not a suitable neighbor then we can just draw
            else
            {
                return true;
            }
            

        }

        //If all else fails, then don't draw anything
        return false;

    }

    //Coordinates labeled with "1" refer to the block we're looking at,
    //the ones labeled with "2" refer to the current block that we're trying to
    //determine whether or not to draw a face for.

    //Directions
    //0 = -x, 1 = +x
    //2 = -z, 3 = +z
    //4 = -y, 5 = +y
    //10 = none
    private bool CanDraw(int x1, int y1, int z1, int x2, int y2, int z2, bool ignoreNeighbors = false, bool debug = false, bool draw = false)
    {

        if(x1 > -1 && x1 < chunkWidth && y1 > -1 && y1 < chunkHeight && z1 > -1 && z1 < chunkWidth)
        {
            if(!GetBlock(voxelData[Utilities.FormatKey( new Vector3(x2, y2, z2))].Id).Opaque && voxelData[Utilities.FormatKey( new Vector3(x1, y1, z1))].Id 
            != voxelData[Utilities.FormatKey( new Vector3(x2, y2, z2))].Id && !GetBlock(voxelData[Utilities.FormatKey( new Vector3(x1, y1, z1))].Id).Opaque || 
            !GetBlock(voxelData[Utilities.FormatKey( new Vector3(x1, y1, z1))].Id).Opaque && GetBlock(voxelData[Utilities.FormatKey( new Vector3(x2, y2, z2))].Id).Opaque)
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

    //Used to modify a single block inside voxelData, this function is called
    //from PlayerData.cs when the player clicks on this chunk
    public void SetBlock(int x, int y, int z, VoxelData.VoxelNames block)
    {
        if(x >= 0 && x < WorldSettings.ChunkWidth && y >= 0 && y < WorldSettings.ChunkHeight && z >= 0 && z < WorldSettings.ChunkWidth)
        {                        
            //Update the block we're currently looking at
            CheckMesh();

            voxelData[Utilities.FormatKey(new Vector3(x, y, z))].Id = (byte)VoxelData.GetVoxel(block).Id;

            //Only update neighboring chunks if we're at the edge of our own chunk
            /*if(x == WorldSettings.ChunkWidth - 1 || x == 0 || z == WorldSettings.ChunkWidth - 1 || z == 0)
            {
                List<TerrainGenerator> neighbors = GetNeighbors();
                //Update neighboring chunks
                if(neighbors.Count > 0)
                {
                    foreach(TerrainGenerator neighbor in neighbors)
                    {
                        neighbor.GenerateMesh();
                    }
                }
            }*/
            //GenerateMesh();
        }
    }

    //Voxel data array population
    public void SetData()
    {
        Array names = Enum.GetValues(typeof(VoxelData.VoxelNames));
        System.Random rand = new System.Random();
        voxelData = null;

        if(voxelData == null)
        {
            //First pass, for voxel data
            //voxelData = new byte[chunkWidth, chunkHeight, chunkWidth];
            voxelData = new Dictionary<string, Block>();
            int index = 0;
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
                            voxelData[Utilities.FormatKey(new Vector3(x, y, z))] = new Block(VoxelData.GetVoxel(VoxelData.VoxelNames.Air).Id, index);
                        }

                        else
                        {                            
                            
                            //Highest position in chunk, aka grass
                            if(y == groundHeight)
                            {
                                if(groundHeight >= 4)
                                {
                                    voxelData[Utilities.FormatKey(new Vector3(x, y, z))] = new Block(VoxelData.GetVoxel(VoxelData.VoxelNames.Grass).Id, index);
                                } 
                                
                                else if(groundHeight < 4 && groundHeight > 2)
                                {
                                    voxelData[Utilities.FormatKey(new Vector3(x, y, z))] = new Block(VoxelData.GetVoxel(VoxelData.VoxelNames.Sand).Id, index);
                                }

                                else {
                                    voxelData[Utilities.FormatKey(new Vector3(x, y, z))] = new Block(VoxelData.GetVoxel(VoxelData.VoxelNames.Stone).Id, index);
                                }

                            }

                            if(y <= groundHeight - 1 && y >= groundHeight - 3)
                            {
                                voxelData[Utilities.FormatKey(new Vector3(x, y, z))] = new Block(VoxelData.GetVoxel(VoxelData.VoxelNames.Dirt).Id, index);
                            }

                            if(y < groundHeight - 3)
                            {
                                if(UnityEngine.Random.Range(0f, 1f) <= 0.935f)
                                {
                                    voxelData[Utilities.FormatKey(new Vector3(x, y, z))] = new Block(VoxelData.GetVoxel(VoxelData.VoxelNames.Stone).Id, index);
                                } else {
                                    voxelData[Utilities.FormatKey(new Vector3(x, y, z))] = new Block(VoxelData.GetVoxel(VoxelData.VoxelNames.Coal).Id, index);
                                }
                            }

                        }

                        index += 1;

                    }
                }
            }

        }

        isLoaded = true;

    }

    //Converts a position to a block index in the chunk
    public int ConvertToBlockIndex(int x, int y, int z)
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
            int blockIndex = 0;
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
                        
                        //First (y)
                        vertices[0 + (blockIndex * 24)] = new Vector3(0 + x, 0 + y, 0 + z);
                        vertices[1 + (blockIndex * 24)] = new Vector3(0 + x, 0 + y, 1 + z);
                        vertices[2 + (blockIndex * 24)] = new Vector3(1 + x, 0 + y, 1 + z);
                        vertices[3 + (blockIndex * 24)] = new Vector3(1 + x, 0 + y, 0 + z);

                        vertices[4 + (blockIndex * 24)] = new Vector3(0 + x, -1 + y, 0 + z);
                        vertices[5 + (blockIndex * 24)] = new Vector3(0 + x, -1 + y, 1 + z);
                        vertices[6 + (blockIndex * 24)] = new Vector3(1 + x, -1 + y, 1 + z);
                        vertices[7 + (blockIndex * 24)] = new Vector3(1 + x, -1 + y, 0 + z);

                        //Second (z)
                        vertices[8 + (blockIndex * 24)] = new Vector3(0 + x, 0 + y, 0 + z);
                        vertices[9 + (blockIndex * 24)] = new Vector3(0 + x, 0 + y, 1 + z);
                        vertices[10 + (blockIndex * 24)] = new Vector3(1 + x, 0 + y, 1 + z);
                        vertices[11 + (blockIndex * 24)] = new Vector3(1 + x, 0 + y, 0 + z);

                        vertices[12 + (blockIndex * 24)] = new Vector3(0 + x, -1 + y, 0 + z);
                        vertices[13 + (blockIndex * 24)] = new Vector3(0 + x, -1 + y, 1 + z);
                        vertices[14 + (blockIndex * 24)] = new Vector3(1 + x, -1 + y, 1 + z);
                        vertices[15 + (blockIndex * 24)] = new Vector3(1 + x, -1 + y, 0 + z);

                        //Third (x)
                        vertices[16 + (blockIndex * 24)] = new Vector3(0 + x, 0 + y, 0 + z);
                        vertices[17 + (blockIndex * 24)] = new Vector3(0 + x, 0 + y, 1 + z);
                        vertices[18 + (blockIndex * 24)] = new Vector3(1 + x, 0 + y, 1 + z);
                        vertices[19 + (blockIndex * 24)] = new Vector3(1 + x, 0 + y, 0 + z);

                        vertices[20 + (blockIndex * 24)] = new Vector3(0 + x, -1 + y, 0 + z);
                        vertices[21 + (blockIndex * 24)] = new Vector3(0 + x, -1 + y, 1 + z);
                        vertices[22 + (blockIndex * 24)] = new Vector3(1 + x, -1 + y, 1 + z);
                        vertices[23 + (blockIndex * 24)] = new Vector3(1 + x, -1 + y, 0 + z);

                        if(voxelData[Utilities.FormatKey(new Vector3(x, y, z))].Id != 0)
                        {
                            //Top faces (y+)
                            if (y + 1 < chunkHeight)
                            {
                                if (CanDraw(x, y + 1, z, x, y, z))
                                {
                                    triangles[0 + (blockIndex * 36)] = 0 + (blockIndex * 24);
                                    triangles[1 + (blockIndex * 36)] = 1 + (blockIndex * 24);
                                    triangles[2 + (blockIndex * 36)] = 2 + (blockIndex * 24);
                                    triangles[3 + (blockIndex * 36)] = 2 + (blockIndex * 24);
                                    triangles[4 + (blockIndex * 36)] = 3 + (blockIndex * 24);
                                    triangles[5 + (blockIndex * 36)] = 0 + (blockIndex * 24);
                                }
                            }
                            else
                            {
                                triangles[0 + (blockIndex * 36)] = 0 + (blockIndex * 24);
                                triangles[1 + (blockIndex * 36)] = 1 + (blockIndex * 24);
                                triangles[2 + (blockIndex * 36)] = 2 + (blockIndex * 24);
                                triangles[3 + (blockIndex * 36)] = 2 + (blockIndex * 24);
                                triangles[4 + (blockIndex * 36)] = 3 + (blockIndex * 24);
                                triangles[5 + (blockIndex * 36)] = 0 + (blockIndex * 24);
                            }

                            //Front faces (-z)
                            if (CanDraw(x, y, z - 1, x, y, z))
                            {
                                triangles[6 + (blockIndex * 36)] = 12 + (blockIndex * 24);
                                triangles[7 + (blockIndex * 36)] = 8 + (blockIndex * 24);
                                triangles[8 + (blockIndex * 36)] = 11 + (blockIndex * 24);
                                triangles[9 + (blockIndex * 36)] = 11 + (blockIndex * 24);
                                triangles[10 + (blockIndex * 36)] = 15 + (blockIndex * 24);
                                triangles[11 + (blockIndex * 36)] = 12 + (blockIndex * 24);

                            }
                            

                            //Left faces (-x)
                            if (CanDraw(x - 1, y, z, x, y, z))
                            {
                                triangles[12 + (blockIndex * 36)] = 21 + (blockIndex * 24);
                                triangles[13 + (blockIndex * 36)] = 17 + (blockIndex * 24);
                                triangles[14 + (blockIndex * 36)] = 16 + (blockIndex * 24);
                                triangles[15 + (blockIndex * 36)] = 16 + (blockIndex * 24);
                                triangles[16 + (blockIndex * 36)] = 20 + (blockIndex * 24);
                                triangles[17 + (blockIndex * 36)] = 21 + (blockIndex * 24);
                            }


                            //Right faces (+x)
                            if (CanDraw(x + 1, y, z, x, y, z))
                            {
                                triangles[18 + (blockIndex * 36)] = 23 + (blockIndex * 24);
                                triangles[19 + (blockIndex * 36)] = 19 + (blockIndex * 24);
                                triangles[20 + (blockIndex * 36)] = 18 + (blockIndex * 24);
                                triangles[21 + (blockIndex * 36)] = 18 + (blockIndex * 24);
                                triangles[22 + (blockIndex * 36)] = 22 + (blockIndex * 24);
                                triangles[23 + (blockIndex * 36)] = 23 + (blockIndex * 24);
                            }

                            //Back faces (+z)
                            if (CanDraw(x, y, z + 1, x, y, z))
                            {
                                triangles[24 + (blockIndex * 36)] = 14 + (blockIndex * 24);
                                triangles[25 + (blockIndex * 36)] = 10 + (blockIndex * 24);
                                triangles[26 + (blockIndex * 36)] = 9 + (blockIndex * 24);
                                triangles[27 + (blockIndex * 36)] = 9 + (blockIndex * 24);
                                triangles[28 + (blockIndex * 36)] = 13 + (blockIndex * 24);
                                triangles[29 + (blockIndex * 36)] = 14 + (blockIndex * 24);
                            }
                            
                            //Bottom faces (-y)
                            if (y - 1 > -1)
                            {
                                if (CanDraw(x, y - 1, z, x, y, z))
                                {
                                    triangles[30 + (blockIndex * 36)] = 7 + (blockIndex * 24);
                                    triangles[31 + (blockIndex * 36)] = 6 + (blockIndex * 24);
                                    triangles[32 + (blockIndex * 36)] = 5 + (blockIndex * 24);
                                    triangles[33 + (blockIndex * 36)] = 5 + (blockIndex * 24);
                                    triangles[34 + (blockIndex * 36)] = 4 + (blockIndex * 24);
                                    triangles[35 + (blockIndex * 36)] = 7 + (blockIndex * 24);
                                }
                            }
                            else
                            {
                                triangles[30 + (blockIndex * 36)] = 7 + (blockIndex * 24);
                                triangles[31 + (blockIndex * 36)] = 6 + (blockIndex * 24);
                                triangles[32 + (blockIndex * 36)] = 5 + (blockIndex * 24);
                                triangles[33 + (blockIndex * 36)] = 5 + (blockIndex * 24);
                                triangles[34 + (blockIndex * 36)] = 4 + (blockIndex * 24);
                                triangles[35 + (blockIndex * 36)] = 7 + (blockIndex * 24);
                            }                        

                        }

                        //Top face, pair 1
                        uvs[0 + (blockIndex * 24)] = new Vector2(1, 1);
                        uvs[1 + (blockIndex * 24)] = new Vector2(1, 0);
                        uvs[2 + (blockIndex * 24)] = new Vector2(0, 0);
                        uvs[3 + (blockIndex * 24)] = new Vector2(0, 1);

                        //Bottom face, pair 1
                        uvs[4 + (blockIndex * 24)] = new Vector2(1, 0);
                        uvs[5 + (blockIndex * 24)] = new Vector2(1, 1);
                        uvs[6 + (blockIndex * 24)] = new Vector2(0, 1);
                        uvs[7 + (blockIndex * 24)] = new Vector2(0, 0);

                        //Front face, pair 2
                        uvs[8 + (blockIndex * 24)] = new Vector2(1, 1);
                        uvs[9 + (blockIndex * 24)] = new Vector2(0, 1);
                        uvs[10 + (blockIndex * 24)] = new Vector2(1, 1);
                        uvs[11 + (blockIndex * 24)] = new Vector2(0, 1);

                        //Back face, pair 2
                        uvs[12 + (blockIndex * 24)] = new Vector2(1, 0);
                        uvs[13 + (blockIndex * 24)] = new Vector2(0, 0);
                        uvs[14 + (blockIndex * 24)] = new Vector2(1, 0);
                        uvs[15 + (blockIndex * 24)] = new Vector2(0, 0);

                        //Left face, pair 3
                        uvs[16 + (blockIndex * 24)] = new Vector2(1, 1);
                        uvs[17 + (blockIndex * 24)] = new Vector2(0, 1);
                        uvs[18 + (blockIndex * 24)] = new Vector2(1, 1);
                        uvs[19 + (blockIndex * 24)] = new Vector2(0, 1);

                        //Right face, pair 3
                        uvs[20 + (blockIndex * 24)] = new Vector2(1, 0);
                        uvs[21 + (blockIndex * 24)] = new Vector2(0, 0);
                        uvs[22 + (blockIndex * 24)] = new Vector2(1, 0);
                        uvs[23 + (blockIndex * 24)] = new Vector2(0, 0);

                        blockIndex += 1;

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

                    for(int j = currentBlock; j < currentBlock + 24; j++)
                    {
                        uvs[j] = Utilities.GetUvCoordinates(block, blockFace, uvs[j].x, uvs[j].y);

                        //Block face handling
                        if((j + 1) % 4 == 0 && blockFace < 5)
                        {
                            blockFace += 1;                        
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
