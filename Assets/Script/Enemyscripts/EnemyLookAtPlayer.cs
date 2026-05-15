using UnityEngine;

public class EnemyLookAtPlayer : MonoBehaviour
{
    public Transform player;        // Drag the Player here in Inspector
    public float rotationSpeed = 5f;

    void Update()
    {
        if (player == null) return;

        // Get direction from enemy to player
        Vector3 direction = player.position - transform.position;

        // Prevent enemy from tilting up/down
        direction.y = 0;

        // Only rotate if player is not exactly in the same position
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Smooth rotation
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}