using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
public static class calculator
{
    /// <summary>
    /// 複数の行列と重みのリストから、全体の加重平均を一度に計算します。
    /// </summary>
    /// <param name="matrices">行列のリスト。</param>
    /// <param name="weights">重みのリスト。</param>
    /// <returns>計算された加重平均行列。</returns>
    public static double[,] CalculateWeightedAverage(List<double[,]> matrices, double[] weights)
    {
        // --- 引数のチェック ---
        if (matrices == null || weights == null || matrices.Count == 0 || weights.Length == 0)
        {
            throw new ArgumentException("行列または重みのリストが空です。");
        }
        if (matrices.Count != weights.Length)
        {
            throw new ArgumentException("行列の数と重みの数が一致しません。");
        }

        // --- 行列の次元を取得し、全ての行列が同じ次元か確認 ---
        int rows = matrices[0].GetLength(0);
        int cols = matrices[0].GetLength(1);
        for (int k = 1; k < matrices.Count; k++)
        {
            if (matrices[k].GetLength(0) != rows || matrices[k].GetLength(1) != cols)
            {
                throw new ArgumentException("全ての行列は同じ次元である必要があります。");
            }
        }

        // --- 計算 ---
        double[,] resultMatrix = new double[rows, cols];
        double sumOfWeights = weights.Sum();

        if (sumOfWeights == 0)
        {
            return resultMatrix; // 全ての要素が0の行列を返す
        }

        // 各要素について、(値x重み)の合計を計算
        for (int k = 0; k < matrices.Count; k++)
        {
            // 重みが0以下のデータは計算に含めない
            if (weights[k] <= 0) continue;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    resultMatrix[i, j] += matrices[k][i, j] * weights[k];
                }
            }
        }

        // 最後に重みの合計で割る
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                resultMatrix[i, j] /= sumOfWeights;
            }
        }

        return resultMatrix;
    }
    /// <summary>
    /// 2つの1次元配列の全組み合わせに関数を適用し、2次元配列を生成します。
    /// </summary>
    /// <param name="genre">1つ目の配列（行列の行に対応）。</param>
    /// <param name="reson">2つ目の配列（行列の列に対応）。</param>
    /// <param name="operation">2つのint値から1つのint値を生成する関数。</param>
    /// <returns>生成された2次元配列。</returns>
    public static double[,] CreateMatrix<T, U>(T[] genre, U[] reson)
        where T : struct, IConvertible
        where U : struct, IConvertible
    {
        int rows = genre.Length;
        int cols = reson.Length;

        // 戻り値の型に合わせて double[,] で配列を作成する
        double[,] resultMatrix = new double[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                // 1. IConvertibleの機能を使って、計算可能なdouble型に変換する
                double gValue = Convert.ToDouble(genre[i]);
                double rValue = Convert.ToDouble(reson[j]);

                // 2. double型で掛け算を実行する
                double result = gValue * rValue;

                // 3. 計算結果(double)をそのまま格納する
                resultMatrix[i, j] = result;
            }
        }

        return resultMatrix;
    }


    /// <summary>
    /// 複数の1次元byte配列のリストから、全体の加重平均を一度に計算します。
    /// </summary>
    /// <param name="arrays">1次元byte配列のリスト。</param>
    /// <param name="weights">重みの配列。</param>
    /// <returns>計算された加重平均の1次元double配列。</returns>
    public static double[] CalculateWeightedAverage(List<byte[]> arrays, double[] weights)
    {
        // --- 引数のチェック ---
        if (arrays == null || weights == null || arrays.Count == 0 || arrays.Count != weights.Length)
        {
            throw new ArgumentException("引数が不正です。");
        }

        // --- 配列の長さが全て同じか確認 ---
        int length = arrays[0].Length;
        foreach (byte[] arr in arrays)
        {
            if (arr.Length != length)
            {
                throw new ArgumentException("全ての配列は同じ長さである必要があります。");
            }
        }

        // --- 計算 ---
        double[] resultSumOfProducts = new double[length];
        double sumOfWeights = 0;
        for (int i = 0; i < weights.Length; ++i)
        {
            if (weights[i] > 0)
            {
                sumOfWeights += weights[i];
            }
        }


        if (sumOfWeights == 0)
        {
            return new double[length]; // 全ての要素が0の配列を返す
        }

        // (値x重み)の合計を計算
        for (int k = 0; k < arrays.Count; k++)
        {
            if (weights[k] <= 0) continue;

            byte[] currentArray = arrays[k];
            double currentWeight = weights[k];

            for (int i = 0; i < length; i++)
            {
                // byteをdoubleに変換しながら計算
                resultSumOfProducts[i] += currentArray[i] * currentWeight;
            }
        }

        // 最後に重みの合計で割る
        for (int i = 0; i < length; i++)
        {
            resultSumOfProducts[i] /= sumOfWeights;
        }

        return resultSumOfProducts;
    }
    //未使用メソッド
    public static double[] CalculateWeightedAverageSimd(List<byte[]> arrays, double[] weights)
    {
        // --- 引数のチェック ---
        if (arrays == null || weights == null || arrays.Count == 0 || arrays.Count != weights.Length)
        {
            throw new ArgumentException("引数が不正です。");
        }
        int length = arrays[0].Length;
        // (他のチェックも同様)

        // --- 計算準備 ---
        double[] resultSumOfProducts = new double[length];
        double sumOfWeights = weights.Where(w => w > 0).Sum();

        if (sumOfWeights == 0)
        {
            return new double[length];
        }

        // --- ステップ1: (値x重み)の合計を計算 ---
        for (int k = 0; k < arrays.Count; k++)
        {
            if (weights[k] <= 0) continue;

            byte[] currentArray = arrays[k];
            double currentWeight = weights[k];
            Vector<double> weightVec = new(currentWeight);

            int i = 0;
            // SIMDが一度に扱えるbyteの数 (Vector<double>の8倍)
            int byteBlockSize = Vector<double>.Count * 8;

            for (; i <= length - byteBlockSize; i += byteBlockSize)
            {
                // 1. byte配列から一度に多くの要素をロード
                Vector<byte> byteVec = new(currentArray, i);

                // 2. byte -> ushort -> uint へ拡張
                Vector.Widen(byteVec, out Vector<ushort> ushortVec1, out Vector<ushort> ushortVec2);
                Vector.Widen(ushortVec1, out Vector<uint> uintVec1, out Vector<uint> uintVec2);
                Vector.Widen(ushortVec2, out Vector<uint> uintVec3, out Vector<uint> uintVec4);

                // 3. uint -> long へ自作の拡張メソッドで拡張
                uintVec1.ExtendToVectorInt64(out Vector<long> longVec1, out Vector<long> longVec2);
                uintVec2.ExtendToVectorInt64(out Vector<long> longVec3, out Vector<long> longVec4);
                uintVec3.ExtendToVectorInt64(out Vector<long> longVec5, out Vector<long> longVec6);
                uintVec4.ExtendToVectorInt64(out Vector<long> longVec7, out Vector<long> longVec8);

                // 4. long -> double へ変換
                Vector<double> dVec1 = Vector.ConvertToDouble(longVec1);
                Vector<double> dVec2 = Vector.ConvertToDouble(longVec2);
                Vector<double> dVec3 = Vector.ConvertToDouble(longVec3);
                Vector<double> dVec4 = Vector.ConvertToDouble(longVec4);
                Vector<double> dVec5 = Vector.ConvertToDouble(longVec5);
                Vector<double> dVec6 = Vector.ConvertToDouble(longVec6);
                Vector<double> dVec7 = Vector.ConvertToDouble(longVec7);
                Vector<double> dVec8 = Vector.ConvertToDouble(longVec8);

                // 5. 結果ベクトルに加重積を加算して書き戻す
                int V_DBL_COUNT = Vector<double>.Count;
                Vector<double> resVec1 = new(resultSumOfProducts, i);
                Vector<double> resVec2 = new(resultSumOfProducts, i + V_DBL_COUNT);
                Vector<double> resVec3 = new(resultSumOfProducts, i + (V_DBL_COUNT * 2));
                Vector<double> resVec4 = new(resultSumOfProducts, i + (V_DBL_COUNT * 3));
                Vector<double> resVec5 = new(resultSumOfProducts, i + (V_DBL_COUNT * 4));
                Vector<double> resVec6 = new(resultSumOfProducts, i + (V_DBL_COUNT * 5));
                Vector<double> resVec7 = new(resultSumOfProducts, i + (V_DBL_COUNT * 6));
                Vector<double> resVec8 = new(resultSumOfProducts, i + (V_DBL_COUNT * 7));

                (resVec1 + (dVec1 * weightVec)).CopyTo(resultSumOfProducts, i);
                (resVec2 + (dVec2 * weightVec)).CopyTo(resultSumOfProducts, i + V_DBL_COUNT);
                (resVec3 + (dVec3 * weightVec)).CopyTo(resultSumOfProducts, i + (V_DBL_COUNT * 2));
                (resVec4 + (dVec4 * weightVec)).CopyTo(resultSumOfProducts, i + (V_DBL_COUNT * 3));
                (resVec5 + (dVec5 * weightVec)).CopyTo(resultSumOfProducts, i + (V_DBL_COUNT * 4));
                (resVec6 + (dVec6 * weightVec)).CopyTo(resultSumOfProducts, i + (V_DBL_COUNT * 5));
                (resVec7 + (dVec7 * weightVec)).CopyTo(resultSumOfProducts, i + (V_DBL_COUNT * 6));
                (resVec8 + (dVec8 * weightVec)).CopyTo(resultSumOfProducts, i + (V_DBL_COUNT * 7));
            }

            // SIMDで処理できなかった端数部分の処理
            for (; i < length; i++)
            {
                resultSumOfProducts[i] += currentArray[i] * currentWeight;
            }
        }

        // --- ステップ2: 最後に重みの合計で割る ---
        Vector<double> divisorVec = new(sumOfWeights);
        int j = 0;
        for (; j <= length - Vector<double>.Count; j += Vector<double>.Count)
        {
            Vector<double> pVec = new(resultSumOfProducts, j);
            (pVec / divisorVec).CopyTo(resultSumOfProducts, j);
        }
        // 端数処理
        for (; j < length; j++)
        {
            resultSumOfProducts[j] /= sumOfWeights;
        }

        return resultSumOfProducts;
    }
    /// <summary>
    /// ベクトルの合計が1になるように正規化します (L1 Normalization)。
    /// </summary>
    public static double[] NormalizeVector(double[] vector)
    {
        double sum = vector.Sum();
        if (sum == 0)
        {
            return vector;
        }
        return vector.Select(v => v / sum).ToArray();
    }
    public static double[] ConvertByteToDoubleArray(byte[] byteArray)
    {
        double[] doubleArray = new double[byteArray.Length];
        for (int i = 0; i < byteArray.Length; i++)
        {
            doubleArray[i] = byteArray[i]; // 暗黙的な型変換 (byte -> double)
        }
        return doubleArray;
    }
    /// <summary>
    /// ２つのベクトルのコサイン類似度を計算する
    /// </summary>
    /// <param name="vectorA"></param>
    /// <param name="vectorB"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static double CalculateCosineSimilarity(double[] vectorA, double[] vectorB)
    {
        if (vectorA.Length != vectorB.Length)
        {
            throw new ArgumentException("ベクトルの次元が一致しません。");
        }

        double dotProduct = 0.0;
        double magnitudeA = 0.0;
        double magnitudeB = 0.0;

        for (int i = 0; i < vectorA.Length; i++)
        {
            dotProduct += vectorA[i] * vectorB[i];
            magnitudeA += vectorA[i] * vectorA[i];
            magnitudeB += vectorB[i] * vectorB[i];
        }

        magnitudeA = Math.Sqrt(magnitudeA);
        magnitudeB = Math.Sqrt(magnitudeB);

        if (magnitudeA == 0 || magnitudeB == 0)
        {
            return 0.0;
        }

        return dotProduct / (magnitudeA * magnitudeB);
    }
    private static byte MatrixValue(byte X, byte Y)
    {
        return (byte)(X * Y);
    }

}
