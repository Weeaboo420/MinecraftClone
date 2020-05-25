using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerData : MonoBehaviour
{

    private int selectedBlock = 0;
    private Text blockText;
    public GameObject uiBlock;

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

        if(blockName == VoxelData.VoxelNames.Air)
        {
            foreach(Transform child in uiBlock.transform)
            {
                if(child.transform.name != "Point Light")
                {
                    child.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", null);
                }
            }
        } 
        
        else 
        {
            foreach(Transform child in uiBlock.transform)
            {
                if(child.transform.name != "Point Light")
                {
                    child.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", Resources.Load<Texture2D>("GUI/atlas"));

                    if(child.transform.name == "TopFace")
                    {
                        child.GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", new Vector2((VoxelData.GetVoxel((byte)selectedBlock).Faces[0] * 0.1f - 0.1f) - VoxelData.ArtifactOffset, 0 - VoxelData.ArtifactOffset));
                    }

                    if(child.transform.name == "FrontFace")
                    {
                        child.GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", new Vector2((VoxelData.GetVoxel((byte)selectedBlock).Faces[2] * 0.1f - 0.1f) - VoxelData.ArtifactOffset, 0 - VoxelData.ArtifactOffset));
                    }

                    if(child.transform.name == "LeftFace")
                    {
                        child.GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", new Vector2((VoxelData.GetVoxel((byte)selectedBlock).Faces[4] * 0.1f - 0.1f) - VoxelData.ArtifactOffset, 0 - VoxelData.ArtifactOffset));
                    }
                }
            }            
        }

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

        if(Input.GetKeyDown(KeyCode.Return))
        {
            GameObject.Find("World").GetComponent<World>().RegenerateWorld();
        }

    }
}
