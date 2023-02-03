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
        /*
        Vector2 vec = Random.insideUnitCircle;
        direction = new Vector3(vec.x , 0, vec.y);
        transform.forward = direction;
        */
    }

    //Moves this GameObject 2 units a second in the forward direction
    void Update()
    {
        // TODO -- add trail renderer!!!!

        //transform.Translate(this.direction * Time.deltaTime * speed);
        transform.Translate( transform.forward * Time.deltaTime * speed, Space.World );
        Collider[] collisions = Physics.OverlapSphere(transform.position, _radius, Physics.AllLayers);
        foreach(Collider col in collisions)
            OnCollision(col);
        /*if(collisions.Length > 0)
            speed = speed * -1;
            */

    }

    private void OnCollision(Collider collider) {

        // ignore collisions with whatever created the projectile
        // TODO -- this might interfere with reflected projectiles
        if(collider == _ownerCollider)
            return;

        // TODO -- this is probably not the best way of checking
        //   maybe should be looking for a HealthBase type of component on the object
        if(collider.gameObject.tag == "Player")
        {
            Debug.Log("PLAYER HIT");
            Destroy(gameObject);

        } else {
            // We'll assume wall or something unimportant for now
            // Kill projectile
            Destroy(gameObject);
        }

    }

    public void SpawnBullet(GameObject owner, Vector2 dir, float speed = 2.0f)
    {
        _owner = owner;
        _ownerCollider = owner.GetComponent<Collider>();
        this.direction = new Vector3(dir.x,0,dir.y);
        //this.speed = speed;
        transform.forward = this.direction;
    }

}
