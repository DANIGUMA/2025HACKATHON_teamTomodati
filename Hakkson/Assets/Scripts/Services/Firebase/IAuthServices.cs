using System;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using Firebase.Firestore;
using System.Collections.Generic;


//安全なアクセスインターフェース
//ユーザーの属性に応じてアクセス可能なヘッダーが違う(一般ユーザーが管理者権限にアクセスできないようにするやつ)
public interface IAuthService 
{
    string CurrentUserId { get; }
    string UserName { get; }
    void SignUp(string email, string password,string username, System.Action<bool, string> callback);
    void SignIn(string email, string password, System.Action<bool, string> callback);
    void SignOut();
}

public interface ISettingsService : IAuthService
{
    FirebaseAuth settingAuth { get; }
    void DeleteUserData(System.Action<bool, string> callback);
}

public class FirebaseAuthService : IAuthService,ISettingsService
{
    private FirebaseAuth auth;
    private FirebaseUser currentUser = null;
    
    public FirebaseAuthService()
    {
        auth = FirebaseAuth.DefaultInstance;
        //currentUser = Firebase.Auth.FirebaseUser
    }
    public string CurrentUserId => Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser?.UserId;

    public string UserName => Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.DisplayName;

    public FirebaseAuth settingAuth => auth;


    public async void SignUp(string email, string password, string userName, System.Action<bool, string> callback)
    {
        try
        {
            var result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            var currentUser = result.User;

            if (string.IsNullOrEmpty(userName))
            {
                userName = currentUser.UserId;
            }

            // ユーザープロフィール更新
            await currentUser.UpdateUserProfileAsync(new Firebase.Auth.UserProfile
            {
                DisplayName = userName
            });

            // Firestoreに保存
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            var userDoc = new Dictionary<string, object>()
            {
                { "username", userName },
                { "email", email },
                { "iconUrl", "" },               // 後でFirebase Storageにアップロードして設定
                { "statusMessage", "" },         // プロフィールコメント
                { "createdAt", Timestamp.GetCurrentTimestamp().ToString() },
                { "updatedAt", Timestamp.GetCurrentTimestamp().ToString() },
                { "frends",new List<string>() },
                { "chatRooms",new List<string>()}
            };

            await db.Collection("users").Document(currentUser.UserId).SetAsync(userDoc);

            Debug.Log("ユーザー名登録完了");
            callback(true, "サインアップ成功");
        }
        catch (Exception e)
        {
            callback(false, e.ToString());
        }
    }

    public async void SignIn(string email, string password, System.Action<bool, string> callback)
    {
        try
        {
            var result = await auth.SignInWithEmailAndPasswordAsync(email, password);
            var currentUser = result.User;

            callback(true, "ログイン成功");


        }
        catch (Exception e)
        {
            callback(false, e.ToString());
        }
    }

    public void SignOut()
    {
        auth.SignOut();
        currentUser = null;
        //return Task.CompletedTask;
    }
    private void CurrentUser()
    {
        currentUser = auth.CurrentUser;
        Debug.Log("Get CurrentUser!");
    } 
    public async void DeleteUserData(System.Action<bool, string> callback)
    {
        CurrentUser();
        try
        {
            await currentUser.DeleteAsync();
            callback(true, "ユーザーデータ削除成功");
        }
        catch (Exception e)
        {
            callback(false, e.ToString());
        }
        
    }
}

