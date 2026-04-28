using UnityEngine;

public class Pickup : MonoBehaviour
{
    public string pickupName = "Pickup";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PickUp(other.gameObject);
        }
    }

    protected virtual void PickUp(GameObject player)
    {
        Debug.Log("Picked up: " + pickupName);
        Destroy(gameObject);
    }
}