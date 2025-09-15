using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class Recommender
{
    // �f�[�^�̃N���X��`
    public class MangaRating
    {
        [KeyType(count: 10000)]
        [LoadColumn(0)]
        public uint UserId;

        // ����ID�̍ő吔��50000���Ɖ��肵��count���w��
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
        // �T���v���f�[�^
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
        // �\���̎��s��
        // ���[�U�[1������102��ǂ񂾏ꍇ�̃G���Q�[�W�����g�X�R�A��\��
        MangaRating mangaPrediction = new() { UserId = 1, MangaId = 102 };
        MangaPrediction prediction = predictionEngine.Predict(mangaPrediction);

        Debug.Log($"���[�U�[ID: {mangaPrediction.UserId} �� ����ID: {mangaPrediction.MangaId} �̗\���G���Q�[�W�����g�X�R�A: {prediction.Score:0.00}");

        // ���[�U�[2������103��ǂ񂾏ꍇ�̃G���Q�[�W�����g�X�R�A��\��
        MangaRating mangaPrediction2 = new() { UserId = 2, MangaId = 103 };
        MangaPrediction prediction2 = predictionEngine.Predict(mangaPrediction2);

        Debug.Log($"���[�U�[ID: {mangaPrediction2.UserId} �� ����ID: {mangaPrediction2.MangaId} �̗\���G���Q�[�W�����g�X�R�A: {prediction2.Score:0.00}");
    }
    public PredictionEngine<MangaRating, MangaPrediction> StartLeran(List<MangaRating> ratings)
    {
        // ML.NET�̊����쐬
        MLContext mlContext = new();



        // �f�[�^��IDataView�ɕϊ�
        IDataView trainingDataView = mlContext.Data.LoadFromEnumerable(ratings);

        // MatrixFactorizationTrainer�̍\��
        MatrixFactorizationTrainer.Options options = new()
        {
            MatrixColumnIndexColumnName = nameof(MangaRating.MangaId),
            MatrixRowIndexColumnName = nameof(MangaRating.UserId),
            LabelColumnName = nameof(MangaRating.EngagementScore),
            NumberOfIterations = 20,
            ApproximationRank = 20
        };

        // �g���[�i�[�I�u�W�F�N�g���쐬
        MatrixFactorizationTrainer trainer = mlContext.Recommendation().Trainers.MatrixFactorization(options);

        // �g���[�i�[��Fit()���\�b�h���g���ă��f�����w�K
        ITransformer model = trainer.Fit(trainingDataView);

        // �\���G���W�����쐬
        PredictionEngine<MangaRating, MangaPrediction> predictionEngine = mlContext.Model.CreatePredictionEngine<MangaRating, MangaPrediction>(model);
        return predictionEngine;
    }
}
