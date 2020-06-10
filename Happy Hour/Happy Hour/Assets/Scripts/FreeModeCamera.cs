using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FreeModeCamera : MonoBehaviour
{
    private float moveSpeed = 5f;
    private float horizontalMouseSpeed = 1.2f, verticalMouseSpeed = 1.2f,  minFov = 60, maxFov = 100, fovIncrement = 2f;    
    private Camera _camera;
    private Camera _debugCamera;

    public GameObject sunLight;


    //Input
    public bool usingController = false;
    private bool canPressDpad = true;
    private GameObject controls_pc, controls_xbox;

    void Start()
    {
        _camera = GetComponent<Camera>();
        _debugCamera = GameObject.Find("Debug Camera").GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; 

        //Find the control explanations for both PC and Xbox, then hide 
        //the ones for Xbox since this is primarily a PC game
        controls_pc = GameObject.Find("controls_pc");
        controls_xbox = GameObject.Find("controls_xbox");
        controls_xbox.SetActive(false);
    }

    public bool UsingController
    {
        get
        {
            return usingController;
        }
    }

    public void ChangeLockMode()
    {
        switch(Cursor.lockState)
        {
            //Lock the cursor and hide it
            case CursorLockMode.None:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;

            //Unlock the cursor and show it
            case CursorLockMode.Locked:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
        }
    }


    //Gets called every time we change between input modes,
    //when we change from controller to keyboard and mouse and
    //vice versa
    private void InputModeChanged()
    {
        switch(usingController)
        {
            case true:
                controls_pc.SetActive(false);
                controls_xbox.SetActive(true);
                break;
            case false:
                controls_xbox.SetActive(false);
                controls_pc.SetActive(true);
                break;
        }
    }

    void Update()
    {

        if(Input.GetAxis("Dpad Y") == 0 && Input.GetAxis("Dpad X") == 0)
        {
            canPressDpad = true;
        }

        if(Input.GetAxisRaw("Horizontal Keyboard") != 0 || Input.GetAxisRaw("Vertical Keyboard") != 0 || Input.inputString.Length > 0)
        {
            usingController = false;
            InputModeChanged();
        }

        if(Input.GetAxis("Right stick x") != 0 || Input.GetAxis("Right stick y") != 0 || Input.GetButtonDown("RB") || Input.GetButtonDown("LB") | Input.GetAxis("Left stick x") != 0 || Input.GetAxis("Left stick y") != 0 || Input.GetButtonDown("A") || Input.GetButtonDown("B") || Input.GetButtonDown("X") || Input.GetButtonDown("Y"))
        {
            usingController = true;
            InputModeChanged();
        } 

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ChangeLockMode();
        }

        //Make the sunlight always be a constant distance away from the player
        Vector3 sunLightPos = sunLight.transform.position;
        sunLightPos = transform.position;
        sunLight.transform.position = sunLightPos;

        //Handle fov changes
        if(Input.GetAxis("Mouse ScrollWheel") < 0 || Input.GetAxis("Dpad Y") < 0 && canPressDpad)
        {
            canPressDpad = false;
            //Scollwheel down, dpad down
            if (_camera.fieldOfView < maxFov)
            {
                _camera.fieldOfView += fovIncrement;              
            }

            if(_camera.fieldOfView > maxFov)
            {
                _camera.fieldOfView = maxFov;                
            }

            _debugCamera.fieldOfView = _camera.fieldOfView;

        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0 || Input.GetAxis("Dpad Y") > 0 && canPressDpad)
        {
            canPressDpad = false;
            //Scollwheel up, dpad up
            if (_camera.fieldOfView > minFov)
            {
                _camera.fieldOfView -= fovIncrement;                
            }

            if (_camera.fieldOfView < minFov)
            {
                _camera.fieldOfView = minFov;                
            }

            _debugCamera.fieldOfView = _camera.fieldOfView;

        }

        //Handle camera rotation and clamping
        Vector3 myRot = transform.eulerAngles;

        if(!usingController)
        {
            myRot.y += Input.GetAxisRaw("Mouse X") * horizontalMouseSpeed;
            myRot.x -= Input.GetAxisRaw("Mouse Y") * verticalMouseSpeed;

            if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
            {
                transform.Translate(transform.forward *  Time.deltaTime  *  moveSpeed, Space.World);
            }

            else if (Input.GetKey(KeyCode.S)  && !Input.GetKey(KeyCode.W))
            {
                transform.Translate(-transform.forward * Time.deltaTime * moveSpeed, Space.World);
            }

            if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            {
                transform.Translate(-transform.right * Time.deltaTime * moveSpeed, Space.World);
            }

            else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
            {
                transform.Translate(transform.right * Time.deltaTime * moveSpeed, Space.World);
            }
        } 
        
        else 
        {
            myRot.y += Input.GetAxis("Right stick x") * horizontalMouseSpeed;
            myRot.x -= Input.GetAxis("Right stick y") * verticalMouseSpeed;

            if (Input.GetAxis("Left stick y") > 0)
            {
                transform.Translate(transform.forward *  Time.deltaTime  *  moveSpeed, Space.World);
            }

            else if (Input.GetAxis("Left stick y") < 0)
            {
                transform.Translate(-transform.forward * Time.deltaTime * moveSpeed, Space.World);
            }

            if (Input.GetAxis("Left stick x") < 0)
            {
                transform.Translate(-transform.right * Time.deltaTime * moveSpeed, Space.World);
            }

            else if (Input.GetAxis("Left stick x") > 0)
            {
                transform.Translate(transform.right * Time.deltaTime * moveSpeed, Space.World);
            }

        }

        if(myRot.x > 180f)
        {
            myRot.x = myRot.x - 360f;
        }

        myRot.x = Mathf.Clamp(myRot.x, -89.9f, 89.9f);        
        transform.rotation = Quaternion.Euler(myRot);        

        

    }
}
