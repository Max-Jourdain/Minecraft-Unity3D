using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float transitionSpeed = 1.0f;
    public float movementDistance = 1.0f;
    private Vector3 targetPosition;
    private Vector3 originalPosition;

    void Start()
    {
        targetPosition = transform.position;
        originalPosition = transform.position;
    }

    public void ResetPlayerPosition()
    {
        transform.position = originalPosition;
        targetPosition = originalPosition;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)) targetPosition += transform.forward * movementDistance; // Move Forward
        if (Input.GetKeyDown(KeyCode.S)) targetPosition -= transform.forward * movementDistance; // Move Backward

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, transitionSpeed * Time.deltaTime);
    }

    public void MoveForward()
    {
        targetPosition += transform.forward * movementDistance;
    }

    public void MoveBackward()
    {
        targetPosition -= transform.forward * movementDistance;
    }
}
