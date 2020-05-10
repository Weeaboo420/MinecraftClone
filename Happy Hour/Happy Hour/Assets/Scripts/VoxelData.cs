﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Voxel
{
    private VoxelData.VoxelNames _name;    
    private bool _opaque;    

    public Voxel(VoxelData.VoxelNames name, bool opaque)
    {
        _name = name;
        _opaque = opaque;
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
        Dirt = 1
    }

    private static List<Voxel> _voxels = new List<Voxel>()
    {
        new Voxel(VoxelNames.Air, false),
        new Voxel(VoxelNames.Dirt, true)
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