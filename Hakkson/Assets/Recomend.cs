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
        Debug.Log("GetStatus�̃e�X�g���J�n���܂�...");
        string response = await apiClient.GetStatus();
        HandleApiResponse(response);
    }

    [Button("Test: Start Server")]
    [ContextMenu("Test: Start Server")]
    public async void TestStartServer()
    {
        Debug.Log("StartServer�̃e�X�g���J�n���܂�...");
        string response = await apiClient.StartServer();
        HandleApiResponse(response);
    }

    [Button("Test: Save Data")]
    [ContextMenu("Test: Save Data")]
    public async void TestSaveData()
    {
        Debug.Log("SaveData�̃e�X�g���J�n���܂�...");
        string response = await apiClient.SaveData();
        HandleApiResponse(response);
    }

    [Button("Test: Load Data")]
    [ContextMenu("Test: Load Data")]
    public async void TestLoadData()
    {
        Debug.Log("LoadData�̃e�X�g���J�n���܂�...");
        string response = await apiClient.LoadData();
        HandleApiResponse(response);
    }

    /// <summary>
    /// API����̃��X�|���X���������鋤�ʂ̃��W�b�N�B
    /// </summary>
    /// <param name="response">API����̃��X�|���X������B�G���[�̏ꍇ��null�B</param>
    private void HandleApiResponse(string response)
    {
        if (response != null)
        {
            Debug.Log($"<color=green>����:</color> API����̃��X�|���X����M���܂����B\n{response}");
        }
        else
        {
            Debug.LogWarning("<color=red>���s:</color> API����̃��X�|���X������܂���ł����B�ڍׂ̓G���[���O���m�F���Ă��������B");
        }
    }

    [Button]
    // --- 2. POST���N�G�X�g�i�ގ����[�U�[�����j ---
    [ContextMenu("Test_GetNearUsers")]
    public async void GetNearUsers(uint userId)
    {
        uint[] users = await apiClient.GetNearUsersAsync(userId);
        if (users != null)
        {
            Debug.Log("�ގ����[�U�[�̎擾�ɐ������܂���: " + string.Join(", ", users));
        }
    }
    [Button]
    // --- 2. POST���N�G�X�g�i�ގ����[�U�[�����j ---
    [ContextMenu("Test_GetNearUsers")]
    public async void GG(uint userId)
    {
        uint[] users = await apiClient.GetResonAsync(userId);
        if (users != null)
        {
            Debug.Log("�ގ����[�U�[�̎擾�ɐ������܂���: " + string.Join(", ", users));
        }
    }
    [Button]
    // --- 3. POST���N�G�X�g�i�������ߖ���擾�j ---
    [ContextMenu("Test_GetRecommendedComics")]
    public async void GetRecommendedComics(uint userId)
    {
        uint[] comics = await apiClient.GetComicsAsync(userId);
        if (comics != null)
        {
            Debug.Log("�������ߖ���̎擾�ɐ������܂���: " + string.Join(", ", comics));
        }
    }

    // --- 4. POST���N�G�X�g�i�R�T�C���ގ��x�v�Z�j ---
    [ContextMenu("Test_GetCosPoint")]
    public async void GetCosPoint(uint[] otherUsers, uint targetUser)
    {
        await apiClient.GetCosPointAsync(targetUser, otherUsers);
        // �R�T�C���ގ��x�̃��X�|���X��apiclient���Ń��O�o�͂���邽�߁A�����ł͓��ʂȏ����͕s�v�ł��B
    }

    // --- . ADD�N�G�X�g ---
    [Button]
    public void AddRandomData()
    {
        GetAddData(Test.GenerateRandomData());
    }

    [Button]
    public async void GetAddData(OnePersonData personData)
    {
        await apiClient.AddUserDataAsync(personData);
        // ���[�U�[�ǉ��̃��X�|���X��apiclient���Ń��O�o�͂���܂��B
    }
}