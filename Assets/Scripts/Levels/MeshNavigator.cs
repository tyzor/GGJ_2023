using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class that handles generating a navigatable mesh for a given room
 * Meshes are pre-calculating in the editor via context menus
 *
 * TODO:
 *  - Ignore everything but the level geometry when casting rays (not sure if we should use layers or tags etc)
 *  - Use a hashtable to store path calculations (using x1_z1_x2_z2 as the string key)
 *  - Calculate a quad tree for the grid where large quadrants may just allow for straight movement within them
 */


public class MeshNavigator : MonoBehaviour
{

    public GameObject floorObj;
    public float navResolution = 1f;

    List<Vector3> walkablePosList;
    List<Vector3> rayHits;

    private Bounds _roomBounds;
    //private bool[,] _gridMesh;
    [SerializeField] private bool[] _gridMesh;
    [SerializeField] private float _gridStartX;
    [SerializeField] private float _gridStartZ;
    [SerializeField] private int _gridSizeX;
    [SerializeField] private int _gridSizeZ;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private class NavNode
    {
        //public NavNode[] connections;
        public List<NavNode> history;
        public int x;
        public int z;

    }

    public bool getPath(Vector3 from, Vector3 dest, ref Vector3 nextMove) 
    {

        // Get current grid cell
        int x = (int)((from.x - _gridStartX) / navResolution);
        int z = (int)((from.z - _gridStartZ) / navResolution);

        // Get target grid cell
        int tx = (int)((dest.x - _gridStartX) / navResolution);
        int tz = (int)((dest.z - _gridStartZ) / navResolution);

        // Super early exit to minimize calls
        if(x == tx && z == tz)
        {
            nextMove.x = dest.x;
            nextMove.y = dest.y;
            nextMove.z = dest.z;
            return true;
        }    
        

        // If we are using only cardinal directions (no diagonals) then each edge would have the same weight on a graph
        // So a BFS search would already return the minimum spanning tree, we will skip A* for now

        List<NavNode> result = new List<NavNode>();
        HashSet<string> visited = new HashSet<string>();
        Queue<NavNode> work = new Queue<NavNode>();

        NavNode start = new NavNode();
        start.x = x;
        start.z = z;
        start.history = new List<NavNode>();
        visited.Add(start.x+"_"+start.z);
        work.Enqueue(start);
        
        while(work.Count > 0)
        {
            NavNode current = work.Dequeue();
            if(current.x == tx && current.z == tz)
            {
                // Found the end
                result = current.history;

                // Lets do some debug here
                for(int p=1;p<current.history.Count-1;p++)
                {
                    
                    NavNode nav = current.history[p];
                    NavNode nextNav = current.history[p+1];
                    Debug.DrawLine(new Vector3(nav.x*navResolution+_gridStartX,1.5f,nav.z*navResolution+_gridStartZ), new Vector3(nextNav.x*navResolution+_gridStartX,1.5f,nextNav.z*navResolution+_gridStartZ), Color.yellow, 3.0f );
                }

                nextMove.x = current.history[1].x * navResolution + _gridStartX;
                nextMove.y = dest.y;
                nextMove.z = current.history[1].z * navResolution + _gridStartZ;
                return true;
            } else {

                // TODO -- presetup the grid as a node graph for searching purposes
                // Didn't find end -- search neighbours
                // Definitely more elegant way to do this but trying to be quick and avoid unnecessary stuff
                int[,] childNodes = new int[,] { 
                        {current.x-1,current.z}, 
                        {current.x+1,current.z}, 
                        {current.x,current.z-1}, 
                        {current.x,current.z+1}
                };
                for(int i=0;i<4;i++)
                {
                    
                    if( !visited.Contains(childNodes[i,0] + "_" + childNodes[i,1])
                    && childNodes[i,0] > 0 && childNodes[i,1] > 0
                    && childNodes[i,0] < _gridSizeX
                    && childNodes[i,1] < _gridSizeZ
                    && _gridMesh[childNodes[i,0]+_gridSizeX*childNodes[i,1]] == true // is a walkable tile
                    ) {

                        // Valid neighbour
                        NavNode nb = new NavNode();
                        nb.x = childNodes[i,0];
                        nb.z = childNodes[i,1];
                        nb.history = new List<NavNode>(current.history);
                        nb.history.Add(current);
                        visited.Add(nb.x+"_"+nb.z);
                        work.Enqueue(nb);
                    }
                }               
            }
        }
        // No route was found if we get here
        return false;
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
        _gridMesh = null;
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
            
        _gridSizeX = (int)Mathf.Ceil(floorBounds.size.x/navResolution);
        _gridSizeZ = (int)Mathf.Ceil(floorBounds.size.z/navResolution);
        _gridMesh = new bool[ _gridSizeX * _gridSizeZ ];

        walkablePosList = new List<Vector3>();

        // Center our grid on the center of the floor mesh
        _gridStartX = ((floorBounds.size.x - _gridSizeX*navResolution) / 2.0f) + floorBounds.min.x + navResolution/2.0f;
        _gridStartZ = ((floorBounds.size.z - _gridSizeZ*navResolution) / 2.0f) + floorBounds.min.z + navResolution/2.0f;

        rayHits = new List<Vector3>();
    
        for(int i=0; i<_gridMesh.Length; i++)
        {
            int x = i % _gridSizeX;
            int z = (i / _gridSizeX);
            RaycastHit hit;
            Vector3 vecFrom = new Vector3(_gridStartX + x*navResolution, 100.0f, _gridStartZ + z*navResolution);
            //Debug.DrawRay(vecFrom,Vector3.down*100.0f,Color.yellow,3.0f);
            if(Physics.SphereCast(vecFrom, navResolution/2.0f, Vector3.down, out hit, 1000.0f))
            {
                //Debug.Log(hit.point);
                if( (hit.transform.gameObject != floorObj ) || hit.point.y - floorY > 0.001f )
                    continue;
                rayHits.Add(hit.point);

                // This current grid is walkable
                _gridMesh[x+_gridSizeX*z] = true;
            }
        }

    }



}
