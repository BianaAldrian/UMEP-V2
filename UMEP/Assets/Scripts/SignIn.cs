using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;

public class SignIn : MonoBehaviour
{
    private string IP;
    public Canvas canvas;
    public GameObject popupPrefab;

    public TMP_InputField id_number;
    public TMP_InputField password;
    public Button signIn;
    public string next_scene;

    void Start()
    {
        IP = PlayerPrefs.GetString("IP");
        // Assign the event listeners for input fields
        id_number.onValueChanged.AddListener(ResetInputFieldColor);
        password.onValueChanged.AddListener(ResetInputFieldColor);

        signIn.onClick.AddListener(CheckInputs);
    }

    void CheckInputs()
    {
        // Check if ID Number is empty
        if (string.IsNullOrEmpty(id_number.text))
        {
            // Change the text and color of the input field
            id_number.text = "ID Number cannot be empty!";
            id_number.textComponent.color = Color.red;

            return; // Stop further processing as ID Number is required
        }

        // Check if Password is empty
        if (string.IsNullOrEmpty(password.text))
        {
            // Change the text and color of the input field
            password.text = "Password cannot be empty!";
            password.textComponent.color = Color.red;
            return; // Stop further processing as Password is required
        }

        StartCoroutine(Signin());
    }

    IEnumerator Signin()
    {
        // Trim whitespace from the input fields before sending them
        string idNumber = id_number.text.Trim();
        string userPassword = password.text.Trim();

        WWWForm form = new WWWForm();
        form.AddField("id_number", idNumber);
        form.AddField("password", userPassword);

        // Check if the IP address is valid before creating the UnityWebRequest
        if (!string.IsNullOrEmpty(IP))
        {
            UnityWebRequest www = UnityWebRequest.Post($"http://{IP}/UMEP/Sign_in.php", form);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"HTTP request failed. Error: {www.error}");
                Debug.LogWarning($"Server IP: {IP}");

                ShowPopup("Connection Error " + www.error);

                // Handle the error, display a message, or take appropriate action
            }
            else
            {
                string response = www.downloadHandler.text;
                Debug.Log($"Server response: {response}");

                if (response == "!exist")
                {
                    // Change the text and color of the input field
                    id_number.text = "User does not exist!";
                    id_number.textComponent.color = Color.red;
                }
                else if (response == "invalid_password")
                {
                    // Change the text and color of the input field
                    password.text = "Invalid password!";
                    password.textComponent.color = Color.red;
                }
                else if (response == "valid_password")
                {
                    // Password is valid, you can proceed with your logic here
                    Debug.Log("Login successful!");
                    PlayerPrefs.SetString("id_number", idNumber);
                    PlayerPrefs.Save();

                    // Use SceneManager to load the next scene
                    SceneManager.LoadScene(next_scene);
                }
            }
        }
        else
        {
            Debug.LogWarning("IP address is not set.");
            ShowPopup("IP address is not set.");
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

    void ResetInputFieldColor(string value)
    {
        // Reset the color of the input field to its default color
        id_number.textComponent.color = Color.black; // Change to your default color
        password.textComponent.color = Color.black; // Change to your default color
    }
}
