using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using UnityEngine;

public partial class OnePersonData
{
    /// <summary>
    /// SIMD を利用して加重平均を高速に計算します。(訂正版)
    /// </summary>
    public static double WeightedAverageSimd(Span<double> values, Span<double> weights)
    {
        if (values.Length != weights.Length)
        {
            throw new ArgumentException("値と重みの要素数が一致しません。");
        }

        int length = values.Length;
        if (length == 0) return 0;

        Vector<double> sumOfProductsVec = Vector<double>.Zero;
        Vector<double> sumOfWeightsVec = Vector<double>.Zero;

        int vectorSize = Vector<double>.Count;
        int i = 0;

        // ベクトル化できる部分を並列計算
        for (; i <= length - vectorSize; i += vectorSize)
        {
            Vector<double> vVec = new(values.Slice(i, vectorSize));
            Vector<double> wVec = new(weights.Slice(i, vectorSize));

            sumOfProductsVec += vVec * wVec;
            sumOfWeightsVec += wVec;
        }

        // ベクトルの各レーンの値を合計する (★ここを訂正)
        double sumOfProducts = Vector.Dot(sumOfProductsVec, Vector<double>.One);
        double sumOfWeights = Vector.Dot(sumOfWeightsVec, Vector<double>.One);

        // ベクトルに入りきらなかった端数部分を計算
        for (; i < length; i++)
        {
            sumOfProducts += values[i] * weights[i];
            sumOfWeights += weights[i];
        }

        return (sumOfWeights == 0) ? 0 : sumOfProducts / sumOfWeights;
    }
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
    /// <summary>
    /// シリアライズ対象のデータをJSON文字列に変換し、さらにUTF-8のバイト配列として取得します。
    /// </summary>
    /// <returns>オブジェクトのバイナリデータ</returns>
    public byte[] GetByte()
    {
        // 1. Newtonsoft.Jsonを使ってオブジェクトをJSON文字列にシリアライズ
        //    [JsonIgnore]属性が付いたプロパティは自動的に除外される
        string jsonString = JsonConvert.SerializeObject(this);
        Debug.Log(jsonString);
        // 2. JSON文字列をUTF-8エンコーディングでbyte配列に変換
        return Encoding.UTF8.GetBytes(jsonString);
    }

    /// <summary>
    /// UTF-8のバイト配列からJSON文字列を復元し、自身のシリアライズ対象フィールドを更新します。
    /// </summary>
    /// <param name="value">復元元のバイナリデータ</param>
    public void SetByte(byte[] value)
    {
        if (value == null || value.Length == 0)
        {
            return;
        }

        // 1. UTF-8エンコーディングのbyte配列をJSON文字列に変換
        string jsonString = Encoding.UTF8.GetString(value);

        // 2. JSON文字列から新しいOnePersonDataオブジェクトを一時的にデシリアライズ
        OnePersonData deserializedData = JsonConvert.DeserializeObject<OnePersonData>(jsonString);

        if (deserializedData != null)
        {
            // 3. デシリアライズしたオブジェクトの値を、現在のオブジェクトのフィールドにコピー
            //    [JsonIgnore]が付いたフィールドはデシリアライズされないため、元の値を保持
            this.personID = deserializedData.personID;
            this.mangaDatas = deserializedData.mangaDatas;
        }
    }
}