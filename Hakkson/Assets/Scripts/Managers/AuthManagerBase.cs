using System;
using System.Threading.Tasks;

public abstract partial class AuthManagerBase
{
    /// <summary>
    /// ユーザー登録
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <param name="username"></param>
    /// <param name="callback"></param>
    public abstract void SignUp(string email, string password, string username, Action<bool, string> callback);
    /// <summary>
    /// ログイン
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <param name="callback"></param>
    public abstract void SignIn(string email, string password, Action<bool, string> callback);
    /// <summary>
    /// ログアウト
    /// </summary>
    public abstract void SignOut();
    /// <summary>
    /// UserID ランダムな文字列
    /// </summary>
    public abstract Task OnLogin();
    public abstract string CurrentUserId { get; }
    public abstract uint CrrentUserUintId { get; }
    public abstract string CurrentUserName { get; }
    public abstract string UserEmail { get; }


    // ✅ 静的アクセスポイント（UIからはここを呼ぶだけ）
    public static AuthManagerBase Instance { get; protected set; }
}
