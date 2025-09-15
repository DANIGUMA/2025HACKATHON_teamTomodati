using System;
using System.Numerics;
using System.Runtime.InteropServices;

public static class Extensions
{
    /// <summary>
    /// Vector<uint> �� 2�� Vector<long> �Ɋg���iWiden�j���܂��B
    /// .NET 7�� Vector.ExtendToVectorInt64 �̋@�\���Č����܂��B
    /// </summary>
    /// <param name="source">�g������Vector<uint></param>
    /// <param name="lower">���ʂ̑O�����i�[����Vector<long></param>
    /// <param name="upper">���ʂ̌㔼���i�[����Vector<long></param>
    public static void ExtendToVectorInt64(this Vector<uint> source, out Vector<long> lower, out Vector<long> upper)
    {
        // Vector<T>�̗v�f����CPU�A�[�L�e�N�`���ɂ���ĈقȂ�
        int uintCount = Vector<uint>.Count;   // ��: 8
        int longCount = Vector<long>.Count; // ��: 4 (uintCount�̔���)

        // �p�t�H�[�}���X�̂��߁A�z��̊m�ۂ͈�x�����ɂ���
        var sourceArray = new uint[uintCount];
        source.CopyTo(sourceArray);

        var lowerArray = new long[longCount];
        var upperArray = new long[longCount];

        // �蓮��uint����long�փL���X�g���A2�̔z��ɕ�����
        for (int i = 0; i < longCount; i++)
        {
            lowerArray[i] = sourceArray[i];
            upperArray[i] = sourceArray[i + longCount];
        }

        // �z�񂩂�V����Vector<long>���쐬
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