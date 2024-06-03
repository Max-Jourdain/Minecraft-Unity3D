using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float transitionSpeed = 1.0f;
    private bool canMove = true;
    private Vector3 targetPosition;
    public float movementDistance = 1.0f;
    private GameObject chunks;
    public float maxZ = 999.0f;

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        if (transform.position == targetPosition)
        {
            canMove = true;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && canMove)
        {
            targetPosition = transform.position + Vector3.forward * movementDistance;
            canMove = false;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, transitionSpeed * Time.deltaTime);

        // Check if out of bounds
        if (IsOutOfBounds(transform.position))
        {
            ResetPosition();
        }
    }

    bool IsOutOfBounds(Vector3 position)
    {
        return position.z > maxZ;
    }

    void ResetPosition()
    {
        targetPosition = new Vector3(0, 0, 0);
        transform.position = targetPosition;
        canMove = true;

        // Reset chunks position
        if (chunks != null)
        {
            chunks.transform.position = new Vector3(0, 0, 0);
        }
    }
}
