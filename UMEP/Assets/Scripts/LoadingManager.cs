using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public Image firstImage;
    public Image secondImage;
    public Image thirdImage;
    public float fadeDuration;
    public string sceneName;

    void Start()
    {
        StartCoroutine(TransitionImages());
    }

    private IEnumerator TransitionImages()
    {
        yield return new WaitForSeconds(1);

        if (firstImage != null && secondImage != null && thirdImage != null)
        {
            // Fade out first and second images
            for (float t = 0; t < 1; t += Time.deltaTime / fadeDuration)
            {
                firstImage.color = new Color(1, 1, 1, Mathf.Lerp(1, 0, t));
                secondImage.color = new Color(1, 1, 1, Mathf.Lerp(1, 0, t));
                yield return null;
            }

            firstImage.gameObject.SetActive(false);
            secondImage.gameObject.SetActive(false);

            // Activate and fade in third image
            thirdImage.gameObject.SetActive(true);
            for (float t = 0; t < 1; t += Time.deltaTime / fadeDuration)
            {
                thirdImage.color = new Color(1, 1, 1, Mathf.Lerp(0, 1, t));
                yield return null;
            }

            // Fade out third image
            for (float t = 0; t < 1; t += Time.deltaTime / fadeDuration)
            {
                thirdImage.color = new Color(1, 1, 1, Mathf.Lerp(1, 0, t));
                yield return null;
            }

            // Load the specified scene
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Image references not set in the editor.");
        }
    }
}
