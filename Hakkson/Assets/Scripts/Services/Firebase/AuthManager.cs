using System;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class AuthManager : AuthManagerBase
{
    private FirebaseAuth auth;
    private FirebaseUser currentUser;
    private FirebaseFirestore db;
    static uint NullId = 0;

    private uint uintId = NullId;
    private string usernametxt;
    private string emailtxt;
    // ���̂���������(���̃R�[�h��Unity���V�[�������[�h����O�ɌĂ�)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        Instance = new AuthManager();
        Debug.Log("AuthManager(Firebase) ����������");
    }

    private AuthManager()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;
        currentUser = auth.CurrentUser;
        Task.Run(async () => await OnLogin());
    }
    //userIdtxt = auth.CurrentUser?.UserId;//ID�͒����o��
    //usernametxt = auth.CurrentUser?.DisplayName;//����ȊO�͈�ċz����B
    //emailtxt = auth.CurrentUser?.Email;
    public override async Task OnLogin()
    {
        Debug.Log("AuthManagerBase.Instance.OnLogin()");
        DocumentSnapshot userdocsnapshot = await db.Collection("users").Document(AuthManagerBase.Instance.CurrentUserId).GetSnapshotAsync();
        if (userdocsnapshot.Exists)
        {
            Debug.Log("uintId Update");
            uintId = userdocsnapshot.GetValue<uint>("uintId");
            usernametxt = userdocsnapshot.GetValue<string>("username");
            emailtxt = userdocsnapshot.GetValue<string>("email");
        }
        else
        {
            Debug.Log("uintId is not Exist");
            uintId = NullId;
        }
    }
    public override string CurrentUserId => auth.CurrentUser?.UserId;
    public override string CurrentUserName => usernametxt;
    public override string UserEmail=> emailtxt;

    public override uint CrrentUserUintId
    {
        get 
        {
            Task.Run(async () => await OnLogin());
            return uintId; 
        }
    }

    public async override void SignUp(string email, string password, string username, Action<bool, string> callback)
    {
        try
        {
            var result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            var currentUser = result.User;
            System.Random rand = new System.Random();
            uint candicate = NullId;
            candicate = (uint)rand.Next(int.MinValue,int.MaxValue);
            

            if (string.IsNullOrEmpty(username))
            {
                username = currentUser.UserId;
            }

            //�ł���Ώd�������Ƃ��̏���.
            uintId = candicate;
            emailtxt = email;
            usernametxt = username;
            // ���[�U�[�v���t�B�[���X�V
            await currentUser.UpdateUserProfileAsync(new Firebase.Auth.UserProfile
            {
                DisplayName = username
            });

            // Firestore�ɕۑ�
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            var userDoc = new Dictionary<string, object>()
            {
                { "username", username },
                { "email", email },
                { "iconUrl", "" },               // ���Firebase Storage�ɃA�b�v���[�h���Đݒ�
                { "statusMessage", "" },         // �v���t�B�[���R�����g
                { "createdAt", Timestamp.GetCurrentTimestamp().ToString() },
                { "updatedAt", Timestamp.GetCurrentTimestamp().ToString() },
                { "frends",new List<string>() },
                { "chatRooms",new List<string>()},
                { "uintId", candicate}
            };

            await db.Collection("users").Document(currentUser.UserId).SetAsync(userDoc);

            Debug.Log("���[�U�[���o�^����");
            callback(true, "�T�C���A�b�v����");
        }
        catch (Exception e)
        {
            callback(false, e.ToString());
        }
    }

    public async override void SignIn(string email, string password, Action<bool, string> callback)
    {
        try
        {
            var result = await auth.SignInWithEmailAndPasswordAsync(email, password);
            await OnLogin();
            var currentUser = result.User;
            emailtxt = email;
            

            callback(true, "���O�C������");


        }
        catch (Exception e)
        {
            callback(false, e.ToString());
        }
    }

    public override void SignOut()
    {
        auth.SignOut();
        currentUser = null;
        uintId = NullId;
    }
}
