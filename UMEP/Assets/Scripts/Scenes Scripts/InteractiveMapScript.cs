using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractiveMapScript : MonoBehaviour
{
    public RectTransform NavDrawer;
    public float slideDuration = 0.5f; // Adjust this value to make the animation faster
    public AnimationCurve slideCurve;

    private bool isSlideIn = false;

    private Vector2 initialPosition;
    private Vector2 targetPosition;
    private Vector2 touchStartPos; // Position where touch/swipe started
    private bool isDragging = false; // Flag to indicate if dragging/swiping is in progress
    private float slideTimer = 0f;

    void Start()
    {
        initialPosition = NavDrawer.anchoredPosition;
        targetPosition = initialPosition; // Ensure initial and target positions are the same initially
    }

    void Update()
    {
        //<------------------------------ Start of navigation drawer function ------------------------------>//
        // Check for swipe input
        /*if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    isDragging = true;
                    touchStartPos = touch.position;
                    break;
                case TouchPhase.Ended:
                    if (isDragging)
                    {
                        Vector2 touchEndPos = touch.position;
                        float deltaX = touchEndPos.x - touchStartPos.x;
                        if (Mathf.Abs(deltaX) > Screen.width * 0.1f) // Adjust threshold as needed
                        {
                            if (deltaX > 0) // Swipe right
                            {
                                Debug.Log("Swiped right");
                                // Do something when swiped right

                                SlideIn();
                            }
                            else // Swipe left
                            {
                                Debug.Log("Swiped left");
                                // Do something when swiped left

                                SlideOut();
                            }
                        }
                    }
                    isDragging = false;
                    break;
                case TouchPhase.Canceled:
                    isDragging = false;
                    break;
            }
        }*/

        if (isSlideIn)
        {
            slideTimer += Time.deltaTime;
            float slideProgress = slideTimer / slideDuration;
            float curveProgress = slideCurve.Evaluate(slideProgress);
            NavDrawer.anchoredPosition = Vector2.Lerp(initialPosition, targetPosition, curveProgress);

            if (slideProgress >= 1f)
            {
                isSlideIn = false;
            }
        }
        //<------------------------------ End of navigation drawer function ------------------------------>//
    }

    public void SlideIn()
    {
        targetPosition = new Vector2(-146.55f, 0f);
        slideTimer = 0f;
        isSlideIn = true;
    }

    private void SlideOut()
    {
        targetPosition = new Vector2(-853f, 0f);
        slideTimer = 10f;
        isSlideIn = true;
    }

    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
