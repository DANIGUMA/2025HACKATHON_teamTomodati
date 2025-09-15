using System;

public abstract class AuthStateMangerBase
{
    public abstract event Action<string> OnUserStateChanged;

    public abstract event Action<string> OnIdChanged;

    public abstract void StartListenState();
    public abstract void StopListenState();

    public abstract void StartLintenId();
    public abstract void StopLintenId();
    public abstract void SignInAnonymously();
    public abstract void SignOutAnonymously();


    public static AuthStateMangerBase Instance { get; protected set; }
}
