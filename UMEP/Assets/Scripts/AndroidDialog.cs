using UnityEngine;
using UnityEngine.Android;

public class AndroidDialog : MonoBehaviour
{
    // Reference to the current Android activity
    private AndroidJavaObject currentActivity;

    // Reference to the Android AlertDialog that will be displayed
    private AndroidJavaObject alertDialog;

    // Reference to the Android EditText widget within the AlertDialog
    private AndroidJavaObject editText;

    // Constant representing the visibility state 'GONE' for Android views
    private const int ViewGone = 8;

    // Constant representing the visibility state 'VISIBLE' for Android views
    private const int ViewVisible = 0;

    private void Awake()
    {
        InitializeCurrentActivity();
    }

    // Initialize the current Android activity
    private void InitializeCurrentActivity()
    {
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }
    }

    /*
    // Create an Android EditText widget
    private void CreateEditText()
    {
        editText = new AndroidJavaObject("android.widget.EditText", currentActivity);
    }

    // Get the text entered in the EditText
    public string GetEditTextValue()
    {
        if (editText != null)
        {
            // Call the getText method on the EditText to retrieve its text as an Editable
            AndroidJavaObject editable = editText.Call<AndroidJavaObject>("getText");

            // Convert the Editable to a string
            string text = editable.Call<string>("toString");

            Debug.Log($"EditText value: {text}");  // Add this line for debugging

            return text;
        }
        else
        {
            Debug.LogError("EditText is not initialized.");
            return null;
        }
    }
    */

    // Show an Android AlertDialog with specified parameters
    public void ShowAlertDialog(string title, string message, string positiveButton, string negativeButton, System.Action onPositiveButtonClick, System.Action onNegativeButtonClick)
    {
        if (currentActivity != null)
        {
            currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                // Create an Android AlertDialog Builder
                AndroidJavaObject builder = new AndroidJavaObject("android.app.AlertDialog$Builder", currentActivity);

                // Set the title and message of the AlertDialog
                builder.Call<AndroidJavaObject>("setTitle", title);
                builder.Call<AndroidJavaObject>("setMessage", message);

                // Add the EditText to the dialog
                builder.Call<AndroidJavaObject>("setView", editText);

                // Set Positive Button with a custom listener
                SetPositiveButton(builder, positiveButton, onPositiveButtonClick);

                // Set Negative Button with a custom listener
                SetNegativeButton(builder, negativeButton, onNegativeButtonClick);

                // Disable canceling the dialog when touching outside
                builder.Call<AndroidJavaObject>("setCancelable", false);

                // Create and show the AlertDialog
                alertDialog = builder.Call<AndroidJavaObject>("create");
                alertDialog.Call("show");
            }));
        }
        else
        {
            Debug.LogError("Failed to get the current Android activity.");
        }
    }

    // Set Positive Button with a custom listener
    private void SetPositiveButton(AndroidJavaObject builder, string positiveButton, System.Action onPositiveButtonClick)
    {
        if (!string.IsNullOrEmpty(positiveButton))
        {
            builder.Call<AndroidJavaObject>("setPositiveButton", positiveButton, new ButtonClickListener(onPositiveButtonClick));
        }
    }

    // Set Negative Button with a custom listener
    private void SetNegativeButton(AndroidJavaObject builder, string negativeButton, System.Action onNegativeButtonClick)
    {
        if (!string.IsNullOrEmpty(negativeButton))
        {
            builder.Call<AndroidJavaObject>("setNegativeButton", negativeButton, new ButtonClickListener(onNegativeButtonClick));
        }
    }

    /*
    // Set the visibility of the EditText to be visible
    public void ShowEditText()
    {
        SetEditTextVisibility(ViewVisible);
    }

    // Set the visibility of the EditText to be gone
    public void HideEditText()
    {
        SetEditTextVisibility(ViewGone);
    }

    // Set the visibility of the EditText widget
    private void SetEditTextVisibility(int visibility)
    {
        if (editText != null)
        {
            editText.Call("setVisibility", visibility);
            Debug.Log($"EditText visibility set to: {visibility}");
        }
    }
    */

    // Dismiss the AlertDialog
    public void CloseAlertDialog()
    {
        if (alertDialog != null)
        {
            alertDialog.Call("dismiss");
        }
    }

    // Custom listener for the Buttons
    private class ButtonClickListener : AndroidJavaProxy
    {
        private System.Action callback;

        public ButtonClickListener(System.Action callback) : base("android.content.DialogInterface$OnClickListener")
        {
            this.callback = callback;
        }

        // Invoke the callback when the button is clicked
        public void onClick(AndroidJavaObject dialog, int which)
        {
            if (callback != null)
            {
                callback.Invoke();
            }
        }
    }
}
