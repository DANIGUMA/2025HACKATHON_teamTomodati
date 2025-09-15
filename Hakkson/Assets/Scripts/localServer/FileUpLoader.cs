using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using static System.Net.WebRequestMethods;
using UnityEngine.UI;

public class FileUploader : MonoBehaviour
{
    [SerializeField] string localserver = "http://localhost:5000/upload";
    public IEnumerator UploadFile(string filePath)
    {
        byte[] fileData = System.IO.File.ReadAllBytes(filePath);
        string fileName = System.IO.Path.GetFileName(filePath);

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", fileData, fileName);

        using (UnityWebRequest www = UnityWebRequest.Post(localserver, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Uploaded! URL: " + www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Upload failed: " + www.error);
            }
        }
    }
    public IEnumerator DownloadImage(string url, RawImage target)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D tex = DownloadHandlerTexture.GetContent(www);
                target.texture = tex;
            }
            else
            {
                Debug.LogError("Download failed: " + www.error);
            }
        }
    }

}
