using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class ApiClient
{
    private const string BASE_URL = "http://localhost:5168/api/v1/recommendation";

    /// <summary>
    /// APIステータスを取得します。
    /// </summary>
    internal async Task GetStatusAsync()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(BASE_URL))
        {
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("APIステータス: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("API接続エラー: " + request.error);
            }
        }
    }

    // --- Public Methods to get Tasks ---
    // これらのメソッドはTaskを返し、呼び出し側はawaitで待機できます。

    public Task<string> StartServer() => SendGetRequestAsync("/Server");
    public Task<string> GetStatus() => SendGetRequestAsync("");
    public Task<string> SaveData() => SendGetRequestAsync("/SaveData");
    public Task<string> LoadData() => SendGetRequestAsync("/LordData"); // LordDataはtypoと仮定

    /// <summary>
    /// 指定されたエンドポイントにGETリクエストを送信する単一の非同期関数。
    /// </summary>
    /// <param name="endpoint">ベースURLに続くエンドポイントパス</param>
    /// <returns>成功時のレスポンス文字列。エラー時はnull。</returns>
    private async Task<string> SendGetRequestAsync(string endpoint)
    {
        string url = BASE_URL + endpoint;
        Debug.Log($"リクエスト開始: {url}");

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            try
            {
                await request.SendWebRequest();
            }
            catch (Exception ex)
            {
                Debug.LogError($"リクエスト送信中に例外が発生: {ex.Message}");
                return null;
            }

            switch (request.result)
            {
                case UnityWebRequest.Result.Success:
                    string responseText = request.downloadHandler.text;
                    Debug.Log($"レスポンス受信: {responseText}");
                    return responseText;
                default:
                    Debug.LogError($"エラー: {request.error}\nURL: {url}");
                    return null;
            }
        }
    }

    internal async Task<uint[]> GetNearUsersAsync(uint userId)
    {
        string endpointUrl = BASE_URL + "/GetNear";
        string jsonData = JsonConvert.SerializeObject(userId);

        using (UnityWebRequest request = new(endpointUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log(jsonResponse);
                uint[] res = ParseUintArray(jsonResponse);
                Debug.Log("類似ユーザーが見つかりました: " + string.Join(", ", res));
                return res;
            }
            else
            {
                Debug.LogError("類似ユーザー検索エラー: " + request.error);
                return null;
            }
        }
    }

    internal async Task<uint[]> GetResonAsync(uint userId)
    {
        string endpointUrl = BASE_URL + "/GetReson";
        string jsonData = JsonConvert.SerializeObject(userId);

        using (UnityWebRequest request = new(endpointUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log(jsonResponse);
                uint[] res = ParseUintArray(jsonResponse);
                Debug.Log("理由ベクトル取得完了: " + string.Join(", ", res));
                return res;
            }
            else
            {
                Debug.LogError("理由ベクトル検索エラー: " + request.error);
                return null;
            }
        }
    }
    internal async Task<uint[]> GetComicsAsync(uint userId)
    {
        string endpointUrl = BASE_URL + "/GetComic";
        string jsonData = JsonConvert.SerializeObject(userId);

        using (UnityWebRequest request = new(endpointUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log($"{jsonResponse}");
                uint[] res = ParseUintArray(jsonResponse);
                Debug.Log("おすすめ漫画: " + string.Join(", ", res));
                return res;
            }
            else
            {
                Debug.LogError("漫画取得エラー: " + request.error);
                return null;
            }
        }
    }

    public static uint[] ParseUintArray(string input)
    {
        string cleanedString = input.Trim('[', ']');
        string[] stringNumbers = cleanedString.Split(',');

        return stringNumbers.Select(s => uint.Parse(s)).ToArray();
    }

    internal async Task GetCosPointAsync(uint targetUser, uint[] otherUsers)
    {
        string endpointUrl = BASE_URL + "/GetCos";

        // C#オブジェクトをJSONにシリアライズ
        CosPointRe data = new() { Target = targetUser, OtherUser = otherUsers };
        string jsonData = JsonConvert.SerializeObject(data);

        using (UnityWebRequest request = new(endpointUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("コサイン類似度: " + jsonResponse);
            }
            else
            {
                Debug.LogError("コサイン類似度計算エラー: " + request.error);
            }
        }
    }

    /// <summary>
    /// OnePersonDataをサーバーに非同期で送信します。
    /// </summary>
    /// <param name="data">送信するユーザーデータ</param>
    internal async Task AddUserDataAsync(OnePersonData data)
    {
        string endpointUrl = BASE_URL + "/AddUser";

        byte[] bodyRaw;
        try
        {
            bodyRaw = data.GetByte();
        }
        catch (Exception ex)
        {
            Debug.LogError($"データのバイト配列化に失敗: {ex.Message}");
            return;
        }

        using (var request = new UnityWebRequest(endpointUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("ユーザー追加成功: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"ユーザー追加エラー: {request.error} (Code: {request.responseCode})");
            }
        }
    }
}

