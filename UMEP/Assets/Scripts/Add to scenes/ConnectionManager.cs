using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class ConnectionManager : MonoBehaviour
{
    public static string IP = "192.168.213.109"; // Make IP static
    public GameObject popupPrefab;
    public Canvas canvas; // Reference to the canvas
    public LoadingManager loadingManager; // Reference to the other script

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CheckConnection());
    }

    IEnumerator CheckConnection()
    {
        UnityWebRequest www = UnityWebRequest.Get($"http://{IP}/UMEP/Check_conn.php");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            ShowPopup("Connection Error " + www.error);
            Debug.Log(www.error);
        }

        else
        {
            Debug.Log(www.downloadHandler.text);
            if (www.downloadHandler.text == "connected")
            {
                // Connection successful
                PlayerPrefs.SetString("IP", IP);
                PlayerPrefs.Save();

                // Enable the loadingManager script
                loadingManager.enabled = true;

            }
            else
            {
                ShowPopup("Received unexpected response from server.");
            }
        }
    }

    void ShowPopup(string message)
    {
        // Instantiate the prefab
        GameObject popup = Instantiate(popupPrefab, canvas.transform);

        // Get the TextMeshPro component in the popup
        TextMeshProUGUI textMeshPro = popup.GetComponentInChildren<TextMeshProUGUI>();

        if (textMeshPro != null)
        {
            // Set the text content
            textMeshPro.text = message;
        }
        else
        {
            Debug.LogError("TextMeshPro component not found in the prefab.");
        }
    }
}
