using Firebase.Auth;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.XR.OpenVR;
using UnityEngine;
using UnityEngine.UI;

public partial class ChatManager : ChatManagerBase
{
    private FirebaseFirestore db;
    private FirebaseUser user;
    //チE�Eタ更新を見るめE��
    private ListenerRegistration listener;
    private string roomId = "";
    private string roomName = "";
    public override event Action<string,string,string> OnMessageReceived;
    public override string ActiveRoomName 
    {
        get => roomName; 
        set 
        {
            //�^�X�N�𓊂����ςȂ��A�I���񍐂Ȃ�.
            Task.Run(async () => await db.Collection("chatRooms").Document(roomId).UpdateAsync("roomname", value));
            Debug.Log("Firestore ��  roomname ���X�V���܂���");
        }

    }
    public override string ActiveRoomID => roomId;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        Instance = new ChatManager();
        Debug.Log("ChatManager(Firebase)����������");
    }
    private ChatManager()
    {
        db = FirebaseFirestore.DefaultInstance;
        user = FirebaseAuth.DefaultInstance.CurrentUser;
    }
   
    public async override void SendMessage(string message)
    {
        if (string.IsNullOrEmpty(message)) return;
        try
        {
            var msg = new Dictionary<string, object>
            {
                { "senderId", user.UserId },
                { "senderName",user.DisplayName},
                { "message", message },
                { "timestamp", Timestamp.GetCurrentTimestamp().ToString() }
            };

            await db.Collection("chatRooms").Document(roomId)
              .Collection("messages").AddAsync(msg);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        
    }
    public async override void AnonymusSendMessage(string message)
    {
        if (string.IsNullOrEmpty(message)) return;
        try
        {
            var msg = new Dictionary<string, object>
            {
                { "senderId", "null" },
                { "senderName","anonymus"},
                { "message", message },
                { "timestamp", Timestamp.GetCurrentTimestamp().ToString() }
            };

            await db.Collection("chatRooms").Document(roomId)
              .Collection("messages").AddAsync(msg);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }


    }
    public override void SendPicture(string pictureURL)
    {
        throw new NotImplementedException();
    }
    private async Task RoomNameUpdate()
    {
        DocumentSnapshot roomdoc = await db.Collection("chatRoooms").Document(roomId).GetSnapshotAsync();
        if (roomdoc.Exists)
        {
            roomName = roomdoc.GetValue<string>("roomname");
        }
        else
        {
            Debug.Log("Room name is not Exist");
        }
    }
    public async override void StartListenMessages(string _roomId)
    {
        roomId = _roomId;
        await RoomNameUpdate();
        listener = db.Collection("chatRooms").Document(roomId)
            .Collection("messages")
            .OrderBy("timestamp")
            .Listen(async snapshot =>
            {
                foreach (var docChange in snapshot.GetChanges())
                {
                    if (docChange.ChangeType == DocumentChange.Type.Added)
                    {
                        string userID = docChange.Document.GetValue<string>("senderId");
                        string username = await GetUserName(userID);
                        string msg = docChange.Document.GetValue<string>("message");
                        string timestamp = docChange.Document.GetValue<string>("timestamp");
                        Debug.Log($"[Firestore] messageGet�: {msg}");
                        OnMessageReceived?.Invoke(msg,username,timestamp); // UI�ɐ����ʒm���΂�.
                    }
                }
            });
    }

    public override void StopLister()
    {
        listener?.Dispose();
        roomId = "";
        roomName = "";
    }
    private async Task<string> GetUserName(string userId)
    {
        DocumentSnapshot snapshot = await db.Collection("users").Document(userId).GetSnapshotAsync();
        if (snapshot.Exists && snapshot.ContainsField("username"))
        {
            return snapshot.GetValue<string>("username");
        }
        return "Unknown";
    }
}
