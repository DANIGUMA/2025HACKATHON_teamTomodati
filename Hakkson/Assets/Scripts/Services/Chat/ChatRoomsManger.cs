using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.VersionControl;
using UnityEngine;

public partial class ChatRoomsManger : ChatRoomsManagerBase
{
    FirebaseFirestore db;
    public override event Action<string,string> OnChatRoomsReceived;
    public override event Action<string, string> OnFoundRoomsReceived;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        Instance = new ChatRoomsManger();
        Debug.Log("ChatRoomsManager(Firebase) ����������");
    }
    private ChatRoomsManger()
    {
        db = FirebaseFirestore.DefaultInstance;
    }
    public override async void  MakeRoomAsync(string roomname,string ownerId, List<string> membersId)
    {
        try
        {
            DocumentReference newChatRoomRef = db.Collection("groups").Document();
            // �� Document() �������Ȃ��ŌĂԂƎ����Ń����_��ID�����蓖�Ă���
            if (!membersId.Contains(ownerId)) 
            { 
                membersId.Add(ownerId);
            }
                
            //������
            db.Collection("chatRooms").Document(newChatRoomRef.Id);
            var docs = new Dictionary<string, object>()
            {
                {"roomname",roomname },
                {"owner",ownerId},
                {"members",membersId },
                {"createAT",Timestamp.GetCurrentTimestamp().ToString() },

            };
            await newChatRoomRef.SetAsync(docs);
            //Chat���O�ۑ�
            var welcomeMessage = new Dictionary<string, object>()
            {
                { "senderId", ownerId },
                { "senderName", "System" },
                { "message", $"�O���[�v {roomname} ���쐬����܂����I" },
                { "timestamp", Timestamp.GetCurrentTimestamp().ToString() }
            };
            await newChatRoomRef.Collection("message").AddAsync(welcomeMessage);

            //���[�U�[�̏����O���[�v�ɓo�^
            await db.Collection("users").Document(ownerId)
              .UpdateAsync("chatRooms", FieldValue.ArrayUnion(newChatRoomRef.Id));

            foreach (var memberId in membersId) 
            {
                await db.Collection("user").Document(memberId)
                    .UpdateAsync("chatRooms", FieldValue.ArrayUnion(newChatRoomRef.Id));
            }

        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

    }
    public override async void MakeRoomAsync(string ownerId)
    {
        try
        {
            DocumentReference newChatRoomRef = db.Collection("chatRooms").Document();
            // �� Document() �������Ȃ��ŌĂԂƎ����Ń����_��ID�����蓖�Ă���
            db.Collection("chatRooms").Document(newChatRoomRef.Id);
            var docs = new Dictionary<string, object>()
            {
                {"roomname",newChatRoomRef.Id },
                {"owner",ownerId},
                {"members",ownerId },
                {"createAT",Timestamp.GetCurrentTimestamp().ToString() },

            };
            await newChatRoomRef.SetAsync(docs);
            //Chat���O�ۑ�
            var welcomeMessage = new Dictionary<string, object>()
            {
                { "senderId", ownerId },
                { "senderName", "System" },
                { "message", $"�O���[�v {newChatRoomRef.Id} ���쐬����܂����I" },
                { "timestamp", Timestamp.GetCurrentTimestamp().ToString() }
            };
            await newChatRoomRef.Collection("messages").AddAsync(welcomeMessage);

            //���[�U�[�̏����O���[�v�ɓo�^
            await db.Collection("users").Document(ownerId)
              .UpdateAsync("chatRooms", FieldValue.ArrayUnion(newChatRoomRef.Id));
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
    public override async void JoinRoomsAsync(string userId, string roomId)
    {
        try
        {
            // �O���[�v�Ƀ��[�U�[��ǉ�
            await db.Collection("chatRooms").Document(roomId)
                .UpdateAsync("members", FieldValue.ArrayUnion(userId));

            // ���[�U�[�ɃO���[�v��ǉ�
            await db.Collection("users").Document(userId)
                .UpdateAsync("chatRooms", FieldValue.ArrayUnion(roomId));

            Debug.Log($"���[�U�[ {userId} ���O���[�v {roomId} �ɎQ�����܂����B");
        }
        catch (Exception e)
        { 
            Debug.LogError(e.Message);
        }
        
    }
    public override async void AddUserToRoom(string userId,string roomId)
    {
        try
        {
            // �O���[�v�Ƀ��[�U�[��ǉ�
            await db.Collection("chatRooms").Document(roomId)
                .UpdateAsync("members", FieldValue.ArrayUnion(userId));

            // ���[�U�[�ɃO���[�v��ǉ�
            await db.Collection("users").Document(userId)
                .UpdateAsync("chatRooms", FieldValue.ArrayUnion(roomId));

            Debug.Log($"���[�U�[ {userId} ���O���[�v {roomId} �ɏ��҂���܂����B");
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
    public override async void LoadRoomsAsync()
    {
        try { 
            // ���O�C�����̃��[�U�[ID
            string userId = AuthManagerBase.Instance.CurrentUserId;

            if (string.IsNullOrEmpty(userId))
            {
                UnityEngine.Debug.LogError("���[�U�[�����O�C�����Ă��܂���");
                return;
            }
            // users/{userId}/ �h�L�������g���擾
            DocumentSnapshot userdoc = await db.Collection("users")
                                        .Document(userId)
                                        .GetSnapshotAsync();
            if (userdoc.Exists)
            {
                List<string> roomIds = userdoc.GetValue<List<string>>("chatRooms");
                foreach (string Id in roomIds)
                {
                    string roomId = Id;
                    DocumentSnapshot chatdoc = await db.Collection("chatRooms").Document(roomId).GetSnapshotAsync();
                    if (chatdoc.Exists)
                    {
                        string roomName = chatdoc.GetValue<string>("roomname");
                        //var dic = chatdoc.ToDictionary();
                        //foreach (var kv in dic)
                        //{
                        //    Debug.Log($"Key{kv.Key},value:{kv.Value}");
                        //}
                        Debug.Log($"GetRoomname{roomName}");
                        Debug.Log($"GetRoomID{roomId}");
                        OnChatRoomsReceived?.Invoke(roomId, roomName);
                    }
                    else
                    {
                        Debug.Log("Key Is not Exists");
                    }
                }
            }
            else
            {
                Debug.Log("Room is not Exist");
            }
            
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log($"LoadRoomsAsync �ŃG���[: {e}");
        }
    }
    public async override void SearchGroupsByNamePrefix(string keyword)
    {
        string start = keyword;
        string end = keyword + "\uf8ff";
        Debug.Log($"Searching...{keyword}");
        // ������̑O����v���� (��: "Uni" -> "Unity Lovers")
        Query query = db.Collection("chatRooms")
        .WhereGreaterThanOrEqualTo("roomname", start);
        //.WhereLessThanOrEqualTo("roomname", end)
        Debug.Log($"getquery:{query.ToString()}");
        QuerySnapshot snapshot = await query.GetSnapshotAsync();
        //.OrderBy("roomname")
        //.StartAt(keyword)
        //.EndAt(keyword + "\uf8ff")   // Unicode �ő�l�Ŕ͈͎w��
        //.GetSnapshotAsync();
        if (snapshot != null)
        {
            Debug.Log("GetSnapShot");
            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                //var dictonary = doc.ToDictionary();
                //foreach (string key in dictonary.Keys)
                //{
                //    Debug.Log($"Keys:{key}");
                //}
                string groupId = doc.Id;
                Debug.Log($"{groupId}");
                string groupName = doc.ContainsField("roomname") ? doc.GetValue<string>("roomname") : "(No Name)";
                //results.Add((groupId, groupName));
                Debug.Log($"SearchHit! {groupId}:{groupName}");
                OnFoundRoomsReceived?.Invoke(groupId, groupName);
            }
            Debug.Log("EndSnapShot");
        }
        else
        {
            Debug.Log("Seaech is not Success");
        }
          //.ContinueWith(task =>
          //{
          //    if (task.IsFaulted || task.IsCanceled)
          //    {
          //        Debug.LogError("�O���[�v�������s: " + task.Exception);
          //        return;
          //    }

          //    //List<(string, string)> results = new List<(string, string)>();

              

          //    //OnFoundRoomsReceived?.Invoke(results);
          //});
    }

}
