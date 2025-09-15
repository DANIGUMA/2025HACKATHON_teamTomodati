using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public partial class OnePersonData
{
    public void Add(double[,] newMatrix, double newWeight)
    {
        if (newMatrix.GetLength(0) != FixedData.GenreCount || newMatrix.GetLength(1) != FixedData.ResonCount)
        {
            throw new ArgumentException("行列の次元が一致しません。");
        }
        if (newWeight <= 0) return;

        // 各要素について、加重平均を更新
        for (int i = 0; i < FixedData.GenreCount; i++)
        {
            for (int j = 0; j < FixedData.ResonCount; j++)
            {
                double sumOfProduct = PreferenceData[i, j] * AllWeight;
                sumOfProduct += newMatrix[i, j] * newWeight;
                PreferenceData[i, j] = sumOfProduct / (AllWeight + newWeight);
            }
        }
        AllWeight += newWeight;
    }
    /// <summary>
    /// 既存の要素の行列と重みを新しいものに変更し、加重平均を効率的に更新します。
    /// </summary>
    /// <param name="oldMatrix">変更前の行列。</param>
    /// <param name="oldWeight">変更前の重み。</param>
    /// <param name="newMatrix">変更後の行列。</param>
    /// <param name="newWeight">変更後の重み。</param>
    public void Update(double[,] oldMatrix, double oldWeight, double[,] newMatrix, double newWeight)
    {
        // --- 引数のチェック ---
        if (oldMatrix.GetLength(0) != FixedData.GenreCount || oldMatrix.GetLength(1) != FixedData.ResonCount ||
            newMatrix.GetLength(0) != FixedData.GenreCount || newMatrix.GetLength(1) != FixedData.ResonCount)
        {
            throw new ArgumentException("行列の次元が一致しません。");
        }

        // --- 新しい重みの合計を先に計算 ---
        // この値はループ内で不変なので、先に計算しておくと効率が良い
        double updatedAllWight = AllWeight - oldWeight + newWeight;

        // 新しい重みの合計が0以下になる場合は処理を中断（またはエラー処理）
        if (updatedAllWight <= 0)
        {
            // ここでは全ての値を0にリセットする例
            for (int i = 0; i < FixedData.GenreCount; i++)
            {
                for (int j = 0; j < FixedData.ResonCount; j++)
                {
                    PreferenceData[i, j] = 0;
                }
            }
            AllWeight = 0;
            return;
        }

        // --- 各要素について、加重平均を更新 ---
        for (int i = 0; i < FixedData.GenreCount; i++)
        {
            for (int j = 0; j < FixedData.ResonCount; j++)
            {
                // 1. 既存の(値x重み)の合計を復元
                double sumOfProduct = PreferenceData[i, j] * AllWeight;

                // 2. 古い影響を取り除く
                sumOfProduct -= oldMatrix[i, j] * oldWeight;

                // 3. 新しい影響を加える
                sumOfProduct += newMatrix[i, j] * newWeight;

                // 4. 新しい加重平均を計算
                PreferenceData[i, j] = sumOfProduct / updatedAllWight;
            }
        }

        // 最後に全体の重みの合計を更新
        AllWeight = updatedAllWight;
    }
    public void CalculateUserStats()
    {
        // 1. 全ての漫画から、評価済みの全チャプター評価を一つのリストに集める
        List<double> allRatings = new();
        foreach (OneMangaData mangaData in mangaDatas.Values)
        {
            // 評価(Rating)がnullでないことを確認
            if (mangaData.engagement.Rating == null) continue;

            foreach (double chapterRating in mangaData.engagement.Rating)
            {
                // 評価が0より大きい（＝評価済み）ものだけを追加
                if (chapterRating > 0)
                {
                    allRatings.Add(chapterRating);
                }
            }
        }

        // 評価が2件未満の場合は、標準偏差は計算できないため処理を終える
        if (allRatings.Count < 2)
        {
            this.AverageRating = allRatings.Count > 0 ? (byte)allRatings[0] : (byte)0;
            this.SDRating = 0;
            return;
        }

        // 2. 平均値を計算
        double average = allRatings.Average();
        this.AverageRating = (byte)Math.Round(average);

        // 3. 標準偏差を計算
        // 各評価と平均との差の2乗の合計を計算
        double sumOfSquares = allRatings.Sum(rating => (rating - average) * (rating - average));

        // 分散を計算 (合計をデータ数で割る)
        double variance = sumOfSquares / allRatings.Count;

        // 分散の平方根をとって標準偏差を求める
        double stdDev = Math.Sqrt(variance);
        this.SDRating = (byte)Math.Round(stdDev);
    }

}