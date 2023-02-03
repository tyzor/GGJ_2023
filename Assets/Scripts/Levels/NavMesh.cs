using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class NavMesh : MonoBehaviour
{
    public NavMeshSurface surface;
    // Start is called before the first frame update
    void Start()
    {
        // Maybe should die if surface hasn't been configured?
        if(!surface)
            surface = GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();

        
    }
    
    // Has the surface finished calculating mesh data
    public bool isReady()
    {
        return (surface != null && surface.navMeshData != null);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
