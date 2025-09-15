using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Sirenix.OdinInspector; // StringBuilder を使うために必要

/// <summary>
/// Unityエディタ上で推薦システムの計算テストを実行するMonoBehaviour。
/// </summary>
public class Test : MonoBehaviour
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
        var personData = new OnePersonData();

        // 2. 指定された数だけランダムな漫画データを生成して追加
        for (ulong i = 0; i < (ulong)testDataCount; i++)
        {
            // TestGeneratorクラスがプロジェクト内に存在することが前提
            var mangaData = TestGenerator.CreateRandomMangaData(i);
            personData.mangaDatas.Add(i, mangaData);
        }
        Debug.Log($"{personData.mangaDatas.Count}件のランダムデータを生成しました。");

        // 4. 嗜好ベクトルの計算処理を実行
        Debug.Log("\n嗜好ベクトル(LData)の計算を実行します...");
        // OnePersonData.Calculations() がプロジェクト内に存在することが前提
        await personData.Calculations();
        Debug.Log("計算が完了しました。");

        // 5. 結果を表示
        Debug.Log("\n--- 計算結果 (LData) ---");
        PrintMatrix(personData.PreferenceData);
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

        var sb = new StringBuilder();
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
