using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerCamera : MonoBehaviour
{
    
    private Camera cam;
    private PlayerData playerData;
    private FreeModeCamera freeModeCamera;
    private bool canUseTriggers = true;
    private bool debug = false;
    private Mesh doubleSidedPlane;
    void Start()
    {
        cam = GetComponent<Camera>();        
        freeModeCamera = GetComponent<FreeModeCamera>();
        playerData = GameObject.Find("PlayerData").GetComponent<PlayerData>();
        doubleSidedPlane = Resources.Load<Mesh>("Models/DoubleSidedPlane");
    }

    void Update()
    {

        if(Input.GetAxis("RT") == 0)
        {
            canUseTriggers = true;
        }

        if(Input.GetKeyDown(KeyCode.T))
        {
            debug = !debug;
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            Utilities.ClearConsole();
        }

        //Right click or Left Trigger
        if(Input.GetMouseButtonDown(1) && !freeModeCamera.UsingController || Input.GetAxis("LT") > 0 && freeModeCamera.UsingController && canUseTriggers)
        {
            canUseTriggers = false;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, 5f))
            {
                //Make sure that we're right clicking on a chunk
                if(hit.transform.gameObject.tag == "Chunk")
                {
                    GameObject foliage = new GameObject("foliage test obj");
                    MeshFilter meshFilter = foliage.AddComponent<MeshFilter>();
                    meshFilter.mesh = doubleSidedPlane;

                    MeshRenderer meshRenderer = foliage.AddComponent<MeshRenderer>();
                    meshRenderer.shadowCastingMode = ShadowCastingMode.On;
                    meshRenderer.receiveShadows = true;
                    meshRenderer.material = Resources.Load<Material>("Materials/Sapling01");

                    foliage.transform.position = hit.point + new Vector3(0, 0.5f, 0);                
                    BoxCollider boxCollider = foliage.AddComponent<BoxCollider>();
                    boxCollider.size = new Vector3(0.85f, 1f, 0.85f);
                    foliage.transform.tag = "Destructible";
                }

                //Rigidbody rigidbody = foliage.AddComponent<Rigidbody>();
                //rigidbody.freezeRotation = true;
            }
        }

        //Left click or Right Trigger
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
                GameObject obj = hit.transform.gameObject;

                //If the object has the tag "destructible" then we destroy it
                if(obj.transform.tag == "Destructible")
                {
                    Destroy(obj);
                }

                if(obj.TryGetComponent(out TerrainGenerator temp))
                {
                    Vector3 posInChunk = hit.point - obj.transform.position + new Vector3(0, 0.7f, 0);

                    posInChunk.x = Mathf.FloorToInt(posInChunk.x);
                    posInChunk.y = Mathf.FloorToInt(posInChunk.y);
                    posInChunk.z = Mathf.FloorToInt(posInChunk.z);                
                    //Modify the block id
                    TerrainGenerator chunkScript = obj.GetComponent<TerrainGenerator>();
                    chunkScript.SetBlock((int) posInChunk.x, (int) posInChunk.y, (int) posInChunk.z, (VoxelData.VoxelNames)playerData.Block);                

                    //Get all neigbors before updating the mesh, this is crucial, otherwise the CanDraw function
                    //won't work properly
                    chunkScript.RetrieveNeighbors();

                    //Update the mesh, only modifying the triangles of the block
                    chunkScript.SetBlockMesh((int)posInChunk.x, (int)posInChunk.y, (int)posInChunk.z);                

                    //Once we're done with the neighboring chunks we can remove them altogether
                    chunkScript.ClearNeighbors();                

                    //Update surrounding chunks if applicable
                    if(posInChunk.x == WorldSettings.ChunkWidth-1)
                    {
                        TerrainGenerator adjacentChunk = Utilities.FindChunk(new Vector3(chunkScript.Position.x + WorldSettings.ChunkWidth, chunkScript.Position.y, chunkScript.Position.z));
                        if(adjacentChunk)
                        {   
                            //We could use GenerateMesh() here but it causes ugly hitching. SetBlocKMesh() is bugged here for some reason
                            //adjacentChunk.SetBlock(0, (int)posInChunk.y, (int)posInChunk.z, VoxelData.GetVoxel(adjacentChunk.Data[Utilities.FormatKey(new Vector3(0, (int)posInChunk.y, (int)posInChunk.z))].Id).Name);
                            adjacentChunk.RetrieveNeighbors();
                            adjacentChunk.SetBlockMesh(-1, (int)posInChunk.y, (int)posInChunk.z, debug);
                            adjacentChunk.ClearNeighbors();
                        }
                    }

                    if(posInChunk.x == 0)
                    {
                        TerrainGenerator adjacentChunk = Utilities.FindChunk(new Vector3(chunkScript.Position.x - WorldSettings.ChunkWidth, chunkScript.Position.y, chunkScript.Position.z));
                        if(adjacentChunk)
                        {
                            adjacentChunk.RetrieveNeighbors();
                            adjacentChunk.SetBlockMesh(WorldSettings.ChunkWidth, (int)posInChunk.y, (int)posInChunk.z, debug);
                            adjacentChunk.ClearNeighbors();
                        }
                    }

                    if(posInChunk.z == WorldSettings.ChunkWidth-1)
                    {
                        TerrainGenerator adjacentChunk = Utilities.FindChunk(new Vector3(chunkScript.Position.x, chunkScript.Position.y, chunkScript.Position.z + WorldSettings.ChunkWidth));
                        if(adjacentChunk)
                        {                        
                            adjacentChunk.RetrieveNeighbors();                        
                            adjacentChunk.SetBlockMesh((int)posInChunk.x, (int)posInChunk.y, -1, debug);
                            adjacentChunk.ClearNeighbors();
                        }
                    }

                    if(posInChunk.z == 0)
                    {
                        TerrainGenerator adjacentChunk = Utilities.FindChunk(new Vector3(chunkScript.Position.x, chunkScript.Position.y, chunkScript.Position.z - WorldSettings.ChunkWidth));
                        if(adjacentChunk)
                        {                       
                            adjacentChunk.RetrieveNeighbors();
                            adjacentChunk.SetBlockMesh((int)posInChunk.x, (int)posInChunk.y, WorldSettings.ChunkWidth, debug);
                            adjacentChunk.ClearNeighbors();
                        }
                    }

                    //chunkScript.SetBlockMesh((int)posInChunk.x + 1, (int)posInChunk.y, (int)posInChunk.z);
                }

            }
            
        }
    }
}
