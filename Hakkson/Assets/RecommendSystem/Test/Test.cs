using Sirenix.OdinInspector; // StringBuilder を使うために必要
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Unityエディタ上で推薦システムの計算テストを実行するMonoBehaviour。
/// </summary>
public class Test
{
    [Header("テスト設定")]
    [Tooltip("テストに使用するランダムな漫画データの数")]
    [SerializeField] private int numberOfManga = 100;

    /// <summary>
    /// ゲーム開始時に自動的に呼び出されるUnityのライフサイクルメソッド。
    /// </summary>
    private async void Start()
    {
        await RunTest(numberOfManga);
    }

    /// <summary>
    /// 指定された数のデータでテストを実行します。
    /// </summary>
    [Button]
    private async Task RunTest(int testDataCount)
    {
        Debug.Log($"--- {testDataCount}件の漫画データでテストを開始 ---");

        // 1. テスト対象のOnePersonDataを準備
        OnePersonData personData = new();

        // 2. 指定された数だけランダムな漫画データを生成して追加
        for (uint i = 0; i < (uint)testDataCount; i++)
        {
            // TestGeneratorクラスがプロジェクト内に存在することが前提
            OneMangaData mangaData = TestGenerator.CreateRandomMangaData(i);
            personData.mangaDatas.Add(i, mangaData);
        }
        Debug.Log($"{personData.mangaDatas.Count}件のランダムデータを生成しました。");

        // 4. 嗜好ベクトルの計算処理を実行
        Debug.Log("\n嗜好ベクトル(LData)の計算を実行します...");
        // OnePersonData.Calculations() がプロジェクト内に存在することが前提
        Debug.Log("計算が完了しました。");

        // 5. 結果を表示
        Debug.Log("\n--- 計算結果 (LData) ---");
        PrintMatrix(personData.PreferenceData);
    }
    public static OnePersonData GenerateRandomData()
    {
        OnePersonData personData = new();

        // 2. 指定された数だけランダムな漫画データを生成して追加
        for (uint i = 0; i < 1; i++)
        {
            // TestGeneratorクラスがプロジェクト内に存在することが前提
            OneMangaData mangaData = TestGenerator.CreateRandomMangaData(i);
            personData.mangaDatas.Add(i, mangaData);
        }
        personData.personID = (uint)Random.Range(0, 10000000000);
        return personData;
    }
    /// <summary>
    /// 行列をUnityのコンソールに見やすく表示するためのヘルパー関数。
    /// </summary>
    private void PrintMatrix(double[,] matrix)
    {
        if (matrix == null)
        {
            Debug.LogWarning("行列がNULLです。");
            return;
        }

        StringBuilder sb = new();
        sb.AppendLine("Matrix Output:");
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            sb.Append("[ ");
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                sb.Append($"{matrix[i, j]:F4} ");
            }
            sb.AppendLine("]");
        }
        Debug.Log(sb.ToString());
    }
}
