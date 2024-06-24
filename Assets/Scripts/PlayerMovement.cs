using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public float transitionSpeed = 1.0f;
    [SerializeField] public float movementDistance = 1.0f;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private Vector3 originalPosition;

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
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, transitionSpeed * Time.deltaTime);
    }

    public void MoveForward()
    {
        targetPosition += transform.forward * movementDistance;
    }

    public void MoveBackward()
    {
        if (targetPosition.z > 0)
        {
            targetPosition -= transform.forward * movementDistance;
        }
    }
}
