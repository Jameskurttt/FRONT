using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform player;

    public float distance = 3.5f;
    public float sideOffset = 1.0f;
    public float height = 1.7f;

    public float mouseSensitivity = 140f;
    public float minPitch = -15f;
    public float maxPitch = 45f;

    float yaw;
    float pitch;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (player != null)
            yaw = player.eulerAngles.y;
    }

    void LateUpdate()
    {
        if (player == null) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 targetPoint = player.position + Vector3.up * height;

        Vector3 wantedPosition =
            targetPoint
            - rotation * Vector3.forward * distance
            + rotation * Vector3.right * sideOffset;

        //  NO SMOOTH = NO SHAKE
        transform.position = wantedPosition;

        transform.rotation = rotation;
    }
}