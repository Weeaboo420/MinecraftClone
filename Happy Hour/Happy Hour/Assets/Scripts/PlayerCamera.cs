using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    
    private Camera cam;
    private PlayerData playerData;

    void Start()
    {
        cam = GetComponent<Camera>();
        playerData = GameObject.Find("PlayerData").GetComponent<PlayerData>();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {

            float distance = 50f;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //DEBUG FEATURE
            //Casts a ray from the mouse position, if it hits anything then it will either instantiate
            //a new vertex prefab or move an existing one
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
                chunk.GetComponent<TerrainGenerator>().SetBlock((int) posInChunk.x, (int) posInChunk.y, (int) posInChunk.z, (VoxelData.VoxelNames)playerData.Block);

            }
            
        }
    }
}
