using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoginPageController : MonoBehaviour
{
    private string IP;
    public Canvas canvas;
    public GameObject popupPrefab;

    public TMP_InputField id_number;
    public TMP_InputField password;

    public TMP_InputField first_name;
    public TMP_InputField last_name;
    public TMP_InputField id_number1;
    public TMP_InputField password1;
    public TMP_InputField phone;
    public TMP_Dropdown college; // Modified to TMP_Dropdown

    public string next_scene;
    public GameObject LoginPanel;
    public GameObject RegistrationPanel;

    void Start()
    {
        IP = PlayerPrefs.GetString("IP");
        AssignResetInputFieldColorListeners();
    }

    void AssignResetInputFieldColorListeners()
    {
        // Create an array of TMP_InputField references
        TMP_InputField[] inputFields = { id_number, password, first_name, last_name, id_number1, password1, phone };

        // Assign the event listeners for input fields
        foreach (TMP_InputField inputField in inputFields)
        {
            if (inputField != null) // Ensure the inputField is not null
            {
                inputField.onValueChanged.AddListener(delegate { ResetInputFieldColor(inputField); });
            }
        }

        // Assign listener for dropdown
        if (college != null)
        {
            college.onValueChanged.AddListener(delegate { ResetDropdownColor(college); });
        }
    }


    void ResetInputFieldColor(TMP_InputField inputField)
    {
        // Reset the color of the input field to its default color
        if (inputField != null)
        {
            inputField.textComponent.color = Color.black; // Change to your default color
        }
    }


    void ResetDropdownColor(TMP_Dropdown dropdown)
    {
        // Reset the color of the dropdown label to its default color
        TMP_Text dropdownLabel = dropdown.GetComponentInChildren<TMP_Text>();
        dropdownLabel.color = Color.black; // Change to your default color
    }

    public void OpenRegistrationPanel()
    {
        RegistrationPanel.SetActive(true);
        LoginPanel.SetActive(false);
    }

    public void OpenLoginPanel()
    {
        LoginPanel.SetActive(true);
        RegistrationPanel.SetActive(false);
    }

    public void CheckSignInInputs()
    {
        TMP_InputField[] inputFields = { id_number, password};
        TMP_Dropdown dropdown = college; // Already a TMP_Dropdown

        foreach (TMP_InputField inputField in inputFields)
        {
            if (inputField != null && string.IsNullOrEmpty(inputField.text))
            {
                // Change the text and color of the input field
                inputField.text = inputField.placeholder.GetComponent<TextMeshProUGUI>().text + " cannot be empty!";
                inputField.textComponent.color = Color.red;
                return; // Stop further processing if any field is empty
            }
        }

        StartCoroutine(Signin());
    }

    public void CheckSignUpInputs()
    {
        TMP_InputField[] inputFields = {first_name, last_name, id_number1, password1, phone };
        TMP_Dropdown dropdown = college; // Already a TMP_Dropdown

        foreach (TMP_InputField inputField in inputFields)
        {
            if (inputField != null && string.IsNullOrEmpty(inputField.text))
            {
                // Change the text and color of the input field
                inputField.text = inputField.placeholder.GetComponent<TextMeshProUGUI>().text + " cannot be empty!";
                inputField.textComponent.color = Color.red;
                return; // Stop further processing if any field is empty
            }
        }

        // Validate the dropdown
        if (dropdown != null && dropdown.value == 0) // Assuming the first option in dropdown is empty
        {
            // Change the color of the dropdown label to indicate error
            TMP_Text dropdownLabel = dropdown.GetComponentInChildren<TMP_Text>();
            dropdownLabel.color = Color.red;
            return; // Stop further processing if the dropdown is not selected
        }

        StartCoroutine(SignUp());
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

    IEnumerator SignUp()
    {
        // Trim whitespace from the input fields before sending them
        string idNumber = id_number1.text.Trim();
        string userPassword = password1.text.Trim();
        string firstName = first_name.text.Trim();
        string lastName = last_name.text.Trim();
        string userPhone = phone.text.Trim();
        string userCollege = college.options[college.value].text; // Get the selected college text

        WWWForm form = new WWWForm();
        form.AddField("id_number", idNumber);
        form.AddField("password", userPassword);
        form.AddField("first_name", firstName);
        form.AddField("last_name", lastName);
        form.AddField("phone", userPhone);
        form.AddField("college", userCollege);

        // Check if the IP address is valid before creating the UnityWebRequest
        if (!string.IsNullOrEmpty(IP))
        {
            UnityWebRequest www = UnityWebRequest.Post($"http://{IP}/UMEP/Sign_up.php", form);
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

                if (response == "user_exists")
                {
                    // User already exists, show appropriate message to the user
                    Debug.Log("User already exists.");
                    // You may want to display an error message to the user

                    ShowPopup("User already exists.");
                }
                else if (response == "success")
                {
                    // Sign-up successful
                    Debug.Log("Sign-up successful!");
                    // You may want to perform additional actions here if needed

                    // Use SceneManager to load the next scene
                    SceneManager.LoadScene(next_scene);
                }
                else
                {
                    // Handle other responses (e.g., error occurred during sign-up)
                    // Show appropriate error message to the user
                    Debug.LogError("Error occurred during sign-up.");
                    // You may want to display an error message to the user
                    ShowPopup("Error occurred during sign-up.");
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
}
