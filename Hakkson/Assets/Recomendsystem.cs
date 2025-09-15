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
    /// API�X�e�[�^�X���擾���܂��B
    /// </summary>
    internal async Task GetStatusAsync()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(BASE_URL))
        {
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("API�X�e�[�^�X: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("API�ڑ��G���[: " + request.error);
            }
        }
    }

    // --- Public Methods to get Tasks ---
    // �����̃��\�b�h��Task��Ԃ��A�Ăяo������await�őҋ@�ł��܂��B

    public Task<string> StartServer() => SendGetRequestAsync("/Server");
    public Task<string> GetStatus() => SendGetRequestAsync("");
    public Task<string> SaveData() => SendGetRequestAsync("/SaveData");
    public Task<string> LoadData() => SendGetRequestAsync("/LordData"); // LordData��typo�Ɖ���

    /// <summary>
    /// �w�肳�ꂽ�G���h�|�C���g��GET���N�G�X�g�𑗐M����P��̔񓯊��֐��B
    /// </summary>
    /// <param name="endpoint">�x�[�XURL�ɑ����G���h�|�C���g�p�X</param>
    /// <returns>�������̃��X�|���X������B�G���[����null�B</returns>
    private async Task<string> SendGetRequestAsync(string endpoint)
    {
        string url = BASE_URL + endpoint;
        Debug.Log($"���N�G�X�g�J�n: {url}");

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            try
            {
                await request.SendWebRequest();
            }
            catch (Exception ex)
            {
                Debug.LogError($"���N�G�X�g���M���ɗ�O������: {ex.Message}");
                return null;
            }

            switch (request.result)
            {
                case UnityWebRequest.Result.Success:
                    string responseText = request.downloadHandler.text;
                    Debug.Log($"���X�|���X��M: {responseText}");
                    return responseText;
                default:
                    Debug.LogError($"�G���[: {request.error}\nURL: {url}");
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
                Debug.Log("�ގ����[�U�[��������܂���: " + string.Join(", ", res));
                return res;
            }
            else
            {
                Debug.LogError("�ގ����[�U�[�����G���[: " + request.error);
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
                Debug.Log("���R�x�N�g���擾����: " + string.Join(", ", res));
                return res;
            }
            else
            {
                Debug.LogError("���R�x�N�g�������G���[: " + request.error);
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
                Debug.Log("�������ߖ���: " + string.Join(", ", res));
                return res;
            }
            else
            {
                Debug.LogError("����擾�G���[: " + request.error);
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

        // C#�I�u�W�F�N�g��JSON�ɃV���A���C�Y
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
                Debug.Log("�R�T�C���ގ��x: " + jsonResponse);
            }
            else
            {
                Debug.LogError("�R�T�C���ގ��x�v�Z�G���[: " + request.error);
            }
        }
    }

    /// <summary>
    /// OnePersonData���T�[�o�[�ɔ񓯊��ő��M���܂��B
    /// </summary>
    /// <param name="data">���M���郆�[�U�[�f�[�^</param>
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
            Debug.LogError($"�f�[�^�̃o�C�g�z�񉻂Ɏ��s: {ex.Message}");
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
                Debug.Log("���[�U�[�ǉ�����: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"���[�U�[�ǉ��G���[: {request.error} (Code: {request.responseCode})");
            }
        }
    }
}

