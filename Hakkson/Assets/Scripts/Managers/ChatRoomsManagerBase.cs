using NUnit.Framework;
using System;
using System.Collections.Generic;

public abstract class ChatRoomsManagerBase
{
    /// <summary>
    /// (string roomname ,string roomId)
    /// </summary>
    public abstract event Action<string,string> OnChatRoomsReceived;
    /// <summary>
    /// List<Tuple<(string roomId,string roomname)>>
    /// </summary>
    public abstract event Action<string, string > OnFoundRoomsReceived;
    /// <summary>
    /// チャットルームの作成
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="memberIds"></param>
    /// <param name="roomname"></param>
    public abstract void MakeRoomAsync(string roomname,string userId, List<string> memberIds);
    /// <summary>
    /// チャットルームの作成
    /// </summary>
    /// <param name="userId"></param>
    public abstract void MakeRoomAsync(string userId);
    /// <summary>
    /// チャットルームへの参加
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="roomId"></param>
    public abstract void JoinRoomsAsync(string userId, string roomId);
    /// <summary>
    /// User/{UserId}/{"chatRooms"}上に存在する全てのルームデータを取得。
    /// </summary>
    public abstract void AddUserToRoom(string userId, string roomId);
    public abstract void LoadRoomsAsync();
    public abstract void SearchGroupsByNamePrefix(string keyword );
    public static ChatRoomsManagerBase Instance { get; protected set; }
}
