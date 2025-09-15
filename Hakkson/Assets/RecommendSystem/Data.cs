using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
public partial class OnePersonData
{
    public uint personID;
    //嗜好ベクトル
    [JsonIgnore]
    public double[,] PreferenceData = new double[FixedData.GenreCount, FixedData.ResonCount];
    [JsonIgnore]
    public Dictionary<ulong, double> CooperativeData = new();
    [JsonIgnore]
    public double AllWeight;
    public Dictionary<uint, OneMangaData> mangaDatas = new();
    [JsonIgnore]
    public double AverageRating, SDRating;
    [JsonIgnore]
    public Dictionary<uint, float> GussedScore = new();
    public Task Calculations()
    {

        // 1. ユーザー統計を計算
        CalculateUserStats();
        CalculationsCooperativeData();
        // 2. 各漫画のEngagementにユーザー統計を反映
        foreach (OneMangaData item in mangaDatas.Values)
        {
            item.engagement.AverageRating = this.AverageRating;
            item.engagement.SDRating = this.SDRating;
        }
        var results = mangaDatas
            .Select(item => new // 2. 各要素を変換
            {
                Value = item.Value.Value,
                Weight = item.Value.Weight
            })
            .ToList(); // 3. 全ての結果を安全にリストに集約

        // --- 結果を分離 ---
        List<double[,]> valueList = results.Select(r => r.Value).ToList();
        List<double> weightList = results.Select(r => r.Weight).ToList();
        Debug.Log(valueList.Count);
        Debug.Log(weightList.Count);
        // 4. 最終的な加重平均を計算
        PreferenceData = calculator.CalculateWeightedAverage(valueList, weightList.ToArray());

        return Task.CompletedTask;
    }
    Task CalculationsCooperativeData()
    {
        foreach (KeyValuePair<uint, OneMangaData> item in mangaDatas)
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
    public ulong ID;
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

    public OneMangaData()
    {
    }

    [JsonIgnore]
    public double[,] Value
    {
        get
        {
            return GetValue();
        }
    }
    [JsonIgnore]
    public double Weight
    {
        get
        {
            return engagement.CalculateWeight();
        }
    }
    private double[,] GetValue()
    {
        // calculatorの存在を前提とする
        double[] resonAverage = calculator.CalculateWeightedAverage(Reson, engagement.Rating);

        resonAverage = calculator.NormalizeVector(resonAverage);
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
    // ジャンルと好きな理由の数を20個に設定
    public const int GenreCount = 20;
    public const int ResonCount = 20;

    public static IReadOnlyList<string> Genre = new string[GenreCount]
    {
        "Action", "Adventure", "Science Fiction", "Fantasy", "Mystery",
        "Suspense", "Horror", "Slice of Life", "Romance", "Comedy",
        "Sports", "School Life", "Historical", "Gourmet", "Medical",
        "Battle", "Gag", "Human Drama", "Isekai", "Sci-Fi Fantasy"
    };

    // List of reasons for liking a manga in English
    public static IReadOnlyList<string> Reson = new string[ResonCount]
    {
        "I like the characters", "The story is moving", "The art style is appealing", "I get drawn into the world-building", "The foreshadowing is amazing",
        "The humor is my type", "The dialogue is touching", "The intense battles are exciting", "There are many tear-jerking moments", "I feel good after reading it",
        "The setting is innovative", "I learn new things", "It gives me courage", "It cheers me up", "The friendship is well-portrayed",
        "The romance makes my heart flutter", "The mystery-solving is fun", "The protagonist's growth is inspiring", "The characters have strong personalities", "I can read it over and over again"
    };
    public static IReadOnlyList<uint> ManagaID = new uint[3] { 0, 1, 2 };
    public static readonly byte[] HorrorVector = new byte[GenreCount]
        {
        0, 0, 0, 0, 0, 0, 1, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };


}