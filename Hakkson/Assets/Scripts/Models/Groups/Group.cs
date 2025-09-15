using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static UnityEngine.InputSystem.InputRemoting;

public class Group
{
    public string Id;
    public string Name;
    public string OwnerId;
    public long CreatedAt;
}

public interface IChatBackend
{
    Task<string> CreateGroup(string name, IEnumerable<string> memberUids);
    IAsyncEnumerable<Message> SubscribeMessages(string groupId, long sinceUnixMs);
    Task SendMessage(string groupId, Message msg); // msg.Idはクライアント生成
    Task<IEnumerable<Group>> ListGroupsForUser(string uid);
}
