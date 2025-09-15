using System;
using System.Threading.Tasks;

public abstract class AdministerManagerBase
{
    public abstract string UserName{get;set;}
    public abstract string UpdatedAt{get;set;}
    public abstract void DeleteUser(Action<bool,string> callback);
    public abstract Task OnLogin();
    public static AdministerManagerBase Instance{ get; protected set; }
}
