using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private List<string> trackNames;
    private AudioSource audioSource;

    void Start()
    {
        //Create a list that will store the names and paths of all music tracks,
        //get a reference to the tracks directory and get all files contained
        //within it and then populate the list accordingly.
        trackNames = new List<string>();
        string path = "Sound/Music";
        DirectoryInfo audioFolder = new DirectoryInfo("Assets/Resources/" + path);
        FileInfo[] files = audioFolder.GetFiles("*.wav");

        foreach(FileInfo file in files)
        {
            trackNames.Add(path + "/" + file.Name.Replace(".wav", ""));
        }

        audioSource = GetComponent<AudioSource>();
        LoadRandomTrack();

    }

    //Loads a random track based on the list of track names
    private void LoadRandomTrack()
    {
        AudioClip newTrack = Resources.Load<AudioClip>(trackNames[Random.Range(0, trackNames.Count)]);
        audioSource.clip = newTrack;
        audioSource.Play();
    }

}
