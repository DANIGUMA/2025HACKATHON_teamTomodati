using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth; // Firebase Authenticationのインポート
using System;
using System.Threading.Tasks; // EventHandlerのために必要

public class AuthStateManagerDontDestroy : MonoBehaviour
{
    // FirebaseAuthのインスタンスを保持する変数
    private FirebaseAuth auth;

    void Awake()
    {
        //破棄されないようにする
        DontDestroyOnLoad(gameObject);
        // FirebaseAuthのデフォルトインスタンスを取得
        auth = FirebaseAuth.DefaultInstance;

        // StateChangedイベントにリスナーを登録
        // このリスナーは、認証状態が変わるたびにOnAuthStateChangedメソッドを呼び出す
        auth.StateChanged += OnAuthStateChanged;
        Debug.Log("FirebaseAuth.StateChanged リスナーを登録しました。");
    }

    // 認証状態が変更されたときに呼び出されるメソッド
    void OnAuthStateChanged(object sender, EventArgs eventArgs)
    {
        // 現在のユーザー情報を取得

        if (AuthManagerBase.Instance.CurrentUserId != null)
        {
            Task.Run(async ()=> await AuthManagerBase.Instance.OnLogin());
            Task.Run(async () => await AdministerManagerBase.Instance.OnLogin());
            // ユーザーがログインしている場合
            Debug.Log($"ユーザーがログインしました: {AuthManagerBase.Instance.CurrentUserName ?? "不明なユーザー名"} ({AuthManagerBase.Instance.UserEmail})");
            // 例: ログイン後の画面に遷移する、ログインUIを非表示にする
            if (SceneManager.GetActiveScene().name != "RecoChat")
            {
                SceneManager.LoadScene("RecoChat");
            }

        }
        else
        {
            // ユーザーがログアウトしている、またはログインしていない場合
            Debug.Log("ユーザーがログアウトしました、またはログインしていません。");
            // 例: ログイン画面を表示する、ゲームのメインメニューに戻す
            if (SceneManager.GetActiveScene().name != "TestLoginScene")
            {
                SceneManager.LoadScene("TestLoginScene");
            }

        }
    }

    void OnDestroy()
    {
        // オブジェクトが破棄される際に、メモリリークを防ぐためリスナーの登録を解除する
        if (auth != null)
        {
            auth.StateChanged -= OnAuthStateChanged;
            Debug.Log("FirebaseAuth.StateChanged リスナーを解除しました。");
        }
    }

    
}
