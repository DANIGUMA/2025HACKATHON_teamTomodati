using System;
using System.Collections.Generic;
using System.Linq;

public class contentBasedRecommendations
{
    // ベクトルの次元数とハッシュのビット数を定義
    private const int VECTOR_SIZE = FixedData.GenreCount * FixedData.ResonCount; // ここにあなたの嗜好ベクトルの次元数を設定
    private const int HASH_BITS = 16;  // ハッシュのビット数（LSHの精度に影響）
    /// <summary>
    /// 全ユーザーの嗜好ベクトルとターゲットユーザーのベクトルを比較し、コサイン類似度を計算します.
    /// </summary>
    /// <param name="allUserVectors">全ユーザーのIDと嗜好ベクトルの辞書</param>
    /// <param name="targetUserVector">ターゲットユーザーの嗜好ベクトル</param>
    /// <param name="MIN_CANDIDATES">最大検索数</param>
    /// <returns>類似度が高い順にソートされた、ユーザーIDと類似度の辞書</returns>
    public Dictionary<uint, double> Calculat(Dictionary<uint, double[]> allUserVectors, double[] targetUserVector, int MIN_CANDIDATES = 100)
    {
        List<uint> MinUserList = RecommendByLSH(allUserVectors, targetUserVector, MIN_CANDIDATES);

        HashSet<uint> MinUserSet = new HashSet<uint>(MinUserList);
        // LINQのWhereメソッドで高速に抽出
        Dictionary<uint, double[]> MinUserDi = allUserVectors
            .Where(kvp => MinUserSet.Contains(kvp.Key))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        return RecommendByCosin(MinUserDi, targetUserVector);
    }
    /// <summary>
    /// LSHによるおすすめを生成し、候補ユーザーのIDリストを返します。
    /// </summary>  
    /// <param name="allUserVectors">全ユーザーIDと嗜好ベクトルの辞書</param>
    /// <param name="targetUserVector">予測対象のユーザーの嗜好ベクトル</param>
    /// <returns>類似性が高い候補ユーザーのIDリスト</returns>
    public List<uint> RecommendByLSH(Dictionary<uint, double[]> allUserVectors, double[] targetUserVector,int MIN_CANDIDATES)
    {
        // ★ハッシュバケットの準備 (ユーザーIDを値に)★
        Dictionary<string, List<uint>> hashBuckets = new();

        // ★全ユーザーのベクトルをハッシュ化し、バケットに格納★
        foreach (KeyValuePair<uint, double[]> entry in allUserVectors)
        {
            uint userId = entry.Key;
            double[] otherUserVector = entry.Value;

            int[] hashArray = Extensions.GenerateLSHHash(otherUserVector, randomPlanes);
            string hashString = string.Join(",", hashArray);

            if (!hashBuckets.ContainsKey(hashString))
            {
                hashBuckets[hashString] = new List<uint>();
            }
            hashBuckets[hashString].Add(userId);
        }

        // 予測対象ユーザーのベクトルをハッシュ化
        int[] userHashArray = Extensions.GenerateLSHHash(targetUserVector, randomPlanes);
        string userHashString = string.Join(",", userHashArray);

        // 候補ユーザーIDを格納するリスト
        List<uint> candidateUserIds = new();

        // ★ハッシュの完全一致をまず探す★
        if (hashBuckets.ContainsKey(userHashString))
        {
            candidateUserIds.AddRange(hashBuckets[userHashString]);
        }

        // ★ハミング距離を許容して検索を拡張する★
        if (candidateUserIds.Count < MIN_CANDIDATES)
        {
            for (int i = 0; i < userHashArray.Length; i++)
            {
                int[] neighborHashArray = (int[])userHashArray.Clone();
                neighborHashArray[i] = neighborHashArray[i] == 1 ? 0 : 1;
                string neighborHashString = string.Join(",", neighborHashArray);

                if (hashBuckets.ContainsKey(neighborHashString))
                {
                    candidateUserIds.AddRange(hashBuckets[neighborHashString]);
                }
            }
        }

        // 重複を削除して返す
        return candidateUserIds.Distinct().ToList();
    }
    /// <summary>
    /// 全ユーザーの嗜好ベクトルとターゲットユーザーのベクトルを比較し、コサイン類似度を計算します。
    /// </summary>
    /// <param name="allUserVectors">全ユーザーのIDと嗜好ベクトルの辞書</param>
    /// <param name="targetUserVector">ターゲットユーザーの嗜好ベクトル</param>
    /// <returns>類似度が高い順にソートされた、ユーザーIDと類似度の辞書</returns>
    public Dictionary<uint, double> RecommendByCosin(Dictionary<uint, double[]> allUserVectors, double[] targetUserVector)
    {
        if (allUserVectors == null || allUserVectors.Count == 0 || targetUserVector == null)
        {
            return new Dictionary<uint, double>();
        }

        // 類似度を格納する一時的な辞書
        Dictionary<uint, double> similarities = new();

        // 全ユーザーのベクトルをループして、類似度を計算
        foreach (KeyValuePair<uint, double[]> entry in allUserVectors)
        {
            uint userId = entry.Key;
            double[] otherUserVector = entry.Value;

            // ターゲットユーザー自身との比較はスキップ
            if (otherUserVector == targetUserVector)
            {
                continue;
            }

            // コサイン類似度を計算
            double similarity = calculator.CalculateCosineSimilarity(targetUserVector, otherUserVector);

            // 結果を辞書に追加
            similarities.Add(userId, similarity);
        }

        // 類似度が高い順にソートして、新しい辞書として返す
        return similarities.OrderByDescending(s => s.Value)
                           .ToDictionary(s => s.Key, s => s.Value);
    }

    private static double[,] randomPlanes = new double[,] { };


    /// <summary>
    /// LSH用のランダムなベクトル群を生成します。
    /// </summary>
    public static void GenerateRandomPlanes()
    {
        randomPlanes = new double[HASH_BITS, VECTOR_SIZE];
        System.Random rand = new();
        for (int i = 0; i < HASH_BITS; i++)
        {
            for (int j = 0; j < VECTOR_SIZE; j++)
            {
                // 平均0、標準偏差1の正規分布に従う乱数を生成
                randomPlanes[i, j] = NextGaussian(rand);
            }
        }
    }

    // ボックスミュラー法による正規分布乱数生成
    private static double NextGaussian(System.Random rand)
    {
        double v1, v2, s;
        do
        {
            v1 = (2.0 * rand.NextDouble()) - 1.0;
            v2 = (2.0 * rand.NextDouble()) - 1.0;
            s = (v1 * v1) + (v2 * v2);
        } while (s >= 1.0 || s == 0);

        s = Math.Sqrt(-2.0 * Math.Log(s) / s);
        return v1 * s;
    }
}
