using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public List<Sprite> Kowai = new List<Sprite>();
    public void LoadNextScene(string Name)
    {
        // 次のシーンを読み込む
        // シーン名またはビルド設定のインデックスを指定します
        
        SceneManager.LoadScene(Name);
    }
    public void ShowManaga(string Name)
    {
        SetManaga();
        SceneManager.LoadScene("SampleScene");

    }
    public void SetManaga()
    {
        Datas.MangaImages = Kowai;
    }
}
