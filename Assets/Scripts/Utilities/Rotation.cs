using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] private Vector3 _rotationAxis = Vector3.up;
    [SerializeField] private float _speed = 250f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate( _rotationAxis * _speed * Time.deltaTime);
    }
}
