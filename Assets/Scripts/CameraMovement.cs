using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotateSpeed = 90f; // degrees per second

    void Update()
    {
        // ===== Movement (WASD) =====
        float moveX = Input.GetAxis("Horizontal"); // A/D
        float moveZ = Input.GetAxis("Vertical");   // W/S

        Vector3 move =
            transform.right * moveX +
            transform.forward * moveZ;

        transform.position += move * moveSpeed * Time.deltaTime;

        // ===== Rotation (Arrow Keys) =====
        float yaw = 0f;
        float pitch = 0f;

        if (Input.GetKey(KeyCode.LeftArrow))  yaw -= 1f;
        if (Input.GetKey(KeyCode.RightArrow)) yaw += 1f;
        if (Input.GetKey(KeyCode.UpArrow))    pitch -= 1f;
        if (Input.GetKey(KeyCode.DownArrow))  pitch += 1f;

        Vector3 rotation = new Vector3(
            pitch * rotateSpeed * Time.deltaTime,
            yaw   * rotateSpeed * Time.deltaTime,
            0f
        );

        transform.eulerAngles += rotation;
    }
}
