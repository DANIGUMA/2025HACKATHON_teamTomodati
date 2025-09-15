using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
public partial class OnePersonData
{
    public ulong personID;
    public double[,] PreferenceData = new double[FixedData.GenreCount, FixedData.ResonCount];
    public Dictionary<ulong, double> CooperativeData = new();

    public double AllWeight;
    public Dictionary<ulong, OneMangaData> mangaDatas = new();

    public double AverageRating, SDRating;
    public Task Calculations()
    {
        Debug.Log("==============================================");
        Debug.Log("OnePersonData.Calculations: 全体の嗜好ベクトル計算を開始します。");

        // 1. ユーザー統計を計算
        CalculateUserStats();
        Debug.Log($"ユーザー統計を更新しました: 平均評価={this.AverageRating:F2}, 標準偏差={this.SDRating:F2}");
        CalculationsCooperativeData();
        // 2. 各漫画のEngagementにユーザー統計を反映
        foreach (OneMangaData item in mangaDatas.Values)
        {
            item.engagement.AverageRating = this.AverageRating;
            item.engagement.SDRating = this.SDRating;
        }
        Debug.Log("全漫画データにユーザー統計を反映しました。");

        Debug.Log("PLINQ を使用してValueとWeightの並列計算を開始します...");
        var results = mangaDatas
            .AsParallel() // 1. 並列処理を宣言
            .Select(item => new // 2. 各要素を並列で変換
            {
                Value = item.Value.Value,
                Weight = item.Value.Weight
            })
            .ToList(); // 3. 全ての結果を安全にリストに集約

        Debug.Log($"{results.Count}件のValueとWeightを収集しました。");
        // --- 結果を分離 ---
        List<double[,]> valueList = results.Select(r => r.Value).ToList();
        List<double> weightList = results.Select(r => r.Weight).ToList();
        // 4. 最終的な加重平均を計算
        PreferenceData = calculator.CalculateWeightedAverage(valueList, weightList.ToArray());
        Debug.Log("<color=yellow>最終的な嗜好ベクトル (LData) の計算が完了しました。</color>");
        Debug.Log("==============================================");

        return Task.CompletedTask;
    }
    Task CalculationsCooperativeData()
    {
        mangaDatas = new();
        foreach (var item in mangaDatas)
        {
            CooperativeData.Add(item.Key, item.Value.Weight);
        }
        return Task.CompletedTask;
    }
    //public Task AddData(OneMangaData data,ulong ID)
    //{

    //}

}
//これは漫画1種類のデータ
public class OneMangaData
{
    ulong ID;
    public Engagement engagement;
    public byte[] Genre = new byte[FixedData.GenreCount];
    public List<byte[]> Reson = new() { new byte[FixedData.ResonCount] };

    public OneMangaData(ulong id, Engagement engagement, byte[] genre, List<byte[]> reson)
    {
        ID = id;
        this.engagement = engagement;
        Genre = genre;
        Reson = reson;
    }

    public double[,] Value
    {
        get
        {
            Debug.Log($"<color=cyan>--- 漫画ID:{ID} のValue (嗜好行列) を計算します ---</color>");
            return GetValue();
        }
    }
    public double Weight
    {
        get
        {
            Debug.Log($"<color=green>--- 漫画ID:{ID} のWeight (重み) を計算します ---</color>");
            return engagement.CalculateWeight();
        }
    }
    private double[,] GetValue()
    {
        // calculatorの存在を前提とする
        double[] resonAverage = calculator.CalculateWeightedAverage(Reson, engagement.Rating);
        Debug.Log($"  Resonの加重平均 (未正規化): [{string.Join(", ", resonAverage.Select(v => v.ToString("F5")))}]");

        resonAverage = calculator.NormalizeVector(resonAverage);
        Debug.Log($"  Resonの加重平均 (正規化後): [{string.Join(", ", resonAverage.Select(v => v.ToString("F5")))}]");

        double[,] matrix = calculator.CreateMatrix(Genre, resonAverage);
        // デバッグ用に生成された行列の内容もログに出力
        // PrintMatrix(matrix); // 必要であれば
        return matrix;
    }

}
public struct Engagement
{
    //---ユーザー固有のデータ---
    public double AverageRating;//全評価データの平均
    public double SDRating;//全評価データの標準偏差
    // --- 基本のエンゲージメント ---
    public double[] Rating;        // 評価 (0-255段階で表現可能)第n話Rating[n]に入っている
    public ushort ChaptersRead;  // 読んだ話数 (最大65,535話)
    public bool IsCompleted;   // 完読したか ★最重要追加項目
    public bool IsFavorited;   // お気に入り登録したか
    public bool HasReread;     // 再読したか

    // --- 精度向上のための追加データ ---
    public ushort TotalChapters; // 漫画の総話数
    public DateTime FirstReadAt;   // 初めて読んだ日時
    public DateTime LastReadAt;    // 最後に読んだ日時
    public bool IsHidden;


    // --- 定数（重み計算の係数） ---
    private const double RatingCoeff = 3.0;
    private const double CompletionRateCoeff = 2.5;
    private const double CompletedBonus = 1.5;
    private const double FavoritedBonus = 2.0;
    private const double RereadBonus = 3.0;
    /// <summary>
    /// 全ての指標を統合し、最終的なエンゲージメントの「重み」を計算します。
    /// </summary>
    public double CalculateWeight()
    {
        // --- ネガティブ指標による早期リターン ---
        if (IsHidden)
        {
            return 0.0;
        }

        // --- ステップ1: 話数ごとの評価から、漫画全体の「総合評価」を算出 ---
        // 後の話数ほど重視する加重平均（時間減衰）を用いる
        double sumOfRatingProducts = 0; // Σ(評価 × 重み)
        double sumOfRatingWeights = 0;  // Σ(重み)
        int ratedChaptersCount = 0;

        for (int i = 0; i < Rating.Length; i++)
        {
            // 評価が0の場合は未評価とみなし、計算に含めない
            if (Rating[i] > 0)
            {
                double weight = i + 1; // 話数番号(1-indexed)を重みとする
                sumOfRatingProducts += Rating[i] * weight;
                sumOfRatingWeights += weight;
                ratedChaptersCount++;
            }
        }

        // 評価が一つもなければ、評価に関するスコアは0とする
        double overallMangaRating = (sumOfRatingWeights > 0) ? sumOfRatingProducts / sumOfRatingWeights : 0;

        // 評価が1つも無い場合は、評価以外の要素だけで重みを決める
        if (ratedChaptersCount == 0)
        {
            // 評価が低すぎてリターンするのは、評価がある場合のみ
            if (overallMangaRating <= 1) return 0.0;
        }

        // --- ステップ2: 総合評価と全エンゲージメントから最終的な「重み」を計算 ---

        // 2a. ユーザーの評価傾向でZ-score正規化
        double zScoreRating = 0.0;
        if (SDRating > 0) // 標準偏差が0の場合はゼロ除算を避ける
        {
            zScoreRating = (overallMangaRating - AverageRating) / SDRating;
        }

        // 2b. 読了率を計算
        double completionRate = (TotalChapters > 0) ? (double)ChaptersRead / TotalChapters : 0;

        // 2c. 基本となる重みを計算
        double finalWeight = (zScoreRating * RatingCoeff) + (completionRate * CompletionRateCoeff);

        // 2d. 各種ボーナスを加算
        if (IsCompleted) finalWeight += CompletedBonus;
        if (IsFavorited) finalWeight += FavoritedBonus;
        if (HasReread) finalWeight += RereadBonus;

        return finalWeight;
    }
}
public static class FixedData
{
    public const int GenreCount = 50;
    public const int ResonCount = 50;
    public static IReadOnlyList<string> Genre = new string[GenreCount];
    public static IReadOnlyList<string> Reson = new string[ResonCount];
}