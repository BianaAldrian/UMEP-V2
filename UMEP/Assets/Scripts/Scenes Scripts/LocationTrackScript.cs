using UnityEngine;
using UnityEngine.SceneManagement;

public class LocationTrackScript : MonoBehaviour
{
    public RectTransform NavDrawer;
    public float slideDuration = 0.5f; // Adjust this value to make the animation faster
    public AnimationCurve slideCurve;

    private bool isSlideIn = false;

    private Vector2 initialPosition;
    private Vector2 targetPosition;
    private float slideTimer = 0f;

    void Start()
    {
        initialPosition = NavDrawer.anchoredPosition;
        targetPosition = initialPosition; // Ensure initial and target positions are the same initially
    }

    void Update()
    {
        //<------------------------------ Start of navigation drawer function ------------------------------>//
        if (isSlideIn)
        {
            slideTimer += Time.deltaTime;
            float slideProgress = slideTimer / slideDuration;
            float curveProgress = slideCurve.Evaluate(slideProgress);
            NavDrawer.anchoredPosition = Vector2.Lerp(initialPosition, targetPosition, curveProgress);

            if (slideProgress >= 1f)
            {
                // Sliding animation is complete
                // No need to set isSlideIn here, as it should remain true while the navigation drawer is in the slide-in state
            }
        }
        else
        {
            slideTimer += Time.deltaTime;
            float slideProgress = slideTimer / slideDuration;
            float curveProgress = slideCurve.Evaluate(slideProgress);
            NavDrawer.anchoredPosition = Vector2.Lerp(targetPosition, initialPosition, curveProgress);

            if (slideProgress >= 1f)
            {
                // Sliding animation is complete
                // Set isSlideIn to false as the navigation drawer is now in the slide-out state
                isSlideIn = false;
            }
        }
        //<------------------------------ End of navigation drawer function ------------------------------>//
    }


    public void Slide()
    {
        if (!isSlideIn)
        {
            // Slide the navigation drawer in
            Debug.Log("Sliding in...");
            targetPosition = new Vector2(-283f, 0f);
            slideTimer = 0f;
            isSlideIn = true;
        }
        else
        {
            // Slide the navigation drawer out
            Debug.Log("Sliding out...");
            targetPosition = initialPosition;
            slideTimer = 0f;
            isSlideIn = false;
        }
    }
    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
