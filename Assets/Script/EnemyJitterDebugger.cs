using UnityEngine;
using UnityEngine.AI;

public class EnemyJitterDebugger : MonoBehaviour
{
    public NavMeshAgent agent;

    private Vector3 lastPosition;
    private Quaternion lastRotation;

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    void Update()
    {
        float posDelta = Vector3.Distance(transform.position, lastPosition);
        float rotDelta = Quaternion.Angle(transform.rotation, lastRotation);

        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }
}