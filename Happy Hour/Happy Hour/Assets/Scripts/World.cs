﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class is intended to store some parameters for the world, such as
//chunk width and height, possibly name in the future, and so on...
public static class WorldSettings
{
    private static int _chunkWidth = 8, _chunkHeight = 10;

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
    private int worldWidth = 8;

    //Perlin noise settings and such
    private float noiseScale = 100f;            
    private int seed;
    private List<TerrainGenerator> chunks;

    public float NoiseScale
    {
        get
        {
            return noiseScale;
        }
    }

    public float NoiseWidth
    {
        get
        {
            return 10f;
        }
    }

    //This function is responsible for delivering height values for the terrain generator
    public int SampleNoise(float x, float y, Vector3 chunkPosition)
    {
        return Mathf.FloorToInt(Mathf.PerlinNoise(0.1f * (x + chunkPosition.x), 0.1f * (y + (chunkPosition.z)) + seed) * WorldSettings.ChunkHeight);
    }

    private void CreateChunk(Vector3 pos)
    {        
        GameObject chunk = new GameObject("Chunk at " + pos.ToString());
        chunk.transform.position = pos;
        chunk.transform.rotation = Quaternion.Euler(0, 0, 0);
        chunk.AddComponent<TerrainGenerator>();
        chunk.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Atlas");
        chunk.GetComponent<TerrainGenerator>().SetWorld(this);
        chunks.Add(chunk.GetComponent<TerrainGenerator>());
    }

    public void CreateSeed()
    {
        seed = Random.Range(0, 256);
    }

    private void GenerateWorld()
    {
        for(int x = 0; x < worldWidth; x++)
        {
            for(int z = 0; z < worldWidth; z++)
            {
                CreateChunk(new Vector3(x * WorldSettings.ChunkWidth, 0, z * WorldSettings.ChunkWidth));
            }            
        } 
    }

    public void RegenerateWorld()
    {
        //Create a new seed and tell every chunk to regenerate
        CreateSeed();
        foreach(TerrainGenerator chunk in chunks)
        {            
            chunk.SetData();
            chunk.GenerateMesh();            
        }
    }

    void Start()
    {
        //Initialize the list of chunks
        chunks = new List<TerrainGenerator>();

        //Seed initilization, noise generation and world generation
        CreateSeed();
        GenerateWorld();
    }

}
