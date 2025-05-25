using UnityEngine.AI;
using UnityEngine;
using Unity.AI.Navigation;

public class Player : MonoBehaviour
{
    NavMeshAgent agent;
    //public GameObject target;
    private NavMeshSurface surface;
    //public bool updateNavmesh;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        surface = GameObject.Find("NavSurface").GetComponent<NavMeshSurface>();
    }

    // Update is called once per frame
    void Update()
    {
        //agent.destination = target.transform.position;

        //if (updateNavmesh)
        //{
        //    surface.BuildNavMesh();
        //    updateNavmesh = false;
        //}
    }

    public void UpdateNavMesh()
    {
        surface.BuildNavMesh();
    }
}
