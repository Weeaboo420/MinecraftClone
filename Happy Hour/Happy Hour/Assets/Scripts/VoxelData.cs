﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Voxel
{
    private VoxelData.VoxelNames _name;    
    private bool _opaque;
    private byte _id;

    public Voxel(VoxelData.VoxelNames name, bool opaque, byte id)
    {
        _name = name;
        _opaque = opaque;
        _id = id;
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
        Kiwi = 7
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
        new Voxel(VoxelNames.Air, false, 0),
        new Voxel(VoxelNames.Dirt, true, 1),
        new Voxel(VoxelNames.Grass, true, 2),
        new Voxel(VoxelNames.Stone, true, 3),
        new Voxel(VoxelNames.Glass, false, 4),
        new Voxel(VoxelNames.Coal, true, 5),
        new Voxel(VoxelNames.Cage, false, 6),
        new Voxel(VoxelNames.Kiwi, true, 7)
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
