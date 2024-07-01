using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public float transitionSpeed = 1.0f;
    [SerializeField] public float movementDistance = 1.0f;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private Vector3 originalPosition;

    private Vector2 startTouchPosition;
    private Vector2 currentTouchPosition;
    private bool stopTouch = false;
    private float swipeThreshold = 50f; // Minimum distance for a swipe to be registered

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

        SwipeDetection();
    }

    private void SwipeDetection()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
                stopTouch = false;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                currentTouchPosition = touch.position;
                Vector2 distance = currentTouchPosition - startTouchPosition;

                if (!stopTouch)
                {
                    if (distance.y > swipeThreshold)
                    {
                        MoveForward();
                        stopTouch = true;
                    }
                    else if (distance.y < -swipeThreshold)
                    {
                        MoveBackward();
                        stopTouch = true;
                    }
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                stopTouch = false;
                startTouchPosition = Vector2.zero;
                currentTouchPosition = Vector2.zero;
            }
        }
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
