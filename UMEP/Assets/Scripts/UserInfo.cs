using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class UserInfo : MonoBehaviour
{
    private string IP;
    private string playerId_number;

    [SerializeField] private TextMeshProUGUI Name;
    [SerializeField] private TextMeshProUGUI id_number;
    [SerializeField] private TextMeshProUGUI email;

    // Start is called before the first frame update
    void Start()
    {
        IP = PlayerPrefs.GetString("IP");
        playerId_number = PlayerPrefs.GetString("id_number");
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(GetUsersInfo());
    }

    IEnumerator GetUsersInfo()
    {
        // Send a GET request to your PHP script
        UnityWebRequest webRequest = UnityWebRequest.Get($"http://{IP}/UMEP/User_info.php");
        // Send the request and wait for the response
        yield return webRequest.SendWebRequest();

        // Check for errors
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + webRequest.error);
        }
        else
        {
            // Log the response
            //Debug.Log("Received: " + webRequest.downloadHandler.text);

            // Parse the JSON data
            ParseUsersInfoData(webRequest.downloadHandler.text);
        }
    }

    void ParseUsersInfoData(string jsonData)
    {
        // Deserialize the JSON data into a list of LocationData objects
        UsersDataArray locationDataArray = JsonUtility.FromJson<UsersDataArray>("{\"Users_data\":" + jsonData + "}");

        foreach (UsersData User_data in locationDataArray.Users_data)
        {
            if (Name != null)
            {
                Name.text = User_data.first_name + " " + User_data.last_name;
            }
            if (id_number != null)
            {
                id_number.text = User_data.id_number;
            }

            string myString = User_data.first_name;
            char firstLetter = myString[0];
            string user_email = (firstLetter.ToString().ToLower()) + User_data.last_name.ToLower() + "." + User_data.id_number + "@umak.edu.ph";
            //Debug.Log("First letter: " + firstLetter);

            if (email != null)
            {
                email.text = user_email;
            }

        }
    }

    [System.Serializable]
    public class UsersData
    {
        public int id;
        public string first_name;
        public string last_name;
        public string id_number;
    }

    [System.Serializable]
    private class UsersDataArray
    {
        public UsersData[] Users_data;
    }
}
