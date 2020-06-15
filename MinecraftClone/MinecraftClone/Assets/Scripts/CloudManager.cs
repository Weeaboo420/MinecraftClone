using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CloudManager : MonoBehaviour
{
    //A list of all cloud models
    private List<Mesh> models;

    //A distance that will define the maximum range from which clouds
    //can spawn in relation to this object, if it's 50 then clouds
    //may not spawn further away than 50 units from this object, and so on...
    //private float spawnline = 150f;

    //Defines a maximum value for height variation when spawning a cloud
    private float heightVariation = 35f;

    //A reference to the player object, used for offsetting this object
    private Transform player;

    //An offset used to place this object at a constant
    //distance away from the player
    public Vector3 offset;

    //Save a reference to the cloud material in here instead
    //of accessing it with Resources.Load every time
    private Material cloudMaterial;

    //Keep a list of all clouds so that we can limit how many
    //clouds can be present at any given time
    private List<Cloud> clouds;

    //The limit for how many clouds can exist at the same time in the world
    private byte limit = 9;

    void Start()
    {

        //Load material
        cloudMaterial = Resources.Load<Material>("Materials/Cloud");

        //Find the player
        player = GameObject.Find("Cloud Camera").transform;        

        //Initialize the lists
        models = new List<Mesh>();
        clouds = new List<Cloud>();

        //Load all cloud models
        foreach(Mesh model in Resources.LoadAll<Mesh>("Models/Clouds"))
        {
            models.Add(model);
        }

        //Spawn a cloud immediately, this way the sky looks less empty
        SpawnCloud();

        //Start the spawning process
        StartCoroutine("spawner");

    }

    //Spawns clouds periodically
    IEnumerator spawner()
    {
        //Wait for a random amount of seconds as per the range
        yield return new WaitForSeconds(Random.Range(8, 16));
        SpawnCloud();

        //Move this object at a constant offset from the player after we spawn a cloud,
        //we don't modify the y-value since the player should be able to reach the
        //clouds if they build high enough
        /*Vector3 myPos = transform.position;
        myPos.x = player.transform.position.x + offset.x;
        myPos.z = player.transform.position.z + offset.z;
        transform.position = myPos;*/

        //Restart the process
        StartCoroutine("spawner");

    }

    //Spanws a cloud
    private void SpawnCloud()
    {
        //Make sure that there is room to spawn another cloud
        if(clouds.Count < limit)
        {
            //Create the base object for the cloud, it is empty by default
            GameObject cloud = new GameObject("cloud");

            //Set the position of the newly spawned cloud            
            cloud.transform.position = transform.position + new Vector3(0, Random.Range(0, heightVariation), 0);

            //Add a mesh filter so we can actually store a cloud model that we then
            //can render
            MeshFilter meshFilter = cloud.AddComponent<MeshFilter>();        
            meshFilter.mesh = models[Random.Range(0, models.Count)];

            //Add a mesh renderer so we can render the mesh,
            //disable shadow casting and shadow reception since this
            //will probably be costly on the gpu, I haven't tried it though.
            MeshRenderer meshRenderer = cloud.AddComponent<MeshRenderer>();
            meshRenderer.material = cloudMaterial;
            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;

            //Fixes the rotation of the cloud because for some reason the models I made
            //are rotated incorrectly.
            Vector3 cloudRotation = cloud.transform.rotation.eulerAngles;
            cloudRotation.x = -90f;
            cloudRotation.z = -90f;
            cloud.transform.rotation = Quaternion.Euler(cloudRotation);

            //Set the scale for the cloud
            float randomScale = Random.Range(18f, 50f);
            cloud.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

            //Add the cloud script
            cloud.AddComponent<Cloud>();

            //Add the cloud to the list of clouds
            clouds.Add(cloud.GetComponent<Cloud>());
        }

    }
}
