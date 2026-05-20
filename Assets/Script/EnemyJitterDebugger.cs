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

        if (posDelta > 0.01f)
        {
            Debug.Log($"[MOVE] Position changing: {posDelta}");
        }

        if (rotDelta > 2f)
        {
            Debug.Log($"[ROTATE] Rotation jitter: {rotDelta}");
        }

        if (agent != null)
        {
            Debug.Log($"[AGENT] isStopped={agent.isStopped} | hasPath={agent.hasPath} | velocity={agent.velocity.magnitude}");
        }

        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }
}