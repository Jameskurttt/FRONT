using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    public EnemyChasePro enemy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemy.StartChasing(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemy.StopChasing();
        }
    }
}