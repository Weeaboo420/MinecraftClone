using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    //Defines a speed range for which any cloud can move,
    //each cloud gets a random value between these two points
    private float minSpeed = 4.5f, maxSpeed = 8.75f;
    private float mySpeed;
    
    //Sets a range for the lifetime a cloud can have
    private float minLiftime = 10f, maxLifetime = 45f;
    private float myLifetime;
    
    //Stores how many seconds this cloud has been alive
    private byte second;

    private Color emissionColor;
    private float emissionIntensity = 0f;

    //Used to tell if this cloud is using night time graphics
    //or day time graphics
    private bool night;

    //Keep a reference to the mesh renderer so we don't
    //have to use GetComponent every time because that's
    //expensive
    private MeshRenderer meshRenderer;

    private DayNightCycle dayNightCycle;

    void Start()
    {

        dayNightCycle = GameObject.Find("Sun and moon container").GetComponent<DayNightCycle>();

        //Set this object to be on the cloud layer
        this.gameObject.layer = 11;

        //Get a speed value based on the defined range
        mySpeed = Random.Range(minSpeed, maxSpeed);

        //Get a lifetime value based on the defined range
        myLifetime = Random.Range(minLiftime, maxLifetime);

        //Sets the opacity to 0 for this object so we can make the cloud
        //fade in
        meshRenderer = GetComponent<MeshRenderer>();
        Color cloudColor = meshRenderer.material.color;
        cloudColor.a = 0f;
        meshRenderer.material.SetColor("_Color", cloudColor);

        emissionColor = meshRenderer.material.GetColor("_EmissionColor");

        //Start counting how long this cloud is alive
        StartCoroutine("addSecond");
    }

    IEnumerator addSecond()
    {
        yield return new WaitForSeconds(1);
        second += 1;

        if(second < myLifetime)
        {
            StartCoroutine("addSecond");
        }

    }

    //Allows for setting the emission color.
    //Used when transitioning between night and day
    //and vice versa, otherwise the clouds would be
    //shining really brightly during the night
    private void SetEmissionColor(Color newEmissionColor, float intensity)
    {
        emissionColor = newEmissionColor;
        emissionIntensity = intensity;
    }

    void LateUpdate()
    {
        if(dayNightCycle.night && !night)
        {
            night = true;
            SetEmissionColor(new Color(80, 80, 80, 1), -1f);
        } 
        
        else if(!dayNightCycle.night && night)
        {
            night = false;
            SetEmissionColor(new Color(180, 180, 180, 1), 0f);
        }
    }

    void Update()
    {
        //Move this cloud in the positive z-direction with the value of
        //mySpeed which we get when we create the cloud
        Vector3 myPos = transform.position;
        myPos.z += mySpeed * Time.deltaTime;
        transform.position = myPos;        

        //Modify the color of the material of the cloud,
        //we only modify the alpha channel since we want
        //to be able to fade the cloud in and out
        Color cloudColor = meshRenderer.material.color;
        
        //Determines if we should fade in or out
        if(second < myLifetime)
        {
            cloudColor.a = Mathf.LerpUnclamped(cloudColor.a, 0.85f, Time.deltaTime * 0.05f);
        } 
        
        else 
        {
            cloudColor.a = Mathf.LerpUnclamped(cloudColor.a, 0f, Time.deltaTime * 0.05f);

            if(cloudColor.a <= 0.003f)
            {
                //If we have faded out then destroy this cloud
                Destroy(this.gameObject);
            }

        }        

        meshRenderer.material.SetColor("_Color", cloudColor);

        //Adjust the emission color accordingly
        if(meshRenderer.material.GetColor("_EmissionColor") != emissionColor)
        {
            Color newEmissionColor = meshRenderer.material.GetColor("_EmissionColor");
            newEmissionColor = Color.LerpUnclamped(newEmissionColor, emissionColor, Time.deltaTime * 0.012f);
            meshRenderer.material.SetColor("_EmissionColor", newEmissionColor * emissionIntensity);
        }

    }

}
