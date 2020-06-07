using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerData : MonoBehaviour
{

    private bool useCursor = false;
    private int selectedBlock = 0;
    private Text blockText;
    public GameObject uiBlock;
    public float x, y;
    public GameObject SelectedBlockCursor;
    private Camera mainCam;
    public TerrainGenerator lookingAtChunk;

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
        mainCam = GameObject.Find("Main Camera").GetComponent<Camera>();
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
                    Voxel block = VoxelData.GetVoxel((byte)selectedBlock);

                    if(child.transform.name == "TopFace")
                    {
                        child.GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", Utilities.GetUvCoordinates(block, block.Faces[0], x, y));
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

    private Vector3 CalcSelectedCursor(Vector3 point)
    {
        Vector3 newPos = new Vector3(0, 0, 0);        

        newPos.x = Mathf.FloorToInt(point.x);

        /*if(point.y <= mainCam.transform.position.y)
        {
            newPos.y = Mathf.FloorToInt(point.y - 0.5f);
        }

        if(point.y > mainCam.transform.position.y)
        {
            newPos.y = Mathf.FloorToInt(point.y + 0.5f);
        }*/   

        newPos.y = Mathf.FloorToInt(point.y);
        newPos.z = Mathf.FloorToInt(point.z);
        return newPos;
    }

    void Update()
    {

        
        if(useCursor)
        {

            RaycastHit hit;
            if(Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out hit, 5f))
            {
                Vector3 selectedCursorPos = SelectedBlockCursor.transform.position;            
                selectedCursorPos = CalcSelectedCursor(hit.point);
                SelectedBlockCursor.transform.position = selectedCursorPos;

                if(hit.transform.TryGetComponent<TerrainGenerator>(out TerrainGenerator chunk))
                {
                    lookingAtChunk = chunk;
                }

                Vector3 bounds = lookingAtChunk.Position + new Vector3(WorldSettings.ChunkWidth-1, WorldSettings.ChunkHeight-1, WorldSettings.ChunkWidth-1);
                if(selectedCursorPos.x <= bounds.x && selectedCursorPos.y <= bounds.y && selectedCursorPos.z <= bounds.z)
                {
                    
                    if(lookingAtChunk.Data[Utilities.FormatKey(new Vector3(selectedCursorPos.x, selectedCursorPos.y, selectedCursorPos.z))].Id == 0)
                    {
                        SelectedBlockCursor.SetActive(false);            
                    } 
                    else 
                    {
                        if(!SelectedBlockCursor.activeSelf)
                        {
                            SelectedBlockCursor.SetActive(true);                        
                        }
                    }
                } 
                
                else 
                {
                    if(SelectedBlockCursor.activeSelf)
                    {
                        SelectedBlockCursor.SetActive(false);                    
                    }
                }

            } 
            
            else 
            {
                if(SelectedBlockCursor.activeSelf)
                {
                    SelectedBlockCursor.SetActive(false);
                    lookingAtChunk = null;
                }
            }

        }

        if(Input.GetKeyDown(KeyCode.Alpha2) || Input.GetButtonDown("RB"))
        {
            if(selectedBlock == VoxelData.Voxels.Count - 1)
            {
                selectedBlock = 0;
            } else {
                selectedBlock += 1;
            }

            SelectedBlockChanged();

        }

        if(Input.GetKeyDown(KeyCode.Alpha1) || Input.GetButtonDown("LB"))
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

        if(Input.GetKeyDown(KeyCode.R))
        {
            x += 0.1f;
            SelectedBlockChanged();    
        }

        if(Input.GetKeyDown(KeyCode.T))
        {
            x -= 0.1f;
            SelectedBlockChanged();    
        }

    }
}
