using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float transitionSpeed = 1.0f;
    private bool canMove = true;
    private Vector3 targetPosition;
    public float movementDistance = 1.0f;

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
    }
}
