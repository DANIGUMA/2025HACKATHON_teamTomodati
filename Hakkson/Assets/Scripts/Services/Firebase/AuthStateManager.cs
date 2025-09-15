using Firebase.Auth;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using static UnityEngine.CullingGroup;

public partial class AuthStateManager : AuthStateMangerBase
{
    private FirebaseAuth auth;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        Instance = new AuthStateManager();
    }
    private AuthStateManager()
    {
        auth = FirebaseAuth.DefaultInstance;
        
    }
    public override event Action<string> OnUserStateChanged;
    public override event Action<string> OnIdChanged;

    public override void StartListenState()
    {
        auth.StateChanged += OnStateChange;
    }
    public override void StopListenState()
    {
        auth.StateChanged -= OnStateChange;
    }

    private void OnStateChange(object sender,EventArgs eventArgs)
    {
        if (auth.CurrentUser != null)
        {
            OnUserStateChanged?.Invoke("Login");
        }
        else
        {
            OnUserStateChanged?.Invoke("LogOut");
        }
        
    }
    public override void StartLintenId()
    {
        auth.IdTokenChanged += OnIdChange;
    }
    public override void StopLintenId()
    {
        auth.IdTokenChanged -= OnIdChange;
    }
    private void OnIdChange(object sender,EventArgs eventArgs)
    {
        if(auth.CurrentUser != null)
        {
            OnIdChanged?.Invoke("SessionRunning");
        }
        else
        {
            OnIdChanged?.Invoke("SessionTimeOut");
        }
        
    }

    // 例として、ログインとログアウトのダミーメソッド
    // これらは本来、ログインボタンやログアウトボタンのクリックイベントなどで呼び出されます
    public override void SignInAnonymously()
    {
        auth.SignInAnonymouslyAsync().ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("匿名ログインがキャンセルされました。");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError($"匿名ログインに失敗しました: {task.Exception}");
                return;
            }
            FirebaseUser newUser = task.Result.User;
            Debug.Log($"匿名ユーザーとしてログインしました: {newUser.UserId}");
        });
    }

    public override void SignOutAnonymously()
    {
        auth.SignOut();
        Debug.Log("ユーザーがログアウトしました。");
    }
}

