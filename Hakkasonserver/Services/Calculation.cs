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
            throw new ArgumentException("�s��̎�������v���܂���B");
        }
        if (newWeight <= 0) return;

        // �e�v�f�ɂ��āA���d���ς��X�V
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
    /// �����̗v�f�̍s��Əd�݂�V�������̂ɕύX���A���d���ς������I�ɍX�V���܂��B
    /// </summary>
    /// <param name="oldMatrix">�ύX�O�̍s��B</param>
    /// <param name="oldWeight">�ύX�O�̏d�݁B</param>
    /// <param name="newMatrix">�ύX��̍s��B</param>
    /// <param name="newWeight">�ύX��̏d�݁B</param>
    public void Update(double[,] oldMatrix, double oldWeight, double[,] newMatrix, double newWeight)
    {
        // --- �����̃`�F�b�N ---
        if (oldMatrix.GetLength(0) != FixedData.GenreCount || oldMatrix.GetLength(1) != FixedData.ResonCount ||
            newMatrix.GetLength(0) != FixedData.GenreCount || newMatrix.GetLength(1) != FixedData.ResonCount)
        {
            throw new ArgumentException("�s��̎�������v���܂���B");
        }

        // --- �V�����d�݂̍��v���Ɍv�Z ---
        // ���̒l�̓��[�v���ŕs�ςȂ̂ŁA��Ɍv�Z���Ă����ƌ������ǂ�
        double updatedAllWight = AllWeight - oldWeight + newWeight;

        // �V�����d�݂̍��v��0�ȉ��ɂȂ�ꍇ�͏����𒆒f�i�܂��̓G���[�����j
        if (updatedAllWight <= 0)
        {
            // �����ł͑S�Ă̒l��0�Ƀ��Z�b�g�����
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

        // --- �e�v�f�ɂ��āA���d���ς��X�V ---
        for (int i = 0; i < FixedData.GenreCount; i++)
        {
            for (int j = 0; j < FixedData.ResonCount; j++)
            {
                // 1. ������(�lx�d��)�̍��v�𕜌�
                double sumOfProduct = PreferenceData[i, j] * AllWeight;

                // 2. �Â��e������菜��
                sumOfProduct -= oldMatrix[i, j] * oldWeight;

                // 3. �V�����e����������
                sumOfProduct += newMatrix[i, j] * newWeight;

                // 4. �V�������d���ς��v�Z
                PreferenceData[i, j] = sumOfProduct / updatedAllWight;
            }
        }

        // �Ō�ɑS�̂̏d�݂̍��v���X�V
        AllWeight = updatedAllWight;
    }
    public void CalculateUserStats()
    {
        // 1. �S�Ă̖��悩��A�]���ς݂̑S�`���v�^�[�]������̃��X�g�ɏW�߂�
        List<double> allRatings = new();
        foreach (OneMangaData mangaData in mangaDatas.Values)
        {
            // �]��(Rating)��null�łȂ����Ƃ��m�F
            if (mangaData.engagement.Rating == null) continue;

            foreach (double chapterRating in mangaData.engagement.Rating)
            {
                // �]����0���傫���i���]���ς݁j���̂�����ǉ�
                if (chapterRating > 0)
                {
                    allRatings.Add(chapterRating);
                }
            }
        }

        // �]����2�������̏ꍇ�́A�W���΍��͌v�Z�ł��Ȃ����ߏ������I����
        if (allRatings.Count < 2)
        {
            this.AverageRating = allRatings.Count > 0 ? (byte)allRatings[0] : (byte)0;
            this.SDRating = 0;
            return;
        }

        // 2. ���ϒl���v�Z
        double average = allRatings.Average();
        this.AverageRating = (byte)Math.Round(average);

        // 3. �W���΍����v�Z
        // �e�]���ƕ��ςƂ̍���2��̍��v���v�Z
        double sumOfSquares = allRatings.Sum(rating => (rating - average) * (rating - average));

        // ���U���v�Z (���v���f�[�^���Ŋ���)
        double variance = sumOfSquares / allRatings.Count;

        // ���U�̕��������Ƃ��ĕW���΍������߂�
        double stdDev = Math.Sqrt(variance);
        this.SDRating = (byte)Math.Round(stdDev);
    }

}