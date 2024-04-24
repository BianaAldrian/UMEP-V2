using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControll : MonoBehaviour
{
    private float rotationSpeed = 8.0f;
    private float zoomSpeed = 5.0f;
    private float swipeSpeed = 4.0f;
    private bool isDragging = false;
    private Vector2 lastDragPosition;

    // Update is called once per frame
    void Update()
    {
        // Check for touch input
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
        // Check for pinch gesture for zooming and moving closer/farther
        else if (Input.touchCount == 2)
        {
            // Get the first and second touch
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            // Calculate previous and current positions of the touches
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
            Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;

            // Calculate the magnitude of the vector between the touches in the previous frame
            float prevTouchDeltaMag = (touch1PrevPos - touch2PrevPos).magnitude;

            // Calculate the magnitude of the vector between the touches in the current frame
            float touchDeltaMag = (touch1.position - touch2.position).magnitude;

            // Calculate the difference in the distances between each frame
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // Adjust the camera's field of view for zooming
            Camera.main.fieldOfView += deltaMagnitudeDiff * zoomSpeed * Time.deltaTime;

            // Clamp the field of view to ensure it stays within reasonable bounds
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 10f, 80f);

            // Move the camera closer or farther along its forward axis
            transform.Translate(Vector3.forward * deltaMagnitudeDiff * zoomSpeed * Time.deltaTime);
        }
        // Check for swipe gesture for panning
        else if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            // Calculate swipe delta between current and previous touch positions
            Vector2 swipeDelta = touch1.deltaPosition + touch2.deltaPosition;

            // Calculate camera movement based on swipe direction
            float swipeX = swipeDelta.x * swipeSpeed * Time.deltaTime;
            float swipeY = swipeDelta.y * swipeSpeed * Time.deltaTime;

            // Translate the camera accordingly
            transform.Translate(Vector3.right * swipeX, Space.World);
            transform.Translate(Vector3.up * swipeY, Space.World);
        }
    }
}
