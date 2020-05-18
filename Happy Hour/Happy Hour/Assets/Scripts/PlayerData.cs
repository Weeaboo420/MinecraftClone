using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerData : MonoBehaviour
{

    private int selectedBlock = 0;
    private Text blockText;
    
    public int Block
    {
        get 
        {
            return selectedBlock;
        }
    }

    void Start()
    {
        blockText = GameObject.Find("CurrentBlock").GetComponent<Text>();
        SelectedBlockChanged();
    }

    private void SelectedBlockChanged()
    {
        VoxelData.VoxelNames blockName = (VoxelData.VoxelNames)selectedBlock;
        blockText.text = blockName.ToString();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            if(selectedBlock == VoxelData.Voxels.Count - 1)
            {
                selectedBlock = 0;
            } else {
                selectedBlock += 1;
            }

            SelectedBlockChanged();

        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            if(selectedBlock == 0)
            {
                selectedBlock = VoxelData.Voxels.Count - 1;
            } else {
                selectedBlock -= 1;
            }

            SelectedBlockChanged();

        }

    }
}
