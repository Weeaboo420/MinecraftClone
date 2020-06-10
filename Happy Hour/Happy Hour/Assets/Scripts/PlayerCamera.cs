using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    
    private Camera cam;
    private PlayerData playerData;
    private FreeModeCamera freeModeCamera;
    private bool canUseTriggers = true;

    void Start()
    {
        cam = GetComponent<Camera>();
        freeModeCamera = GetComponent<FreeModeCamera>();
        playerData = GameObject.Find("PlayerData").GetComponent<PlayerData>();
    }

    void Update()
    {

        if(Input.GetAxis("RT") == 0)
        {
            canUseTriggers = true;
        }

        /*if(Input.GetKeyDown(KeyCode.C))
        {
            Utilities.ClearConsole();
        }*/

        if(Input.GetMouseButtonDown(0) && !freeModeCamera.UsingController || Input.GetAxis("RT") > 0 && freeModeCamera.UsingController && canUseTriggers)
        {
            canUseTriggers = false;
            float distance = 80f;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, distance))
            {
                //Grab a reference to the chunk that was clicked,
                //calculate the relative position inside the chunk that
                //was clicked, then round that position into integers
                GameObject chunk = hit.transform.gameObject;
                Vector3 posInChunk = hit.point - chunk.transform.position + new Vector3(0, 0.5f, 0);

                posInChunk.x = Mathf.FloorToInt(posInChunk.x);
                posInChunk.y = Mathf.FloorToInt(posInChunk.y);
                posInChunk.z = Mathf.FloorToInt(posInChunk.z);

                //Modify the block id
                TerrainGenerator chunkScript = chunk.GetComponent<TerrainGenerator>();
                chunkScript.SetBlock((int) posInChunk.x, (int) posInChunk.y, (int) posInChunk.z, (VoxelData.VoxelNames)playerData.Block);                

                //Update the mesh, only modifying the triangles of the block
                chunkScript.SetBlockMesh((int)posInChunk.x, (int)posInChunk.y, (int)posInChunk.z);                

                
                //Update surrounding chunks if applicable

                /*if(posInChunk.x == WorldSettings.ChunkWidth-1)
                {
                    TerrainGenerator adjacentChunk = Utilities.FindChunk(new Vector3(chunkScript.Position.x + WorldSettings.ChunkWidth, chunkScript.Position.y, chunkScript.Position.z));
                    if(adjacentChunk)
                    {                        
                        adjacentChunk.SetBlockMesh(0, (int)posInChunk.y, (int)posInChunk.z, true, true, true);
                    }
                }

                if(posInChunk.x == 0)
                {
                    TerrainGenerator adjacentChunk = Utilities.FindChunk(new Vector3(chunkScript.Position.x - WorldSettings.ChunkWidth, chunkScript.Position.y, chunkScript.Position.z));
                    if(adjacentChunk)
                    {                        
                        adjacentChunk.SetBlockMesh(WorldSettings.ChunkWidth-1, (int)posInChunk.y, (int)posInChunk.z, true, true, true);
                    }
                }

                if(posInChunk.z == WorldSettings.ChunkWidth-1)
                {
                    TerrainGenerator adjacentChunk = Utilities.FindChunk(new Vector3(chunkScript.Position.x, chunkScript.Position.y, chunkScript.Position.z + WorldSettings.ChunkWidth));
                    if(adjacentChunk)
                    {                        
                        adjacentChunk.SetBlockMesh((int)posInChunk.x, (int)posInChunk.y, 0, true, true, true);
                    }
                }

                if(posInChunk.z == 0)
                {
                    TerrainGenerator adjacentChunk = Utilities.FindChunk(new Vector3(chunkScript.Position.x, chunkScript.Position.y, chunkScript.Position.z - WorldSettings.ChunkWidth));
                    if(adjacentChunk)
                    {                        
                        adjacentChunk.SetBlockMesh((int)posInChunk.x, (int)posInChunk.y, WorldSettings.ChunkWidth-1, true, true, true);
                    }
                }*/

                //chunkScript.SetBlockMesh((int)posInChunk.x + 1, (int)posInChunk.y, (int)posInChunk.z);


            }
            
        }
    }
}
