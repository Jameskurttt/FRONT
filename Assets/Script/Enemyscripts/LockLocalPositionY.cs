using UnityEngine;

public class LockLocalPositionY : MonoBehaviour
{
    private float startLocalY;

    private void Awake()
    {
        startLocalY = transform.localPosition.y;
    }

    private void LateUpdate()
    {
        Vector3 pos = transform.localPosition;
        pos.y = startLocalY;
        transform.localPosition = pos;
    }
}