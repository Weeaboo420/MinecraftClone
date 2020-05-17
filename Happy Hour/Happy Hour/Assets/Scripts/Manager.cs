﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;

public static class Beta
{
    private static List<Guid> _keys = new List<Guid>() 
    {
        new Guid("928d4b58-d6c5-41ad-87d3-db20eecdc929"),
        new Guid("afbef41e-8ebc-40a7-9890-f7506725c214"),
        new Guid("b7397bf6-d546-42f0-bb85-cb6fd25c9fe2")
    };

    public static List<Guid> Keys
    {
        get 
        {
            return _keys;
        }
    }
}


public class Manager : MonoBehaviour
{
    public InputField keyField;
    public Text invalidKeyText;

    private bool IsValidKey(string input)
    {
        foreach(Guid key in Beta.Keys)
        {
            if(input == key.ToString())
            {
                return true;
            }
        }

        return false;

    }

    void Awake()
    {
        /*if(PlayerPrefs.HasKey("Beta-Key"))
        {
            if(IsValidKey(PlayerPrefs.GetString("Beta-Key")))
            {
                SceneManager.LoadScene(1);
            }
        }*/
    }

    /*IEnumerator GetKeys()
    {
        UnityWebRequest request = new UnityWebRequest("")
    }*/

    public void Verify()
    {
        if(IsValidKey(keyField.text))
        {            
            invalidKeyText.enabled = false;
            PlayerPrefs.SetString("Beta-Key", keyField.text);
            SceneManager.LoadScene(1);
        }



        else 
        {            
            invalidKeyText.enabled = true;
        }

    }
}

