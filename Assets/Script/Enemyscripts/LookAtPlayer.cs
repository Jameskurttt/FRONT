using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private bool onlyYRotation = true;

    void Update()
    {
        if (player == null) return;

        Vector3 direction = player.position - transform.position;
        if (onlyYRotation) direction.y = 0;

        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);
    }
}