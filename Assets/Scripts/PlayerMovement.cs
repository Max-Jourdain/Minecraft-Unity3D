using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float transitionSpeed = 1.0f; // Speed of the transition
    private bool canMove = true;
    private Vector3 targetPosition;
    public float movementDistance = 1.0f; // Fixed movement distance

    void Start()
    {
        targetPosition = transform.position; // Initialize target position
    }

    void Update()
    {
        // Check if the player has reached the target position
        if (transform.position == targetPosition)
        {
            canMove = true;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && canMove)
        {
            // Set the new target position
            targetPosition = transform.position + Vector3.forward * movementDistance;
            targetPosition.x = Mathf.Round(targetPosition.x); // Round the x position to ensure it's an integer

            canMove = false;
        }

        // Smoothly move the player towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, transitionSpeed * Time.deltaTime);
    }
}
