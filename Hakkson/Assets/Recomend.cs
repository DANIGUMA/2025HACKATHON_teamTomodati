using Sirenix.OdinInspector;
using UnityEngine;
using System.Threading.Tasks;

public class Recomend : MonoBehaviour
{
    private readonly ApiClient apiClient = new();

    [ShowInInspector]
    public OnePersonData data1;
    [ShowInInspector]
    public OnePersonData data2;

    // --- Public Methods for UI Buttons or testing ---

    [Button("Test: Get Status")]
    [ContextMenu("Test: Get Status")]
    public async void TestGetStatus()
    {
        Debug.Log("GetStatusのテストを開始します...");
        string response = await apiClient.GetStatus();
        HandleApiResponse(response);
    }

    [Button("Test: Start Server")]
    [ContextMenu("Test: Start Server")]
    public async void TestStartServer()
    {
        Debug.Log("StartServerのテストを開始します...");
        string response = await apiClient.StartServer();
        HandleApiResponse(response);
    }

    [Button("Test: Save Data")]
    [ContextMenu("Test: Save Data")]
    public async void TestSaveData()
    {
        Debug.Log("SaveDataのテストを開始します...");
        string response = await apiClient.SaveData();
        HandleApiResponse(response);
    }

    [Button("Test: Load Data")]
    [ContextMenu("Test: Load Data")]
    public async void TestLoadData()
    {
        Debug.Log("LoadDataのテストを開始します...");
        string response = await apiClient.LoadData();
        HandleApiResponse(response);
    }

    /// <summary>
    /// APIからのレスポンスを処理する共通のロジック。
    /// </summary>
    /// <param name="response">APIからのレスポンス文字列。エラーの場合はnull。</param>
    private void HandleApiResponse(string response)
    {
        if (response != null)
        {
            Debug.Log($"<color=green>成功:</color> APIからのレスポンスを受信しました。\n{response}");
        }
        else
        {
            Debug.LogWarning("<color=red>失敗:</color> APIからのレスポンスがありませんでした。詳細はエラーログを確認してください。");
        }
    }

    [Button]
    // --- 2. POSTリクエスト（類似ユーザー検索） ---
    [ContextMenu("Test_GetNearUsers")]
    public async void GetNearUsers(uint userId)
    {
        uint[] users = await apiClient.GetNearUsersAsync(userId);
        if (users != null)
        {
            Debug.Log("類似ユーザーの取得に成功しました: " + string.Join(", ", users));
        }
    }
    [Button]
    // --- 2. POSTリクエスト（類似ユーザー検索） ---
    [ContextMenu("Test_GetNearUsers")]
    public async void GG(uint userId)
    {
        uint[] users = await apiClient.GetResonAsync(userId);
        if (users != null)
        {
            Debug.Log("類似ユーザーの取得に成功しました: " + string.Join(", ", users));
        }
    }
    [Button]
    // --- 3. POSTリクエスト（おすすめ漫画取得） ---
    [ContextMenu("Test_GetRecommendedComics")]
    public async void GetRecommendedComics(uint userId)
    {
        uint[] comics = await apiClient.GetComicsAsync(userId);
        if (comics != null)
        {
            Debug.Log("おすすめ漫画の取得に成功しました: " + string.Join(", ", comics));
        }
    }

    // --- 4. POSTリクエスト（コサイン類似度計算） ---
    [ContextMenu("Test_GetCosPoint")]
    public async void GetCosPoint(uint[] otherUsers, uint targetUser)
    {
        await apiClient.GetCosPointAsync(targetUser, otherUsers);
        // コサイン類似度のレスポンスはapiclient内でログ出力されるため、ここでは特別な処理は不要です。
    }

    // --- . ADDクエスト ---
    [Button]
    public void AddRandomData()
    {
        GetAddData(Test.GenerateRandomData());
    }

    [Button]
    public async void GetAddData(OnePersonData personData)
    {
        await apiClient.AddUserDataAsync(personData);
        // ユーザー追加のレスポンスはapiclient内でログ出力されます。
    }
}