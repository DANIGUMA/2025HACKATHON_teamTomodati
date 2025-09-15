using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MangaManeger : MonoBehaviour
{
    public Move move;
    public OpenAnke anke;
    ApiClient apiClient = new();
    public List<TextMeshProUGUI> texts;
    public uint ID;
    public int Chapter;
    public async void Start()
    {
        anke.Hide();
        move._currentIndex = 0;
        //move.list = Datas.MangaImages;
        ID = 0;
        Chapter = 0;
        for (int i = 0; i < texts.Count; i++)
        {
            texts[i].text = FixedData.Reson[i];
        }
    }
    public void Next()
    {
        if (move._currentIndex + 1 == move.list.Count)
        {
            anke.Show();
        }
        else
        {
            move.NextPage();
        }
    }
    public void Previous()
    {
        move.BackPage();
    }
    public async void FinRead(int i)
    {
        Debug.Log("FIN");
        if (Datas.personData.mangaDatas.ContainsKey(0) == false)
        {
            OneMangaData mangaData = new();
            mangaData.Genre = FixedData.HorrorVector;
            mangaData.Reson[Chapter] = CreateOneHotByteArray(i,FixedData.ResonCount);
            mangaData.engagement = new Engagement();
            mangaData.ID = ID;
            Datas.personData.mangaDatas.Add(ID, mangaData);
            try
            {
                await apiClient.AddUserDataAsync(Datas.personData);
            }
            catch
            {

            }
            finally
            {
                SceneManager.LoadScene("Recomend");
            }
        }
        SceneManager.LoadScene("Recomend");

    }
    public static byte[] CreateOneHotByteArray(int i, int size)
    {
        // 配列のサイズが0以下、またはインデックスが範囲外の場合は例外をスロー
        if (size <= 0)
        {
            throw new ArgumentException("配列のサイズは0より大きくなければなりません。", nameof(size));
        }
        if (i < 0 || i >= size)
        {
            throw new ArgumentOutOfRangeException(nameof(i), "インデックスは配列の範囲内に収まらなければなりません。");
        }

        // すべての要素が0で初期化されたバイト配列を作成
        byte[] byteArray = new byte[size];

        // 指定されたインデックスの要素を1に設定
        byteArray[i] = 1;

        return byteArray;
    }
}
