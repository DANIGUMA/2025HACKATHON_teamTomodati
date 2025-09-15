using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.EditorTools;
using UnityEngine.SceneManagement;

public class RoomJoinButton : MonoBehaviour
{
    [SerializeField] private TMP_Text roomNameText;
    private string roomId;

    public void Setup(string id, string name)
    {
        roomId = id;
        roomNameText.text = name;
    }

    public void OnClick()
    {
        // ���[��ID��ۑ����ăV�[���J��
        //Listener���N��
        ChatRoomsManagerBase.Instance.JoinRoomsAsync(AuthManagerBase.Instance.CurrentUserId,roomId);
        ChatManagerBase.Instance.StartListenMessages(roomId);
        SceneManager.LoadScene("TestChat2Scene");

    }
}
