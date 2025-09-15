using TMPro;
using UnityEditor.Callbacks;
using UnityEngine;

public class LoginSceneManager : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] GameObject logInCanvas;
    [SerializeField] GameObject signUpCanvas;
    [Header("Login form")]
    [SerializeField] TextMeshProUGUI emailLogInTextMeshPro;
    [SerializeField] TextMeshProUGUI passwordLogInTextMeshPro;
    [Header("SignUP from")]
    [SerializeField] TextMeshProUGUI emailSignUpTextMeshPro;
    [SerializeField] TextMeshProUGUI passwordSignUpTextMeshPro;
    [SerializeField] TextMeshProUGUI userNameTextMeshPro;
    [Header("Notification")]
    [SerializeField] GameObject notificationObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Start()
    {
        logInCanvas.SetActive(true);
        signUpCanvas.SetActive(false);
    }
    public void OnSignUpButtonClick()
    {
        if (emailSignUpTextMeshPro == null || passwordSignUpTextMeshPro == null)
        {
            Debug.LogError("NullReferenceError:TextMeshProUGUI is not defined");
            return;
        }
        string email = emailSignUpTextMeshPro.text;
        string pass = passwordSignUpTextMeshPro.text;
        string userName = userNameTextMeshPro.text;
        if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(pass))
        {

            AuthManagerBase.Instance.SignUp(email, pass, userName, (success, message) =>
            {
                Debug.Log(message);
                if (success)
                {
                    Debug.Log("SiginUp Success!");
                    Debug.Log($"UserID:{AuthManagerBase.Instance.CurrentUserId}");
                }
            });
        }

    }
    public void OnSignInButtonClick()
    {
        if (emailLogInTextMeshPro == null || passwordLogInTextMeshPro == null)
        {
            Debug.LogError("NullReferenceError:TextMeshProUGUI is not defined");
            return;
        }
        string email = emailLogInTextMeshPro.text;
        string pass = passwordLogInTextMeshPro.text;
        if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(pass))
        {
            AuthManagerBase.Instance.SignIn(email, pass, (success, message) =>
            {
                Debug.Log(message);
                if (success)
                {
                    Debug.Log("SiginIn Success!");
                    Debug.Log($"UserID:{AuthManagerBase.Instance.CurrentUserId}");
                }
            });

        }
        else
        {
            OnOpenNotification();
        }
    }
    public void OnOpenNotification()
    {
        Debug.Log("ONOpenNotification!");
    }
    public void OnResisterButtonClick()
    {
        signUpCanvas.SetActive(true);
        logInCanvas.SetActive(false);
        
    }
    public void OnBackButtonClick()
    {
        logInCanvas.SetActive(true);
        signUpCanvas.SetActive(false);
    }
    
}
