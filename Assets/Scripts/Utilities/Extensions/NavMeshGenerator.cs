using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshGenerator : MonoBehaviour
{

    public GameObject floorObj;
    public float navResolution = 1f;

    List<Vector3> walkablePosList;
    List<Vector3> rayHits;

    private Bounds _roomBounds;
    private bool[,] gridMesh;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if(_roomBounds != null)
        {
            Gizmos.color = new Color(0f,1f,0f,1f);
            Gizmos.DrawWireCube(_roomBounds.center, _roomBounds.size);
        }

        if(walkablePosList != null)
        {
            foreach(Vector3 v in walkablePosList)
            {
                Gizmos.color = new Color(0f,1f,0f,1f);
                Gizmos.DrawWireCube(v,new Vector3(navResolution,.2f,navResolution));
            }
        }

        if(rayHits != null)
        {
            foreach(Vector3 v in rayHits)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(v,navResolution/2.0f);
            }
        }


    }

    [ContextMenu("ClearNavData")]
    public void ClearNavData() {
        gridMesh = null;
        walkablePosList = null;
        rayHits = null;
    }

    [ContextMenu("GenerateNavMesh")]
    public void RunNavMeshGenerator()
    {
        
        //var data = dungeonProfile.GenerateDungeon(rootRoom, roomPrefabs);
        Debug.Log("Running navmesh generator...");

        // Get all children bounds
        MeshRenderer[] childMeshes = GetComponentsInChildren<MeshRenderer>();
        _roomBounds = childMeshes[0].bounds;
        for(int i=0; i<childMeshes.Length;i++)
        {
            _roomBounds.Encapsulate(childMeshes[i].bounds);
        }

        // Get floor bounds
        Bounds floorBounds = floorObj.GetComponent<MeshRenderer>().bounds;
        float floorY = floorBounds.max.y;
            
        gridMesh = new bool[ (int)Mathf.Ceil(floorBounds.size.x/navResolution), (int)Mathf.Ceil(floorBounds.size.z/navResolution) ];
        walkablePosList = new List<Vector3>();

        // Center our grid on the center of the floor mesh
        float startX = ((floorBounds.size.x - gridMesh.GetLength(0)*navResolution) / 2.0f) + floorBounds.min.x + navResolution/2.0f;
        float startZ = ((floorBounds.size.z - gridMesh.GetLength(1)*navResolution) / 2.0f) + floorBounds.min.z + navResolution/2.0f;

        rayHits = new List<Vector3>();
        for(int x = 0; x<gridMesh.GetLength(0); x++)
        {
            for(int z = 0; z<gridMesh.GetLength(1); z++)
            {

                RaycastHit hit;
                Vector3 vecFrom = new Vector3(startX + x*navResolution, 100.0f, startZ + z*navResolution);
                Debug.DrawRay(vecFrom,Vector3.down*100.0f,Color.yellow,3.0f);
                
                if(Physics.SphereCast(vecFrom, navResolution/2.0f, Vector3.down, out hit, 1000.0f))
                {
                    //Debug.Log(hit.point);
                    if(hit.transform.gameObject != floorObj || hit.point.y - floorY > 0.001f )
                        continue;
                    rayHits.Add(hit.point);
                }
                

                /*

                Vector3 baseVec = new Vector3(startX + x*navResolution, 100.0f, startZ + z*navResolution);

                // raycast each corner of this square
                // cast example ray
                RaycastHit hit;
                Vector3 vecFrom = baseVec;
                Debug.DrawRay(vecFrom,Vector3.down*100.0f,Color.yellow,3.0f);
                if(Physics.Raycast(vecFrom, Vector3.down, out hit, Mathf.Infinity))
                {
                    //Debug.Log(hit.point);
                    if(hit.transform.gameObject != floorObj || hit.point.y - floorY > 0.001f )
                        continue;
                    rayHits.Add(hit.point);
                }

                vecFrom.x += navResolution;
                Debug.DrawRay(vecFrom,Vector3.down*100.0f,Color.yellow,3.0f);
                if(Physics.Raycast(vecFrom, Vector3.down, out hit, Mathf.Infinity))
                {
                    if(hit.transform.gameObject != floorObj || hit.point.y - floorY > 0.001f )
                        continue;
                    rayHits.Add(hit.point);
                }

                vecFrom.x -= navResolution;
                vecFrom.z += navResolution;
                Debug.DrawRay(vecFrom,Vector3.down*100.0f,Color.yellow,3.0f);
                if(Physics.Raycast(vecFrom, Vector3.down, out hit, Mathf.Infinity))
                {
                    if(hit.transform.gameObject != floorObj || hit.point.y - floorY > 0.001f )
                        continue;
                    rayHits.Add(hit.point);
                }

                vecFrom.z += navResolution;
                Debug.DrawRay(vecFrom,Vector3.down*100.0f,Color.yellow,3.0f);
                if(Physics.Raycast(vecFrom, Vector3.down, out hit, Mathf.Infinity))
                {
                    if(hit.transform.gameObject != floorObj || hit.point.y - floorY > 0.001f )
                        continue;
                    rayHits.Add(hit.point);
                }
                

                Vector3 walkVec = new Vector3(x*navResolution + startX + navResolution/2.0f, floorY, z*navResolution+startZ + navResolution/2.0f);
                if(walkVec.z > 16)
                {
                    Debug.Log(x + " " + z);
                    Debug.Log(walkVec);
                }
                
                walkablePosList.Add(walkVec);
*/


            }
        }

    }



}
