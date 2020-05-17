using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Voxel
{
    private VoxelData.VoxelNames _name;    
    private bool _opaque;
    private int _id;

    public Voxel(VoxelData.VoxelNames name, bool opaque, int id)
    {
        _name = name;
        _opaque = opaque;
        _id = id;
    }

    public int Id
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

public static class VoxelData
{
    public enum VoxelNames 
    { 
        Air = 0,
        Dirt = 1,
        Grass = 2,
        Stone = 3,
        Glass = 4
    }

    private static List<Voxel> _voxels = new List<Voxel>()
    {
        new Voxel(VoxelNames.Air, false, 0),
        new Voxel(VoxelNames.Dirt, true, 1),
        new Voxel(VoxelNames.Grass, true, 2),
        new Voxel(VoxelNames.Stone, true, 3),
        new Voxel(VoxelNames.Glass, false, 4)
    };

    public static Voxel GetVoxel(VoxelNames name)
    {
        if (_voxels.Any(voxel => voxel.Name == name))
        {
            return _voxels.Find(voxel => voxel.Name == name);
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
