using UnityEngine;

public class DialogManager : MonoBehaviour
{
    // Singleton instance
    // Singleton instance
    private static DialogManager instance;

    // Property to access the instance
    public static DialogManager Instance
    {
        get
        {
            if (instance == null)
            {
                // Attempt to find an existing instance in the scene
                instance = FindObjectOfType<DialogManager>();

                // If no instance exists, create a new one
                if (instance == null)
                {
                    // Create a new GameObject for the DialogManager
                    GameObject obj = new GameObject("DialogManager");
                    instance = obj.AddComponent<DialogManager>();
                }
            }
            return instance;
        }
    }

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // Ensure there's only one instance of DialogManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keep the instance alive between scenes
        }
        else
        {
            Destroy(gameObject); // If an instance already exists, destroy this one
        }
    }


    // Function to show a dialog with a message
    public void ShowDialog(string title, string message)
    {
        // Check if the current platform is Android
        if (Application.platform == RuntimePlatform.Android)
        {
            // Create an instance of AndroidJavaClass for UnityPlayer
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                // Get the current activity
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                // Check if the current activity is not null
                if (currentActivity != null)
                {
                    // Create an instance of AlertDialog.Builder
                    AndroidJavaObject alertDialogBuilder = new AndroidJavaObject("android.app.AlertDialog$Builder", currentActivity);

                    // Set the title and message of the dialog
                    alertDialogBuilder.Call<AndroidJavaObject>("setTitle", title);
                    alertDialogBuilder.Call<AndroidJavaObject>("setMessage", message);

                    // Create the dialog
                    AndroidJavaObject alertDialog = alertDialogBuilder.Call<AndroidJavaObject>("create");

                    // Show the dialog
                    alertDialog.Call("show");
                }
                else
                {
                    Debug.LogError("Failed to get current activity.");
                }
            }
        }
        else
        {
            Debug.LogWarning("Dialogs are only supported on Android platform.");
        }
    }
}
