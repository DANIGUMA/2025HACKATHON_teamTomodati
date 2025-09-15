using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class contentBasedRecommendations
{
    // �x�N�g���̎������ƃn�b�V���̃r�b�g�����`
    private const int VECTOR_SIZE = FixedData.GenreCount * FixedData.ResonCount; // �����ɂ��Ȃ��̚n�D�x�N�g���̎�������ݒ�
    private const int HASH_BITS = 16;  // �n�b�V���̃r�b�g���iLSH�̐��x�ɉe���j
    public const int MIN_CANDIDATES = 100;
    public Dictionary<uint, double> Calculat(Dictionary<uint, double[]> allUserVectors, double[] targetUserVector)
    {
        List<uint> MinUserList = RecommendByLSH(allUserVectors, targetUserVector);

        HashSet<uint> MinUserSet = new HashSet<uint>(MinUserList);
        // LINQ��Where���\�b�h�ō����ɒ��o
        Dictionary<uint, double[]> MinUserDi = allUserVectors
            .Where(kvp => MinUserSet.Contains(kvp.Key))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        return RecommendByCosin(MinUserDi, targetUserVector);
    }
    /// <summary>
    /// LSH�ɂ�邨�����߂𐶐����A��⃆�[�U�[��ID���X�g��Ԃ��܂��B
    /// </summary>
    /// <param name="allUserVectors">�S���[�U�[ID�ƚn�D�x�N�g���̎���</param>
    /// <param name="targetUserVector">�\���Ώۂ̃��[�U�[�̚n�D�x�N�g��</param>
    /// <returns>�ގ�����������⃆�[�U�[��ID���X�g</returns>
    public List<uint> RecommendByLSH(Dictionary<uint, double[]> allUserVectors, double[] targetUserVector)
    {
        // ���n�b�V���o�P�b�g�̏��� (���[�U�[ID��l��)��
        Dictionary<string, List<uint>> hashBuckets = new();

        // ���S���[�U�[�̃x�N�g�����n�b�V�������A�o�P�b�g�Ɋi�[��
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

        // �\���Ώۃ��[�U�[�̃x�N�g�����n�b�V����
        int[] userHashArray = Extensions.GenerateLSHHash(targetUserVector, randomPlanes);
        string userHashString = string.Join(",", userHashArray);

        // ��⃆�[�U�[ID���i�[���郊�X�g
        List<uint> candidateUserIds = new();

        // ���n�b�V���̊��S��v���܂��T����
        if (hashBuckets.ContainsKey(userHashString))
        {
            candidateUserIds.AddRange(hashBuckets[userHashString]);
        }

        // ���n�~���O���������e���Č������g�����遚
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

        // �d�����폜���ĕԂ�
        return candidateUserIds.Distinct().ToList();
    }
    /// <summary>
    /// �S���[�U�[�̚n�D�x�N�g���ƃ^�[�Q�b�g���[�U�[�̃x�N�g�����r���A�R�T�C���ގ��x���v�Z���܂��B
    /// </summary>
    /// <param name="allUserVectors">�S���[�U�[��ID�ƚn�D�x�N�g���̎���</param>
    /// <param name="targetUserVector">�^�[�Q�b�g���[�U�[�̚n�D�x�N�g��</param>
    /// <returns>�ގ��x���������Ƀ\�[�g���ꂽ�A���[�U�[ID�Ɨގ��x�̎���</returns>
    public Dictionary<uint, double> RecommendByCosin(Dictionary<uint, double[]> allUserVectors, double[] targetUserVector)
    {
        if (allUserVectors == null || allUserVectors.Count == 0 || targetUserVector == null)
        {
            return new Dictionary<uint, double>();
        }

        // �ގ��x���i�[����ꎞ�I�Ȏ���
        Dictionary<uint, double> similarities = new();

        // �S���[�U�[�̃x�N�g�������[�v���āA�ގ��x���v�Z
        foreach (KeyValuePair<uint, double[]> entry in allUserVectors)
        {
            uint userId = entry.Key;
            double[] otherUserVector = entry.Value;

            // �^�[�Q�b�g���[�U�[���g�Ƃ̔�r�̓X�L�b�v
            if (otherUserVector == targetUserVector)
            {
                continue;
            }

            // �R�T�C���ގ��x���v�Z
            double similarity = calculator.CalculateCosineSimilarity(targetUserVector, otherUserVector);

            // ���ʂ������ɒǉ�
            similarities.Add(userId, similarity);
        }

        // �ގ��x���������Ƀ\�[�g���āA�V���������Ƃ��ĕԂ�
        return similarities.OrderByDescending(s => s.Value)
                           .ToDictionary(s => s.Key, s => s.Value);
    }

    private double[,] randomPlanes;
    public void Start()
    {
        // �A�v���P�[�V�����N�����Ɉ�x���������_���ȃx�N�g���𐶐�
        GenerateRandomPlanes();
    }

    /// <summary>
    /// LSH�p�̃����_���ȃx�N�g���Q�𐶐����܂��B
    /// </summary>
    private void GenerateRandomPlanes()
    {
        randomPlanes = new double[HASH_BITS, VECTOR_SIZE];
        System.Random rand = new();
        for (int i = 0; i < HASH_BITS; i++)
        {
            for (int j = 0; j < VECTOR_SIZE; j++)
            {
                // ����0�A�W���΍�1�̐��K���z�ɏ]�������𐶐�
                randomPlanes[i, j] = NextGaussian(rand);
            }
        }
    }

    // �{�b�N�X�~�����[�@�ɂ�鐳�K���z��������
    private double NextGaussian(System.Random rand)
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
