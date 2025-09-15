using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Getimage : MonoBehaviour
{// API�̃x�[�XURL
    public string apiBaseUrl;

    // �摜��\������UI�R���|�[�l���g
    private RawImage rawImage;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        if (rawImage == null)
        {
            Debug.LogError("RawImage�R���|�[�l���g��������܂���B");
        }

        // ��: Start�֐��ŉ摜�����[�h����ꍇ
        // StartCoroutine(LoadImage("example.jpg"));
    }

    // --- �摜�����[�h����֐� ---
    public IEnumerator LoadImage(string imageName)
    {
        if (rawImage == null)
        {
            Debug.LogError("RawImage���ݒ肳��Ă��܂���B");
            yield break;
        }

        string fullUrl = $"{apiBaseUrl.TrimEnd('/')}/GetImage/{imageName}";

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(fullUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"�摜�̃��[�h�Ɏ��s���܂���: {request.error}");
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            rawImage.texture = texture;
            Debug.Log($"�摜 '{imageName}' �𐳏�Ƀ��[�h���܂����B");
        }
    }

    // --- �O�����炱�̊֐����Ăяo���� ---
    public void LoadImageFromButton(string imageName)
    {
        StartCoroutine(LoadImage(imageName));
    }
}
