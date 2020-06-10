using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class Voxel
{
    private VoxelData.VoxelNames _name;    
    private bool _opaque;
    private byte _id;
    private int[] _faces;

    public Voxel(VoxelData.VoxelNames name, bool opaque, byte id, int[] faces)
    {
        _name = name;
        _opaque = opaque;
        _id = id;
        _faces = faces;
    }

    public int[] Faces
    {
        get
        {
            return _faces;
        }
    }

    public byte Id
    {
        get 
        {
            return _id;
        }
    }

    public VoxelData.VoxelNames Name
    {
        get
        {
            return _name;
        }
    }

    public bool Opaque
    {
        get
        {
            return _opaque;
        }
    }

}

public static class Utilities 
{
    //uvs[j].x = (block.Faces[blockFace] * 0.1f) - artifactOffset;
    //uvs[j].x = (block.Faces[blockFace] * 0.1f) - 0.1f + artifactOffset;

    /*public static void ClearConsole()
    {
         var assembly = Assembly.GetAssembly(typeof(SceneView));
         var type = assembly.GetType("UnityEditor.LogEntries");
         var method = type.GetMethod("Clear");
         method.Invoke(new object(), null);
    }*/

    private static float GetRow(Voxel block, int face)
    {
        float rowValue = 0f;        

        for(int cell = 1; cell < block.Faces[face] + 1; cell++)
        {
            if(cell % 11 == 0)
            {                
                rowValue += 0.1f;
            }
        }

        return rowValue;        
    }

    public static string FormatKey(Vector3 pos)
    {
        pos.x = Mathf.Clamp(pos.x, 0, WorldSettings.ChunkWidth-1);
        pos.y = Mathf.Clamp(pos.y, 0, WorldSettings.ChunkHeight-1);
        pos.z = Mathf.Clamp(pos.z, 0, WorldSettings.ChunkWidth-1);

        return pos.x + "" + pos.y + "" + pos.z;
    }

    //Returns a chunk if it exists
    public static TerrainGenerator FindChunk(Vector3 pos)
    {
        if(GameObject.Find("Chunk at " + pos.ToString()))
        {
            return GameObject.Find("Chunk at " + pos.ToString()).GetComponent<TerrainGenerator>();
        }

        return null;

    }

    public static Vector2 GetUvCoordinates(Voxel block, int face, float x, float y)
    {
        Vector2 coords = new Vector2(0, 0);

        if(x == 0.1f)
        {
            coords.x = (block.Faces[face] * 0.1f) - VoxelData.ArtifactOffset - GetRow(block, face) * 10;
        }

        else if (x == 0f)
        {
            coords.x = (block.Faces[face] * 0.1f) - 0.1f + VoxelData.ArtifactOffset - GetRow(block, face) * 10;
        }

        if(y == 0.1f)
        {
            coords.y = y + GetRow(block, face) - VoxelData.ArtifactOffset; 
        }

        else if(y == 0f)
        {
            coords.y = y + GetRow(block, face) + VoxelData.ArtifactOffset;
        }



        //Clamping
        if(coords.x < 0f)
        {
            coords.x = 0f;
        }

        if(coords.x > 1f)
        {
            coords.x = 1f;
        }

        if(coords.y < 0f)
        {
            coords.y = 0f;
        }

        if(coords.y > 1f)
        {
            coords.y = 1f;
        }

        return coords;
    }

}

public static class VoxelData
{
    public enum VoxelNames 
    { 
        Air = 0,
        Dirt = 1,
        Grass = 2,
        Stone = 3,
        Glass = 4,
        Coal = 5,
        Cage = 6,
        Kiwi = 7,
        Log = 8,
        Planks = 9,
        Cobblestone = 10,
        Sand = 11
    }

    public static float ArtifactOffset
    {
        get
        {
            return 0.0005f;

        }
    }

    private static List<Voxel> _voxels = new List<Voxel>()
    {
        //Currently there is a limitation in that the two faces on an axis must have
        //the same texture index, otherwise the texture doesn't apply correctly and it
        //just looks horrible

        //Faces order = top, bottom, front, back, left, right
        new Voxel(VoxelNames.Air, false, 0, new int[] {0, 0, 0, 0, 0, 0}),
        new Voxel(VoxelNames.Dirt, true, 1, new int[] {1, 1, 1, 1, 1, 1}),
        new Voxel(VoxelNames.Grass, true, 2, new int[] {2, 1, 8, 8, 8, 8}),
        new Voxel(VoxelNames.Stone, true, 3, new int[] {3, 3, 3, 3, 3, 3}),
        new Voxel(VoxelNames.Glass, false, 4, new int[] {4, 4, 4, 4, 4, 4}),
        new Voxel(VoxelNames.Coal, true, 5, new int[] {5, 5, 5, 5, 5, 5}),
        new Voxel(VoxelNames.Cage, false, 6, new int[] {6, 6, 6, 6, 6, 6}),
        new Voxel(VoxelNames.Kiwi, true, 7, new int[] {7, 7, 7, 7, 7, 7}),
        new Voxel(VoxelNames.Log, true, 8, new int[] {10, 10, 9, 9, 9, 9}),
        new Voxel(VoxelNames.Planks, true, 9, new int[] {11, 11, 11, 11, 11, 11}),
        new Voxel(VoxelNames.Cobblestone, true, 10, new int[] {12, 12, 12, 12, 12, 12}),
        new Voxel(VoxelNames.Sand, true, 11, new int [] {13, 13, 13, 13, 13, 13})
    };

    public static Voxel GetVoxel(VoxelNames name)
    {
        if (_voxels.Any(voxel => voxel.Name == name))
        {
            return _voxels.Find(voxel => voxel.Name == name);
        }

        return null;
    }

    public static Voxel GetVoxel(byte id)
    {
        if (_voxels.Any(voxel => voxel.Id == (int)id))
        {
            return _voxels.Find(voxel => voxel.Id == (int)id);
        }

        return null;
    }

    public static List<Voxel> Voxels
    {
        get
        {
            return _voxels;
        }
    }

}
