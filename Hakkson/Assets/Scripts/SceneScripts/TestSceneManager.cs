using TMPro;
using UnityEngine;

public class TestSceneManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI emailTextMeshPro;
    [SerializeField] TextMeshProUGUI passwaordTextMeshPro;
    [SerializeField] TextMeshProUGUI userNameTextMeshPro;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Start()
    {

    }
    public void OnSignUpButtonClick()
    {
        if (emailTextMeshPro == null || passwaordTextMeshPro == null)
        {
            Debug.LogError("NullReferenceError:TextMeshProUGUI is not defined");
            return;
        }
        string email = emailTextMeshPro.text;
        string pass = passwaordTextMeshPro.text;
        string userName = userNameTextMeshPro.text;
        if (email != "" && pass != "")
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
        if (emailTextMeshPro == null || passwaordTextMeshPro == null)
        {
            Debug.LogError("NullReferenceError:TextMeshProUGUI is not defined");
            return;
        }
        string email = emailTextMeshPro.text;
        string pass = passwaordTextMeshPro.text;
        if (email != "" && pass != "")
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
    }
    public void OnSignOutButtonClick()
    {
        AuthManagerBase.Instance.SignOut();
        Debug.Log("SiginOut Success!");
    }
}
