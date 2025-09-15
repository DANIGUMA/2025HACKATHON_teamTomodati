using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
public static class calculator
{
    /// <summary>
    /// �����̍s��Əd�݂̃��X�g����A�S�̂̉��d���ς���x�Ɍv�Z���܂��B
    /// </summary>
    /// <param name="matrices">�s��̃��X�g�B</param>
    /// <param name="weights">�d�݂̃��X�g�B</param>
    /// <returns>�v�Z���ꂽ���d���ύs��B</returns>
    public static double[,] CalculateWeightedAverage(List<double[,]> matrices, double[] weights)
    {
        // --- �����̃`�F�b�N ---
        if (matrices == null || weights == null || matrices.Count == 0 || weights.Length == 0)
        {
            throw new ArgumentException("�s��܂��͏d�݂̃��X�g����ł��B");
        }
        if (matrices.Count != weights.Length)
        {
            throw new ArgumentException("�s��̐��Əd�݂̐�����v���܂���B");
        }

        // --- �s��̎������擾���A�S�Ă̍s�񂪓����������m�F ---
        int rows = matrices[0].GetLength(0);
        int cols = matrices[0].GetLength(1);
        for (int k = 1; k < matrices.Count; k++)
        {
            if (matrices[k].GetLength(0) != rows || matrices[k].GetLength(1) != cols)
            {
                throw new ArgumentException("�S�Ă̍s��͓��������ł���K�v������܂��B");
            }
        }

        // --- �v�Z ---
        double[,] resultMatrix = new double[rows, cols];
        double sumOfWeights = weights.Sum();

        if (sumOfWeights == 0)
        {
            return resultMatrix; // �S�Ă̗v�f��0�̍s���Ԃ�
        }

        // �e�v�f�ɂ��āA(�lx�d��)�̍��v���v�Z
        for (int k = 0; k < matrices.Count; k++)
        {
            // �d�݂�0�ȉ��̃f�[�^�͌v�Z�Ɋ܂߂Ȃ�
            if (weights[k] <= 0) continue;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    resultMatrix[i, j] += matrices[k][i, j] * weights[k];
                }
            }
        }

        // �Ō�ɏd�݂̍��v�Ŋ���
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
    /// 2��1�����z��̑S�g�ݍ��킹�Ɋ֐���K�p���A2�����z��𐶐����܂��B
    /// </summary>
    /// <param name="genre">1�ڂ̔z��i�s��̍s�ɑΉ��j�B</param>
    /// <param name="reson">2�ڂ̔z��i�s��̗�ɑΉ��j�B</param>
    /// <param name="operation">2��int�l����1��int�l�𐶐�����֐��B</param>
    /// <returns>�������ꂽ2�����z��B</returns>
    public static double[,] CreateMatrix<T, U>(T[] genre, U[] reson)
        where T : struct, IConvertible
        where U : struct, IConvertible
    {
        int rows = genre.Length;
        int cols = reson.Length;

        // �߂�l�̌^�ɍ��킹�� double[,] �Ŕz����쐬����
        double[,] resultMatrix = new double[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                // 1. IConvertible�̋@�\���g���āA�v�Z�\��double�^�ɕϊ�����
                double gValue = Convert.ToDouble(genre[i]);
                double rValue = Convert.ToDouble(reson[j]);

                // 2. double�^�Ŋ|���Z�����s����
                double result = gValue * rValue;

                // 3. �v�Z����(double)�����̂܂܊i�[����
                resultMatrix[i, j] = result;
            }
        }

        return resultMatrix;
    }


    /// <summary>
    /// ������1����byte�z��̃��X�g����A�S�̂̉��d���ς���x�Ɍv�Z���܂��B
    /// </summary>
    /// <param name="arrays">1����byte�z��̃��X�g�B</param>
    /// <param name="weights">�d�݂̔z��B</param>
    /// <returns>�v�Z���ꂽ���d���ς�1����double�z��B</returns>
    public static double[] CalculateWeightedAverage(List<byte[]> arrays, double[] weights)
    {
        // --- �����̃`�F�b�N ---
        if (arrays == null || weights == null || arrays.Count == 0 || arrays.Count != weights.Length)
        {
            throw new ArgumentException("�������s���ł��B");
        }

        // --- �z��̒������S�ē������m�F ---
        int length = arrays[0].Length;
        foreach (byte[] arr in arrays)
        {
            if (arr.Length != length)
            {
                throw new ArgumentException("�S�Ă̔z��͓��������ł���K�v������܂��B");
            }
        }

        // --- �v�Z ---
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
            return new double[length]; // �S�Ă̗v�f��0�̔z���Ԃ�
        }

        // (�lx�d��)�̍��v���v�Z
        for (int k = 0; k < arrays.Count; k++)
        {
            if (weights[k] <= 0) continue;

            byte[] currentArray = arrays[k];
            double currentWeight = weights[k];

            for (int i = 0; i < length; i++)
            {
                // byte��double�ɕϊ����Ȃ���v�Z
                resultSumOfProducts[i] += currentArray[i] * currentWeight;
            }
        }

        // �Ō�ɏd�݂̍��v�Ŋ���
        for (int i = 0; i < length; i++)
        {
            resultSumOfProducts[i] /= sumOfWeights;
        }

        return resultSumOfProducts;
    }
    //���g�p���\�b�h
    public static double[] CalculateWeightedAverageSimd(List<byte[]> arrays, double[] weights)
    {
        // --- �����̃`�F�b�N ---
        if (arrays == null || weights == null || arrays.Count == 0 || arrays.Count != weights.Length)
        {
            throw new ArgumentException("�������s���ł��B");
        }
        int length = arrays[0].Length;
        // (���̃`�F�b�N�����l)

        // --- �v�Z���� ---
        double[] resultSumOfProducts = new double[length];
        double sumOfWeights = weights.Where(w => w > 0).Sum();

        if (sumOfWeights == 0)
        {
            return new double[length];
        }

        // --- �X�e�b�v1: (�lx�d��)�̍��v���v�Z ---
        for (int k = 0; k < arrays.Count; k++)
        {
            if (weights[k] <= 0) continue;

            byte[] currentArray = arrays[k];
            double currentWeight = weights[k];
            Vector<double> weightVec = new(currentWeight);

            int i = 0;
            // SIMD����x�Ɉ�����byte�̐� (Vector<double>��8�{)
            int byteBlockSize = Vector<double>.Count * 8;

            for (; i <= length - byteBlockSize; i += byteBlockSize)
            {
                // 1. byte�z�񂩂��x�ɑ����̗v�f�����[�h
                Vector<byte> byteVec = new(currentArray, i);

                // 2. byte -> ushort -> uint �֊g��
                Vector.Widen(byteVec, out Vector<ushort> ushortVec1, out Vector<ushort> ushortVec2);
                Vector.Widen(ushortVec1, out Vector<uint> uintVec1, out Vector<uint> uintVec2);
                Vector.Widen(ushortVec2, out Vector<uint> uintVec3, out Vector<uint> uintVec4);

                // 3. uint -> long �֎���̊g�����\�b�h�Ŋg��
                uintVec1.ExtendToVectorInt64(out Vector<long> longVec1, out Vector<long> longVec2);
                uintVec2.ExtendToVectorInt64(out Vector<long> longVec3, out Vector<long> longVec4);
                uintVec3.ExtendToVectorInt64(out Vector<long> longVec5, out Vector<long> longVec6);
                uintVec4.ExtendToVectorInt64(out Vector<long> longVec7, out Vector<long> longVec8);

                // 4. long -> double �֕ϊ�
                Vector<double> dVec1 = Vector.ConvertToDouble(longVec1);
                Vector<double> dVec2 = Vector.ConvertToDouble(longVec2);
                Vector<double> dVec3 = Vector.ConvertToDouble(longVec3);
                Vector<double> dVec4 = Vector.ConvertToDouble(longVec4);
                Vector<double> dVec5 = Vector.ConvertToDouble(longVec5);
                Vector<double> dVec6 = Vector.ConvertToDouble(longVec6);
                Vector<double> dVec7 = Vector.ConvertToDouble(longVec7);
                Vector<double> dVec8 = Vector.ConvertToDouble(longVec8);

                // 5. ���ʃx�N�g���ɉ��d�ς����Z���ď����߂�
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

            // SIMD�ŏ����ł��Ȃ������[�������̏���
            for (; i < length; i++)
            {
                resultSumOfProducts[i] += currentArray[i] * currentWeight;
            }
        }

        // --- �X�e�b�v2: �Ō�ɏd�݂̍��v�Ŋ��� ---
        Vector<double> divisorVec = new(sumOfWeights);
        int j = 0;
        for (; j <= length - Vector<double>.Count; j += Vector<double>.Count)
        {
            Vector<double> pVec = new(resultSumOfProducts, j);
            (pVec / divisorVec).CopyTo(resultSumOfProducts, j);
        }
        // �[������
        for (; j < length; j++)
        {
            resultSumOfProducts[j] /= sumOfWeights;
        }

        return resultSumOfProducts;
    }
    /// <summary>
    /// �x�N�g���̍��v��1�ɂȂ�悤�ɐ��K�����܂� (L1 Normalization)�B
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
            doubleArray[i] = byteArray[i]; // �ÖٓI�Ȍ^�ϊ� (byte -> double)
        }
        return doubleArray;
    }
    /// <summary>
    /// �Q�̃x�N�g���̃R�T�C���ގ��x���v�Z����
    /// </summary>
    /// <param name="vectorA"></param>
    /// <param name="vectorB"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static double CalculateCosineSimilarity(double[] vectorA, double[] vectorB)
    {
        if (vectorA.Length != vectorB.Length)
        {
            throw new ArgumentException("�x�N�g���̎�������v���܂���B");
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
