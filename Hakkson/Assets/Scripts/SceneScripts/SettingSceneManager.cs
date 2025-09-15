using UnityEngine;

public class SettingSceneManager : MonoBehaviour
{

    public void OnDeleteUserButton()
    {
        AdministerManagerBase.Instance.DeleteUser((success, message) =>
        {
            Debug.Log(message);
            if (success)
            {
                Debug.Log("Delete Success!");
                AuthManagerBase.Instance.SignOut();
            }
        });
    }
    public void ChangeUserName(string userName)
    {
        AdministerManagerBase.Instance.UserName = userName;
    }

}
