using UnityEngine;

public class TouchInputManager : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;

    private Vector2 startTouchPosition;
    private Vector2 currentSwipe;

    private float rotationSpeed = 2.0f;
    private float zoomSpeed = 5.0f;
    private float swipeSpeed = 1.0f;
    private bool isDragging = false;
    private Vector2 lastDragPosition;

    void Update()
    {
        // Check for touch input
        // For single finger
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            // Check if touch has just begun
            if (touch.phase == TouchPhase.Began)
            {
                // Store the initial touch position
                lastDragPosition = touch.position;
                isDragging = true;
            }
            // Check if touch is moving
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                // Calculate the delta movement since the last frame
                Vector2 delta = touch.position - lastDragPosition;

                // Calculate rotation angles based on touch delta
                float verticalInput = -delta.y * rotationSpeed * Time.deltaTime;
                float horizontalInput = delta.x * rotationSpeed * Time.deltaTime;

                // Rotate the camera accordingly
                transform.Rotate(Vector3.right, verticalInput);
                transform.Rotate(Vector3.up, horizontalInput, Space.World);

                // Update the last drag position for the next frame
                lastDragPosition = touch.position;
            }
            // Check if touch has ended
            else if (touch.phase == TouchPhase.Ended)
            {
                // Reset dragging state
                isDragging = false;
            }
        }

        // for two fingers
        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            if (touch1.phase == TouchPhase.Moved && touch2.phase == TouchPhase.Moved)
            {
                // Check if both touches are moving in the same direction
                Vector2 touch1Delta = touch1.deltaPosition.normalized;
                Vector2 touch2Delta = touch2.deltaPosition.normalized;

                // swiped two fingers
                if (Vector2.Dot(touch1Delta, touch2Delta) > 0.7f) // Using dot product for direction similarity
                {
                    // Calculate swipe delta between current and previous touch positions
                    Vector2 swipeDelta = touch1.deltaPosition + touch2.deltaPosition;

                    // Calculate camera movement based on swipe direction
                    float swipeX = swipeDelta.x * swipeSpeed * Time.deltaTime;
                    float swipeY = swipeDelta.y * swipeSpeed * Time.deltaTime;

                    // Translate the camera accordingly
                    transform.Translate(Vector3.right * swipeX, Space.World);
                    transform.Translate(Vector3.up * swipeY, Space.World);
                } 

                // pinched
                else
                {
                    // Calculate previous and current positions of the touches
                    Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
                    Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;

                    // Calculate the magnitude of the vector between the touches in the previous frame
                    float prevTouchDeltaMag = (touch1PrevPos - touch2PrevPos).magnitude;

                    // Calculate the magnitude of the vector between the touches in the current frame
                    float touchDeltaMag = (touch1.position - touch2.position).magnitude;

                    // Calculate the difference in the distances between each frame
                    float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                    // Move the camera closer or farther along its forward axis
                    _camera.transform.Translate(Vector3.forward * deltaMagnitudeDiff * zoomSpeed * Time.deltaTime);
                }
            }
        }
    }
}