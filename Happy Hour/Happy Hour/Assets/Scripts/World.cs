using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//This class is intended to store some parameters for the world, such as
//chunk width and height, possibly name in the future, and so on...
public static class WorldSettings
{
    private static int _chunkWidth = 8, _chunkHeight = 8;

    public static int ChunkWidth
    {
        get 
        {
            return _chunkWidth;
        }
    }

    public static int ChunkHeight
    {
        get 
        {
            return _chunkHeight;
        }
    }

}

//This class is mainly responsible for spawning in new chunks
public class World : MonoBehaviour
{

    //Specifies how many chunks wide the world will be, this number is then squared since the width is
    //used in both the x- and z-directions
    private int worldWidth = 5;

    private void CreateChunk(Vector3 pos)
    {        
        GameObject chunk = new GameObject("Chunk at " + pos.ToString());
        chunk.transform.position = pos;
        chunk.transform.rotation = Quaternion.Euler(0, 0, 0);
        chunk.AddComponent<TerrainGenerator>();
        chunk.GetComponent<MeshRenderer>().material = (Material) AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Atlas.mat");
    }

    void Start()
    {
        for(int x = 0; x < worldWidth; x++)
        {
            for(int z = 0; z < worldWidth; z++)
            {
                CreateChunk(new Vector3(x * WorldSettings.ChunkWidth, 0, z * WorldSettings.ChunkWidth));
            }            
        }        
    }

}
