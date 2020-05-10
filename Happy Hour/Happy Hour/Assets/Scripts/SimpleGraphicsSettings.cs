using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class SimpleGraphicsSettings : MonoBehaviour
{
    public bool useVsync = true;
    private bool showFps = false;
    private Text fpsCounter;
    private bool canPoll = true;

    void Start()
    {
        Application.targetFrameRate = 200;
        fpsCounter = GameObject.Find("fpsCounter").GetComponent<Text>();
        switch (useVsync)
        {
            case true:
                QualitySettings.vSyncCount = 0;
                break;
            case false:
                QualitySettings.vSyncCount = 2;
                break;
        }
    }

    IEnumerator fpsPollTick()
    {
        canPoll = false;
        yield return new WaitForSeconds(0.75f);
        fpsCounter.text = "fps:" + Mathf.RoundToInt((1.0f / Time.smoothDeltaTime)).ToString();
        canPoll = true;
    }

    void Update()
    {

        fpsCounter.enabled = showFps;
        if(showFps)
        {
            if (fpsCounter.text.Length == 0)
            {
                fpsCounter.text = "fps:" + Mathf.RoundToInt((1.0f / Time.smoothDeltaTime)).ToString();
            }

            if (canPoll)
            {
                StartCoroutine("fpsPollTick");
            }
        }

        if(Input.GetKeyDown(KeyCode.V))
        {
            switch(useVsync)
            {
                case true:
                    QualitySettings.vSyncCount = 0;
                    break;
                case false:
                    QualitySettings.vSyncCount = 2;
                    break;
            }

            useVsync = !useVsync;

        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            showFps = !showFps;
        }

    }
}
