using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class Recommender
{
    // データのクラス定義
    public class MangaRating
    {
        [KeyType(count: 10000)]
        [LoadColumn(0)]
        public uint UserId;

        // 漫画IDの最大数が50000冊と仮定してcountを指定
        [KeyType(count: 50000)]
        [LoadColumn(1)]
        public uint MangaId;
        [LoadColumn(2)]
        public float EngagementScore;
    }

    public class MangaPrediction
    {
        public float Score;
    }
    [Button]
    public void Test()
    {
        // サンプルデータ
        List<MangaRating> ratings = new()
        {
            new MangaRating { UserId = 1, MangaId = 101, EngagementScore = 5.0f },
            new MangaRating { UserId = 1, MangaId = 103, EngagementScore = 4.0f },
            new MangaRating { UserId = 2, MangaId = 101, EngagementScore = 5.0f },
            new MangaRating { UserId = 2, MangaId = 102, EngagementScore = 3.0f },
            new MangaRating { UserId = 3, MangaId = 102, EngagementScore = 4.0f },
            new MangaRating { UserId = 3, MangaId = 103, EngagementScore = 5.0f },
            new MangaRating { UserId = 4, MangaId = 101, EngagementScore = 4.5f },
            new MangaRating { UserId = 4, MangaId = 102, EngagementScore = 3.5f },
            new MangaRating { UserId = 4, MangaId = 103, EngagementScore = 5.0f },
            new MangaRating { UserId = 5, MangaId = 102, EngagementScore = 4.0f },
            new MangaRating { UserId = 5, MangaId = 103, EngagementScore = 3.0f }
        };
        PredictionEngine<MangaRating, MangaPrediction> predictionEngine = StartLeran(ratings);
        // 予測の実行例
        // ユーザー1が漫画102を読んだ場合のエンゲージメントスコアを予測
        MangaRating mangaPrediction = new() { UserId = 1, MangaId = 102 };
        MangaPrediction prediction = predictionEngine.Predict(mangaPrediction);

        Debug.Log($"ユーザーID: {mangaPrediction.UserId} と 漫画ID: {mangaPrediction.MangaId} の予測エンゲージメントスコア: {prediction.Score:0.00}");

        // ユーザー2が漫画103を読んだ場合のエンゲージメントスコアを予測
        MangaRating mangaPrediction2 = new() { UserId = 2, MangaId = 103 };
        MangaPrediction prediction2 = predictionEngine.Predict(mangaPrediction2);

        Debug.Log($"ユーザーID: {mangaPrediction2.UserId} と 漫画ID: {mangaPrediction2.MangaId} の予測エンゲージメントスコア: {prediction2.Score:0.00}");
    }
    public PredictionEngine<MangaRating, MangaPrediction> StartLeran(List<MangaRating> ratings)
    {
        // ML.NETの環境を作成
        MLContext mlContext = new();



        // データをIDataViewに変換
        IDataView trainingDataView = mlContext.Data.LoadFromEnumerable(ratings);

        // MatrixFactorizationTrainerの構成
        MatrixFactorizationTrainer.Options options = new()
        {
            MatrixColumnIndexColumnName = nameof(MangaRating.MangaId),
            MatrixRowIndexColumnName = nameof(MangaRating.UserId),
            LabelColumnName = nameof(MangaRating.EngagementScore),
            NumberOfIterations = 20,
            ApproximationRank = 20
        };

        // トレーナーオブジェクトを作成
        MatrixFactorizationTrainer trainer = mlContext.Recommendation().Trainers.MatrixFactorization(options);

        // トレーナーのFit()メソッドを使ってモデルを学習
        ITransformer model = trainer.Fit(trainingDataView);

        // 予測エンジンを作成
        PredictionEngine<MangaRating, MangaPrediction> predictionEngine = mlContext.Model.CreatePredictionEngine<MangaRating, MangaPrediction>(model);
        return predictionEngine;
    }
}
