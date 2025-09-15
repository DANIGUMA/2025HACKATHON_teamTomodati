using System;
using System.Numerics;
using System.Runtime.InteropServices;

public static class Extensions
{
    /// <summary>
    /// Vector<uint> を 2つの Vector<long> に拡張（Widen）します。
    /// .NET 7の Vector.ExtendToVectorInt64 の機能を再現します。
    /// </summary>
    /// <param name="source">拡張元のVector<uint></param>
    /// <param name="lower">結果の前半を格納するVector<long></param>
    /// <param name="upper">結果の後半を格納するVector<long></param>
    public static void ExtendToVectorInt64(this Vector<uint> source, out Vector<long> lower, out Vector<long> upper)
    {
        // Vector<T>の要素数はCPUアーキテクチャによって異なる
        int uintCount = Vector<uint>.Count;   // 例: 8
        int longCount = Vector<long>.Count; // 例: 4 (uintCountの半分)

        // パフォーマンスのため、配列の確保は一度だけにする
        var sourceArray = new uint[uintCount];
        source.CopyTo(sourceArray);

        var lowerArray = new long[longCount];
        var upperArray = new long[longCount];

        // 手動でuintからlongへキャストし、2つの配列に分ける
        for (int i = 0; i < longCount; i++)
        {
            lowerArray[i] = sourceArray[i];
            upperArray[i] = sourceArray[i + longCount];
        }

        // 配列から新しいVector<long>を作成
        lower = new Vector<long>(lowerArray);
        upper = new Vector<long>(upperArray);
    }
    public static int[] GenerateLSHHash(double[] vector, double[,] randomPlanes)
    {
        int numberOfHashes = randomPlanes.GetLength(0);
        int vectorSize = randomPlanes.GetLength(1);
        int[] hash = new int[numberOfHashes];

        for (int i = 0; i < numberOfHashes; i++)
        {
            double dotProduct = 0;
            for (int j = 0; j < vectorSize; j++)
            {
                dotProduct += vector[j] * randomPlanes[i, j];
            }
            hash[i] = dotProduct >= 0 ? 1 : 0;
        }
        return hash;
    }
}