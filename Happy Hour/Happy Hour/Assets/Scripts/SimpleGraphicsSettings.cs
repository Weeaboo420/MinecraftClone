using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class SimpleGraphicsSettings : MonoBehaviour
{
    private bool useVsync = true;
    private bool showFps = false;
    private Text fpsCounter;
    private bool canPoll = true;
    private GameObject ui;
    private GameObject uiCamera;

    void Start()
    {        
        Application.targetFrameRate = 200;
        fpsCounter = GameObject.Find("fpsCounter").GetComponent<Text>();
        ui = GameObject.Find("Canvas");
        uiCamera = GameObject.Find("UI Camera");

        switch (useVsync)
        {
            case true:
                QualitySettings.vSyncCount = 1;        
                break;
            case false:
                QualitySettings.vSyncCount = 0;  
                break;
        }
    }

    IEnumerator fpsPollTick()
    {
        canPoll = false;
        yield return new WaitForSeconds(1f);
        fpsCounter.text = "fps:" + Mathf.RoundToInt((1.0f / Time.smoothDeltaTime)).ToString();
        canPoll = true;
    }

    void Update()
    {

        //Manage fps display
        fpsCounter.enabled = showFps;
        if(showFps)
        {
            if (fpsCounter.text.Length == 0)
            {
                fpsCounter.text = "fps:" + Mathf.RoundToInt((1.0f / Time.deltaTime)).ToString();
            }

            if (canPoll)
            {
                StartCoroutine("fpsPollTick");
            }
        }

        //Toggle vsync
        if(Input.GetKeyDown(KeyCode.V))
        {

            useVsync = !useVsync;

            switch(useVsync)
            {
                case true:
                    QualitySettings.vSyncCount = 1;
                    break;
                case false:
                    QualitySettings.vSyncCount = 0;
                    break;
            }

        }

        //Toggle fps display
        if (Input.GetKeyDown(KeyCode.F))
        {
            showFps = !showFps;
        }

        //Toggle user interface
        if(Input.GetKeyDown(KeyCode.H))
        {
            ui.SetActive(!ui.activeSelf);
            uiCamera.SetActive(!uiCamera.activeSelf);
        }

    }
}
