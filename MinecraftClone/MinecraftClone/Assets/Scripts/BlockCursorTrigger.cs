using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCursorTrigger : MonoBehaviour
{
    private PlayerCamera parentScript;

    public void SetParent(PlayerCamera newParent)
    {
        parentScript = newParent;
    }

    void OnTriggerEnter(Collider other)
    {
        parentScript.CursorIsColliding = other.transform.CompareTag("Chunk");
    }

    void OnTriggerExit(Collider other)
    {
        parentScript.CursorIsColliding = !other.transform.CompareTag("Chunk");
    }
}
