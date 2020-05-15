using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public float rotationSpeed = 1f, colorLerpSpeed = 2f;
    public bool night;
    public Material sunMaterial;

    void Update()
    {
        
        Color newSunColor = sunMaterial.color;
        if(night)
        {
            newSunColor.a = Mathf.LerpUnclamped(newSunColor.a, 0f, Time.deltaTime * colorLerpSpeed);
        } else {
            newSunColor.a = Mathf.LerpUnclamped(newSunColor.a, 1f, Time.deltaTime * colorLerpSpeed);
        }
        sunMaterial.color = newSunColor;


        if(transform.rotation.eulerAngles.x >= 0 && transform.rotation.eulerAngles.x <= 145)
        {
            night = false;
        } 
        
        else
        {
            night = true;
        }       

        transform.RotateAround(transform.position, Vector3.right, 1f * Time.deltaTime * rotationSpeed);
    }
}
