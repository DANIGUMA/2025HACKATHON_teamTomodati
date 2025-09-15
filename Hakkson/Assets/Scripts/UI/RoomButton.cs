using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.EditorTools;
using UnityEngine.SceneManagement;

public class RoomButton : MonoBehaviour
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
        ChatManagerBase.Instance.StartListenMessages(roomId);
        SceneManager.LoadScene("TestChat2Scene");
       
    }
}
