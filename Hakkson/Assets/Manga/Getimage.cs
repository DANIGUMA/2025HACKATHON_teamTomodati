using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Getimage : MonoBehaviour
{// APIのベースURL
    public string apiBaseUrl;

    // 画像を表示するUIコンポーネント
    private RawImage rawImage;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        if (rawImage == null)
        {
            Debug.LogError("RawImageコンポーネントが見つかりません。");
        }

        // 例: Start関数で画像をロードする場合
        // StartCoroutine(LoadImage("example.jpg"));
    }

    // --- 画像をロードする関数 ---
    public IEnumerator LoadImage(string imageName)
    {
        if (rawImage == null)
        {
            Debug.LogError("RawImageが設定されていません。");
            yield break;
        }

        string fullUrl = $"{apiBaseUrl.TrimEnd('/')}/GetImage/{imageName}";

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(fullUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"画像のロードに失敗しました: {request.error}");
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            rawImage.texture = texture;
            Debug.Log($"画像 '{imageName}' を正常にロードしました。");
        }
    }

    // --- 外部からこの関数を呼び出す例 ---
    public void LoadImageFromButton(string imageName)
    {
        StartCoroutine(LoadImage(imageName));
    }
}
