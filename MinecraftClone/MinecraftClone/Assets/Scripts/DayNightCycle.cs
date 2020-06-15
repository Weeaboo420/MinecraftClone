using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DayNightCycle : MonoBehaviour
{
    public float rotationSpeed = 1f, colorLerpSpeed = 2f;
    public bool night;
    public Material sunMaterial;
    public Material moonMaterial;
    public Material skyboxMaterial;
    public float skyboxFadeSpeed;

    public Color dayColor, nightColor;
    public Color ambientDayColor, ambientNightColor;

    public Light sunlight, moonlight;


    void Start()
    {        
        //Make sure that we get the correct settings at the beginning
        skyboxMaterial.SetColor("_Tint", dayColor);
        sunlight.intensity = 1.22f;
        moonlight.intensity = 0f;
        RenderSettings.ambientIntensity = 1.15f;
    }

    void Update()
    {
        
        Color newSunColor = sunMaterial.color;
        Color newMoonColor = moonMaterial.color;
        Color newSkyboxColor = skyboxMaterial.GetColor("_Tint");
        Color ambientColor = RenderSettings.ambientSkyColor;

        float sunlightIntensity = sunlight.intensity;
        float moonlightIntensity = moonlight.intensity;

        if(night)
        {
            
            RenderSettings.sun = moonlight;
            RenderSettings.ambientIntensity = 0.8f;

            newSunColor.a = Mathf.LerpUnclamped(newSunColor.a, 0f, Time.deltaTime * colorLerpSpeed);
            newMoonColor.a = Mathf.LerpUnclamped(newMoonColor.a, 1f, Time.deltaTime * colorLerpSpeed);
            newSkyboxColor = Color.LerpUnclamped(newSkyboxColor, nightColor, Time.deltaTime * skyboxFadeSpeed);
            ambientColor = Color.LerpUnclamped(ambientColor, ambientNightColor, Time.deltaTime * skyboxFadeSpeed);

            sunlightIntensity = Mathf.LerpUnclamped(sunlightIntensity, 0f, Time.deltaTime * colorLerpSpeed);
            moonlightIntensity = Mathf.LerpUnclamped(moonlightIntensity, 1f, Time.deltaTime * colorLerpSpeed);

        } else {

            RenderSettings.sun = sunlight;
            RenderSettings.ambientIntensity = 1.15f;

            newSunColor.a = Mathf.LerpUnclamped(newSunColor.a, 1f, Time.deltaTime * colorLerpSpeed);
            newMoonColor.a = Mathf.LerpUnclamped(newMoonColor.a, 0f, Time.deltaTime * colorLerpSpeed);
            newSkyboxColor = Color.LerpUnclamped(newSkyboxColor, dayColor, Time.deltaTime * skyboxFadeSpeed);
            ambientColor = Color.LerpUnclamped(ambientColor, ambientDayColor, Time.deltaTime * skyboxFadeSpeed);

            sunlightIntensity = Mathf.LerpUnclamped(sunlightIntensity, 1.22f, Time.deltaTime * colorLerpSpeed);
            moonlightIntensity = Mathf.LerpUnclamped(moonlightIntensity, 0f, Time.deltaTime * colorLerpSpeed);
        }

        sunMaterial.color = newSunColor;
        moonMaterial.color = newMoonColor;
        skyboxMaterial.SetColor("_Tint", newSkyboxColor);
        RenderSettings.ambientSkyColor = ambientColor;

        sunlight.intensity = sunlightIntensity;
        moonlight.intensity = moonlightIntensity;

        if(transform.rotation.eulerAngles.x >= 0 && transform.rotation.eulerAngles.x <= 120)
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
