using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGJ.Utilities.Extensions;

public class TempWhirlAnimation : MonoBehaviour
{
    private float speed = 1.0f;
    private float strength = 0.0f;

    List<Transform> _orbs;
    // Start is called before the first frame update
    void Start()
    {
        _orbs = new List<Transform>();
        // Get all children
        foreach(Transform t in GetComponentsInChildren<Transform>())
        {
            if(t != this.transform)
                _orbs.Add(t);
        }
    }

    // Update is called once per frame
    void Update()
    {
        strength = Mathf.Sin(Time.time);

        // move orbs away from center
        foreach(Transform orb in _orbs)
        {
            Vector3 pos = orb.localPosition;
            pos.x += Mathf.Clamp(strength * Time.deltaTime,0,3.0f);
            orb.localPosition = pos;
        }
        transform.Rotate(0, 360 * Time.deltaTime * speed * 0.5f, 0 );

    }
}
