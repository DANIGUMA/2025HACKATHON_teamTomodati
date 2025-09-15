using System;
using System.Threading.Tasks;

/// <summary>
/// 
/// </summary>
/// 
public abstract class ChatManagerBase
{
    /// <summary>
    /// (string message,string username , string timestamp)
    /// データベースが更新されると、メッセージを返すイベントハンドル
    /// </summary>
    public abstract event Action<string,string,string> OnMessageReceived;
    public abstract string ActiveRoomID { get; }
    public abstract string ActiveRoomName { get; set; }
    public abstract void SendMessage(string message);
    public abstract void AnonymusSendMessage(string message);
    public abstract void SendPicture(string pictureURL);
    /// <summary>
    /// データベースに変更がある際OnMessageReceivedにメッセージを渡すようになる関数。
    /// </summary>
    public abstract void StartListenMessages(string roomId);
    /// <summary>
    /// このメソッドを呼び出さない限りメッセージを受信し続けてしまいます。
    /// </summary>
    public abstract void StopLister();
    public static ChatManagerBase Instance { get; protected set; }
}
