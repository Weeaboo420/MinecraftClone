using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class WaterTile : MonoBehaviour
{
    private MeshRenderer _renderer;
    private Material _mat;    
    private float xSpeed = 0.8f, ySpeed = 0.5f;

    void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
        _mat = _renderer.material;
    }

    void Update()
    {
        Vector2 offset = _mat.mainTextureOffset;
        offset.x += xSpeed * Time.deltaTime;
        offset.y += ySpeed * Time.deltaTime;
        _mat.mainTextureOffset = offset;
        _renderer.material = _mat;
    }
}
