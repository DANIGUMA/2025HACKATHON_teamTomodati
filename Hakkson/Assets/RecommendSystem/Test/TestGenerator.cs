using System;
using System.Collections.Generic;

public static class TestGenerator
{
    private static readonly Random _random = new Random();

    /// <summary>
    /// �����_����Engagement�f�[�^�𐶐����܂��B
    /// </summary>
    public static Engagement CreateRandomEngagement(int maxChapters)
    {
        var totalChapters = (ushort)_random.Next(10, maxChapters);
        var chaptersRead = (ushort)_random.Next(1, totalChapters + 1);
        var firstRead = DateTime.Now.AddDays(-_random.Next(1, 365));

        var ratings = new double[totalChapters];
        // �ǂ񂾘b���܂Ń����_���ɕ]�������� (���܂ɕ]�����Ȃ�)
        for (int i = 0; i < chaptersRead; i++)
        {
            if (_random.NextDouble() > 0.3) // 30%�̊m���ŕ]�����Ȃ�
            {
                ratings[i] = _random.Next(1, 6); // 1-5�̕]��
            }
        }

        return new Engagement
        {
            // ���悲�Ƃ̃G���Q�[�W�����g
            Rating = ratings,
            TotalChapters = totalChapters,
            ChaptersRead = chaptersRead,
            IsCompleted = (chaptersRead == totalChapters),
            IsFavorited = _random.NextDouble() > 0.7, // 30%�̊m���ł��C�ɓ���
            HasReread = (chaptersRead == totalChapters) && (_random.NextDouble() > 0.8), // ���ǎ҂�20%���ē�
            IsHidden = _random.NextDouble() > 0.98, // 2%�̊m���Ŕ�\��
            FirstReadAt = firstRead,
            LastReadAt = firstRead.AddDays(_random.Next(0, 30))
        };
    }

    /// <summary>
    /// �����_����OneMangaData�𐶐����܂��B
    /// </summary>
    public static OneMangaData CreateRandomMangaData(ulong id)
    {
        var engagement = CreateRandomEngagement(200); //�ő�200�b�Ɖ���

        var genre = new byte[FixedData.GenreCount];
        for (int i = 0; i < genre.Length; i++)
        {
            genre[i] = (byte)_random.Next(0, 11); // 0-10�̃����_���Ȓl
        }

        var reson = new List<byte[]>();
        int resonVectorCount = engagement.TotalChapters; // 1-3��Reson�x�N�g��
        for (int i = 0; i < resonVectorCount; i++)
        {
            var resonVec = new byte[FixedData.ResonCount];
            for (int j = 0; j < resonVec.Length; j++)
            {
                resonVec[j] = (byte)_random.Next(0, 11);
            }
            reson.Add(resonVec);
        }

        return new OneMangaData(id, engagement, genre, reson);
    }
}