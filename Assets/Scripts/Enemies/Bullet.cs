using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    public float speed = 2.0f;
    public Vector3 direction;

    public string searchTag;
    private GameObject _owner; // the object that created this
    private Collider _ownerCollider;

    // Caching size data for collision detection
    private Bounds _bounds;
    private float _radius;

    void Start() 
    {
        _bounds = GetComponent<MeshRenderer>().bounds;
        _radius = _bounds.extents.x;
        
        // Random direction at load
        Vector2 vec = Random.insideUnitCircle;
        direction = new Vector3(vec.x , 0, vec.y);
        transform.forward = direction;
    }

    //Moves this GameObject 2 units a second in the forward direction
    void Update()
    {
        transform.Translate(transform.forward * Time.deltaTime * speed);
        Collider[] collisions = Physics.OverlapSphere(transform.position, _radius, Physics.AllLayers);

        /*if(collisions.Length > 0)
            speed = speed * -1;
            */

    }
    public void SpawnBullet(GameObject owner, Vector2 dir)
    {
        _owner = owner;
        _ownerCollider = owner.GetComponent<BoxCollider>();
        direction = new Vector3(dir.x,transform.position.y,dir.y);
    }

}
