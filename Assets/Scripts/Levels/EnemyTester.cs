using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTester : MonoBehaviour
{
    public GameObject currentRoom;
    public GameObject target;

    private Vector3 nextMoveTarget;

    // Start is called before the first frame update

    void Start()
    {
        nextMoveTarget = target.transform.position + Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentRoom.GetComponent<MeshNavigator>().getPath(transform.position,target.transform.position, ref nextMoveTarget))
        {
            Debug.Log(nextMoveTarget);
            transform.LookAt(nextMoveTarget);
            transform.Translate(transform.forward * .5f * Time.deltaTime);
        }
    }

    
    [ContextMenu("TestingNav")]
    public void TestingNav() {
        Debug.Log(System.DateTimeOffset.Now.ToUnixTimeMilliseconds());
        Vector3 nextMove = new Vector3();
        Debug.Log("Result :" + currentRoom.GetComponent<MeshNavigator>().getPath(transform.position,target.transform.position, ref nextMove));
        Debug.Log("Next move: " + nextMove);
        Debug.Log(System.DateTimeOffset.Now.ToUnixTimeMilliseconds());
    }
    
    [ContextMenu("TestingTime")]
    public void TestingTime() {
        Debug.Log(System.DateTimeOffset.Now.ToUnixTimeSeconds());
    }
}
