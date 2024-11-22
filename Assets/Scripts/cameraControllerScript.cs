using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // The character to follow
    public Vector3 offset; // Offset relative to the character's rotation
    public float smoothSpeed = 0.125f; // Speed of the camera movement
    public float rotationSpeed = 5f; // Speed of the camera rotation

    private void LateUpdate()
    {
        if (target != null)
        {
            // Calculate the desired position behind the character
            Vector3 desiredPosition = target.position + target.TransformDirection(offset);

            // Smoothly interpolate to the desired position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;

            // Make the camera look in the same direction as the character
            Quaternion desiredRotation = Quaternion.LookRotation(target.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
