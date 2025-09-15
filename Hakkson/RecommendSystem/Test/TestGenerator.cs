using System;
using System.Collections.Generic;

public static class TestGenerator
{
    private static readonly Random _random = new Random();

    /// <summary>
    /// ランダムなEngagementデータを生成します。
    /// </summary>
    public static Engagement CreateRandomEngagement(int maxChapters)
    {
        var totalChapters = (ushort)_random.Next(10, maxChapters);
        var chaptersRead = (ushort)_random.Next(1, totalChapters + 1);
        var firstRead = DateTime.Now.AddDays(-_random.Next(1, 365));

        var ratings = new double[totalChapters];
        // 読んだ話数までランダムに評価を入れる (たまに評価しない)
        for (int i = 0; i < chaptersRead; i++)
        {
            if (_random.NextDouble() > 0.3) // 30%の確率で評価しない
            {
                ratings[i] = _random.Next(1, 6); // 1-5の評価
            }
        }

        return new Engagement
        {
            // 漫画ごとのエンゲージメント
            Rating = ratings,
            TotalChapters = totalChapters,
            ChaptersRead = chaptersRead,
            IsCompleted = (chaptersRead == totalChapters),
            IsFavorited = _random.NextDouble() > 0.7, // 30%の確率でお気に入り
            HasReread = (chaptersRead == totalChapters) && (_random.NextDouble() > 0.8), // 完読者の20%が再読
            IsHidden = _random.NextDouble() > 0.98, // 2%の確率で非表示
            FirstReadAt = firstRead,
            LastReadAt = firstRead.AddDays(_random.Next(0, 30))
        };
    }

    /// <summary>
    /// ランダムなOneMangaDataを生成します。
    /// </summary>
    public static OneMangaData CreateRandomMangaData(ulong id)
    {
        var engagement = CreateRandomEngagement(200); //最大200話と仮定

        var genre = new byte[FixedData.GenreCount];
        for (int i = 0; i < genre.Length; i++)
        {
            genre[i] = (byte)_random.Next(0, 11); // 0-10のランダムな値
        }

        var reson = new List<byte[]>();
        int resonVectorCount = engagement.TotalChapters; // 1-3個のResonベクトル
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