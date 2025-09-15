using System;
using UnityEngine;
using Firebase.Auth;
using System.Threading.Tasks;
using Firebase.Firestore;
using System.Collections.Generic;

public partial class AdministerManager : AdministerManagerBase
{
    FirebaseAuth auth;
    FirebaseUser currentuser;
    FirebaseFirestore db;
    DocumentReference userdoc;
   
    string iconUrl = "";
    string createdAt = "";
    string updatedAt = "";
    string stetusMessage = "";
    
    // 実体を差し込む
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    
    private static void Init()
    {
        Instance = new AdministerManager();
        
        Debug.Log("AdministerManager(Firebase) 初期化完了");
    }
    private AdministerManager() 
    {
        auth = FirebaseAuth.DefaultInstance;
        currentuser = auth.CurrentUser;
        db = FirebaseFirestore.DefaultInstance;
        Task.Run(async () => await OnLogin());
    }
    public override async Task OnLogin()
    {
        Debug.Log("AdministerManagerBase.Instance.OnLogin()");
        
            userdoc = db.Collection("users").Document(AuthManagerBase.Instance.CurrentUserId);
            Debug.Log(userdoc.ToString());
            DocumentSnapshot userdocsnapshot = await userdoc.GetSnapshotAsync();
            if (userdocsnapshot.Exists)
            {
                iconUrl = userdocsnapshot.GetValue<string>("iconUrl");
                createdAt = userdocsnapshot.GetValue<string>("createdAt");
                updatedAt = userdocsnapshot.GetValue<string>("updated");
                stetusMessage = userdocsnapshot.GetValue<string>("stetusMessage");
                Debug.Log("DataBase is Updated");
            }
            else
            {
                Debug.Log("DateBase is not Updated");
            }
        
    }
    public override string UpdatedAt
    {
        get => updatedAt;
        set
        {
            Task.Run(async () => await userdoc.UpdateAsync("updatedAt", Timestamp.GetCurrentTimestamp().ToString()));
            Debug.Log("Firestore の  updatedAt を更新しました");
        }
    }
    public override string UserName 
    {  
        get => currentuser.DisplayName;
        //非同期的に変更
        set
        {
            Task.Run(async () => await currentuser.UpdateUserProfileAsync(new UserProfile
            {
                DisplayName = value
            }));
            var update = new Dictionary<string, object>
            {
                {"username",value},
            };
            Task.Run(async () => await userdoc.UpdateAsync(update));
            Debug.Log("Firestore の username を更新しました");
        } 
    }
    public override void DeleteUser(Action<bool, string> callback)
    {
        currentuser.DeleteAsync().ContinueWith(task =>
        {
            if (!task.IsFaulted)
            {
                callback(false, task.Exception.ToString());
                return;
            }
            if (task.IsCanceled)
            {
                callback(false, task.Exception.ToString());
                return;
            }
            callback(true, "ユーザーデータ削除成功");
        });
    }
}
