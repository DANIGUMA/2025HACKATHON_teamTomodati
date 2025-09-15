using Firebase.Auth;
using Firebase.Firestore;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChatSceneManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform messageContainer; // ScrollView��Content
    [SerializeField] private GameObject myMessagePrefab;   // ���b�Z�[�W�p��Prefab
    [SerializeField] private GameObject otherMessagePrefab;
    [SerializeField] private TextMeshProUGUI inputField;      // ���M�p�e�L�X�g�{�b�N�X
    [SerializeField] private TextMeshProUGUI titlename;      //���[�����\��
    [SerializeField] private TextMeshProUGUI anonymustext;  //�������ǂ���
    [Header("Canvas")]
    [SerializeField] GameObject settingCanvas;
    [SerializeField] TextMeshProUGUI userId;
    private bool is_anonymus = false;
    private bool isSettingOpen = false;


    void Start()
    {
        settingCanvas.SetActive(false);
        ChatManagerBase.Instance.OnMessageReceived += AddMessageToUI;
        // ���������A���^�C���Ŏ�M
        //���[���I����ʂŒ��ڎ��s�ł��邽�߁A�������̂ق����ǂ�
        //ChatManagerBase.Instance.StartListenMessages();
        titlename.text = ChatManagerBase.Instance.ActiveRoomName;
        anonymustext.text = $"NowUserState:{AuthManagerBase.Instance.CurrentUserName}";
    }
    

    void OnDestroy()
    {
        ChatManagerBase.Instance.OnMessageReceived -= AddMessageToUI;
        ChatManagerBase.Instance.StopLister();
    }

    private void AddMessageToUI(string message,string username,string timestamp)
    {
        GameObject prefab = (username == AuthManagerBase.Instance.CurrentUserName) ? myMessagePrefab : otherMessagePrefab;
        GameObject msgObj = Instantiate(prefab, messageContainer);
        var textComp = msgObj.GetComponentInChildren<TextMeshProUGUI>();
        textComp.text = $"[{username}]:{message}\n{timestamp}";
        Debug.Log($"message:{message}\nuser:{username}\n{timestamp}");
    }
    public void OnSendButtonClick()
    {
        if (!is_anonymus)
        {
            ChatManagerBase.Instance.SendMessage(inputField.text);
        }
        else
        {
            ChatManagerBase.Instance.AnonymusSendMessage(inputField.text);
        }
        
        inputField.text = "";
    }
    public void OnBackToRoomsButtonClick()
    {
        SceneManager.LoadScene("RecoChat");
    }
    public void OnSwitchButtonClick()
    {
        is_anonymus = !is_anonymus;
        if (is_anonymus)
        {
            anonymustext.text = "NowUserState:Anonymus!";
        }
        else
        {
            anonymustext.text = $"NowUserState:{AuthManagerBase.Instance.CurrentUserName}";
        }
    }
    public void OnSettingButtonClick()
    {
        isSettingOpen = !isSettingOpen;
        if (isSettingOpen)
        {
            settingCanvas.SetActive(true);
        }
        else
        {
            settingCanvas.SetActive(false);
        }
    }
    public void OInviteButtonClick()
    {
        ChatRoomsManagerBase.Instance.AddUserToRoom(userId.text,ChatManagerBase.Instance.ActiveRoomID);
        userId.text = "";
    }

}
