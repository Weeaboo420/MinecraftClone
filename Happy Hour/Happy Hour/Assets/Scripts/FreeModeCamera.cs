using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FreeModeCamera : MonoBehaviour
{
    private float moveSpeed, speedMultiplier = 1f;
    private float horizontalMouseSpeed = 1.2f, verticalMouseSpeed = 1.2f,  minFov = 60, maxFov = 100, fovIncrement = 2f;
    private int inputs = 0;
    private bool movingCamera = true, sprinting = false;
    private Camera _camera;

    public GameObject sunLight;    

    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    private void HandleSpeed()
    {
        //Limit max speed based on the amount of inputs. This prevents going double the normal speed when moving diagonally.
        if (inputs >= 2)
        {
            if (!sprinting)
            {
                speedMultiplier = 0.65f;
            } else
            {
                speedMultiplier = 0.9f;
            }
        }

        else if (inputs < 2)
        {
            if (!sprinting)
            {
                speedMultiplier = 1f;
            }
        }
    }

    void Update()
    {

        //Make the sunlight always be a constant distance away from the player
        Vector3 sunLightPos = sunLight.transform.position;
        sunLightPos = transform.position;
        sunLight.transform.position = sunLightPos;

        //Basic setup
        movingCamera = Input.GetMouseButton(1);
        moveSpeed = 4.5f * speedMultiplier;
        sprinting = Input.GetKey(KeyCode.LeftShift);

        switch(sprinting)
        {
            case true:
                if(inputs < 2)
                {
                    speedMultiplier = 1.7f;                    
                }
                break;

            case false:
                if(inputs < 2)
                {
                    speedMultiplier = 1f;
                }
                break;
        }

        //Handle sprinting
        if (Input.GetKeyDown(KeyCode.LeftShift) && inputs < 2)
        {
            speedMultiplier = 1.3f;            
        }

        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            speedMultiplier = 1f;           
        }

        //Handle fov changes
        if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            //Scollwheel down
            if (_camera.fieldOfView < maxFov)
            {
                _camera.fieldOfView += fovIncrement;
            }

            if(_camera.fieldOfView > maxFov)
            {
                _camera.fieldOfView = maxFov;
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            //Scollwheel up
            if (_camera.fieldOfView > minFov)
            {
                _camera.fieldOfView -= fovIncrement;
            }

            if (_camera.fieldOfView < minFov)
            {
                _camera.fieldOfView = minFov;
            }
        }

        //Handle camera rotation and clamping
        Vector3 myRot = transform.rotation.eulerAngles;
        if (movingCamera)
        {
            myRot.y += Input.GetAxisRaw("Mouse X") * horizontalMouseSpeed;
            myRot.x -= Input.GetAxisRaw("Mouse Y") * verticalMouseSpeed;            
        }
        transform.rotation = Quaternion.Euler(myRot);
        Quaternion myQRot = transform.rotation;
        myQRot.x = Mathf.Clamp(myQRot.x, -90, 90);
        transform.rotation = myQRot;


        //Limit max speed based on the amount of inputs. This prevents going double the normal speed when moving diagonally.
        if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
        {
            inputs += 1;
            if(inputs > 4)
            {
                inputs = 4;
            }

            HandleSpeed();

        }

        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D))
        {
            inputs -= 1;
            if(inputs < 0 )
            {
                inputs = 0;
            }

            HandleSpeed();
        }


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
}
