using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshUpdater : MonoBehaviour
{
    public Transform Destination;
    public Transform NextPosition;

    NavMeshPath path;
    NavMeshAgent agent;

    public NavMeshPath Path => path;
    // Update is called once per frame
    void Start()
    {
        path = new NavMeshPath();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (!Physics.Raycast(Destination.position + Vector3.up * 2, Vector3.down, out var hitInfo, 4))
            NextPosition.position = transform.position;
        else if (!NavMesh.SamplePosition(hitInfo.point, out var hit, 0.3f, NavMesh.AllAreas))
            NextPosition.position = transform.position;
        else
        {
            agent.CalculatePath(hitInfo.point, path);
            if (path.corners.Length > 1)
                NextPosition.position = path.corners[1];
            else
                NextPosition.position = Destination.position;
        }

    }
}
