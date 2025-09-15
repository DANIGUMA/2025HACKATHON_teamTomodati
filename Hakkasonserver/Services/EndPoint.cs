using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.ML;
using static Recommender;
using Microsoft.OpenApi.Writers;
public class EndPoint
{
    public ConcurrentDictionary<uint, OnePersonData> AllUserData = new();
    public ConcurrentDictionary<uint, double[]> AllManga = new();
    PredictionEngine<MangaRating, MangaPrediction> predictionEngine;
    Recommender recommender = new();
    public async Task StartServer()
    {
        AllUserData = await Boot.Run(RecommendationController.FilePath);

        List<MangaRating> ratings = AllUserData
        .SelectMany(userEntry =>
            userEntry.Value.CooperativeData.Select(coopEntry => new MangaRating
            {
                UserId = (uint)userEntry.Key,
                MangaId = (uint)coopEntry.Key,
                EngagementScore = (float)coopEntry.Value
            })
        )
        .ToList();
         if (ratings == null || ratings.Count == 0)
        {
            // エラーを避けるために処理を中断するか、空のモデルを返すなどの対応をします。
            return; // このメソッドを終了する場合
        }
        //特異値行列生成
        predictionEngine = recommender.StartLeran(ratings);
         contentBasedRecommendations.GenerateRandomPlanes();
         AllManga = AggregateMangaReasons(AllUserData);
    }
    /// <summary>
    /// 嗜好ベクトルが近いユーザーを検索する
    /// </summary>
    /// <param name="ID">検索するユーザーID</param>
    /// <param name="MaxNumber">最大検索数</param>
    /// <returns>嗜好ベクトルが近いユーザーID配列</returns>
    public uint[] GetNereUserData(uint ID, int MaxNumber = 100)
    {
        contentBasedRecommendations recommendations = new();
        // ターゲットユーザーの嗜好ベクトルを取得
        if (!AllUserData.ContainsKey(ID))
        {
            return new uint[0];
        }

        var targetUserVector = calculator.GetFlattenedPreferenceData(AllUserData[ID]);

        // 全ユーザーの嗜好ベクトルを辞書にまとめる
        var allUserVectors = AllUserData
            .ToDictionary(kvp => kvp.Key, kvp => calculator.GetFlattenedPreferenceData(kvp.Value));

        // コサイン類似度を計算し、上位のユーザーIDを取得
        var similarUsers = recommendations.Calculat(allUserVectors, targetUserVector, MaxNumber);

        return similarUsers.Keys.ToArray();
    }
    /// <summary>
    /// 他のユーザーとの平均コサイン類似度を計算
    /// </summary>
    /// <param name="ID">比較するユーザー</param>
    /// <param name="OtherUser">比較対象のユーザー軍</param>
    /// <returns>平均コサイン類似度</returns>
    public double CosinPoint(uint ID, uint[] OtherUser)
    {
        contentBasedRecommendations recommendations = new();
        HashSet<uint> otherUserSet = new HashSet<uint>(OtherUser.Select(id => (uint)id));

        // LINQのWhereメソッドでフィルタリング
        // Contains()メソッドを使って、otherUserSetに含まれるキーを持つ要素だけを抽出
        var filteredData = AllUserData
            .Where(kvp => otherUserSet.Contains(kvp.Key));

        // フィルタリングされた結果を新しいConcurrentDictionaryに格納
        ConcurrentDictionary<uint, OnePersonData> otherUserData = new(filteredData);
        var targetUserVector = calculator.GetFlattenedPreferenceData(AllUserData[ID]);

        // 全ユーザーの嗜好ベクトルを辞書にまとめる
        var allUserVectors = AllUserData
            .ToDictionary(kvp => kvp.Key, kvp => calculator.GetFlattenedPreferenceData(kvp.Value));

        // コサイン類似度を計算し、上位のユーザーIDを取得
        var similarUsers = recommendations.RecommendByCosin(allUserVectors, targetUserVector);
        double Average = 0;
        foreach (var CosData in similarUsers.Values)
        {
            Average += CosData;
        }

        return Average / OtherUser.Length;
    }
    /// <summary>
    /// 特定のユーザーに合致する漫画IDを検索する
    /// </summary>
    /// <param name="ID"></param>
    /// <returns>ユーザーに合致する漫画</returns>
    public uint[] GetHighPointManga(uint ID)
    {
        List<uint> MangaID = new();
        var Other = GetNereUserData(ID, 100);
        foreach (var data in Other)
        {
            MangaID.AddRange(AllUserData[data].mangaDatas.Keys.ToList());
        }
        return MangaID.ToArray();
    }
    /// <summary>
    /// ユーザーデータを更新する
    /// </summary>
    /// <param name="Data">Json形式のデータ</param>
   // このメソッドは AllUserData (ConcurrentDictionary<uint, OnePersonData>) に
// アクセスできるクラス内にあることを想定しています。
public bool PUTUserData(byte[] data)
{
    // 1. バイト配列からOnePersonDataオブジェクトを復元
    var onePerson = new OnePersonData();
    onePerson.SetByte(data);

    // 2. 復元したデータに対して必要な計算処理を実行
    //    (元のコードにあった重要な処理なので維持します)
    onePerson.Calculations();
    // --- ここから下のロジックは提示されたコードと全く同じです ---

    // 更新前のオブジェクトを格納する変数
    OnePersonData oldPerson;
    // 3. 辞書から現在の値を取得
    if (!AllUserData.TryGetValue(onePerson.personID, out oldPerson))
    {
        // キーが存在しない場合は、新しいデータとして追加を試みる
        return AllUserData.TryAdd(onePerson.personID, onePerson);
    }

    // 4. キーが存在する場合、TryUpdateでアトミックな更新を試みる
    //    データの競合（レースコンディション）に備えて、複数回リトライする
    for (int i = 0; i < 3; i++) // 例として3回まで再試行
    {
        // TryUpdateは、辞書の現在の値が第3引数(oldPerson)と一致する場合にのみ
        // 第2引数(onePerson)の値で更新する、安全なメソッド
        if (AllUserData.TryUpdate(onePerson.personID, onePerson, oldPerson))
        {
            // 更新に成功
            return true;
        }
        
        // 更新に失敗した場合、別のスレッドが先にデータを変更した可能性があるため、
        // 最新のデータを再度取得してリトライする
        if (!AllUserData.TryGetValue(onePerson.personID, out oldPerson))
        {
            // リトライの過程でキーが削除されていた場合、
            // 新規追加として処理する
            return AllUserData.TryAdd(onePerson.personID, onePerson);
        }
    }

    // 複数回のリトライ後も更新に失敗した場合
    return false;
}
    /// <summary>
    /// 指定された漫画IDに対応する値の配列を取得し、値が大きい順に並べ替えたインデックスの配列を生成します。
    /// この処理は元の配列を一切変更しません。
    /// </summary>
    /// <param name="mangaId">インデックスを取得したい漫画のID。</param>
    /// <returns>
    /// 値の降順（大きい順）でソートされたインデックスの配列。
    /// 指定されたmangaIdが辞書内に見つからない場合は、空の配列を返します。
    /// </returns>
     public int[] GetSortedIndicesByValue(uint mangaId)
    {
        // 1. キーが存在するか安全に確認し、対応する値(double[]配列)を取得
        if (AllManga.TryGetValue(mangaId, out double[] values) && values != null)
        {
            // 2. LINQを使ってインデックス配列を生成
            var sortedIndices = values
                // 各要素を「値」と「元のインデックス」を持つ匿名オブジェクトに変換
                .Select((value, index) => new { Value = value, Index = index })

                // 「値(Value)」を基準に降順（大きい順）で並べ替え
                .OrderByDescending(pair => pair.Value)

                // 並べ替えた結果から「インデックス(Index)」だけを抽出
                .Select(pair => pair.Index)

                // 最終結果を配列として確定
                .ToArray();

            return sortedIndices;
        }

        // キーが存在しない、または値がnullの場合は空の配列を返す
        return Array.Empty<int>();
    }
    float GetMangaScore(uint[] Users, uint MangaID)
    {
        MangaRating Socre = new();
        float AllScore = 0;
        foreach (uint Id in Users)
        {
            Socre = new() { UserId = Id, MangaId = MangaID };
            AllScore = +predictionEngine.Predict(Socre).Score;
        }
        return AllScore / Users.Length;
    }
    /// <summary>
    /// すべてのユーザーデータを集約し、漫画IDごとに最終的な加重平均理由ベクトルを計算します。
    /// </summary>
    /// <param name="allUserData">すべてのユーザーデータが格納されたConcurrentDictionary</param>
    /// <returns>漫画IDをキー、集約された理由ベクトルを値とするDictionary</returns>
    public static ConcurrentDictionary<uint, double[]> AggregateMangaReasons(ConcurrentDictionary<uint, OnePersonData> allUserData)
    {
        // 中間結果を格納する辞書
        // Key: 漫画ID (uint)
        // Value: 各ユーザーの計算結果を格納するリスト。タプル(第1段階で計算した理由ベクトル, 第2段階で使うエンゲージメントの重み)
        var intermediateResults = new Dictionary<uint, List<(double[] reasonVector, double weight)>>();

        // --- ステップ1: 全ユーザーの全漫画データをループし、第1段階の加重平均を計算 ---
        foreach (var user in allUserData.Values)
        {
            foreach (var mangaEntry in user.mangaDatas)
            {
                uint mangaId = mangaEntry.Key;
                OneMangaData mangaData = mangaEntry.Value;

                // 【第1段階の加重平均】
                // 各話の理由ベクトル(Reson)を、各話の評価(Rating)で加重平均する
                
                // ResonのList<byte[]>を計算用にList<double[]>に変換
                var resonVectors = mangaData.Reson
                    .Select(resonArray => resonArray.Select(b => (double)b).ToArray())
                    .ToList();
                
                // 加重平均を計算
                double[] userMangaReasonVector = CalculateWeightedAverageForVectors(resonVectors, mangaData.engagement.Rating);
                
                // 第2段階で使う重みを取得
                double engagementWeight = mangaData.Weight;

                // 中間結果に格納
                if (!intermediateResults.ContainsKey(mangaId))
                {
                    intermediateResults[mangaId] = new List<(double[] reasonVector, double weight)>();
                }
                intermediateResults[mangaId].Add((userMangaReasonVector, engagementWeight));
            }
        }

        // --- ステップ2: 中間結果を基に、第2段階の加重平均を計算 ---
        var finalMangaVectors = new Dictionary<uint, double[]>();

        foreach (var mangaGroup in intermediateResults)
        {
            uint mangaId = mangaGroup.Key;
            var userResults = mangaGroup.Value;

            // 【第2段階の加重平均】
            // 全ユーザーの集約理由ベクトルを、エンゲージメントスコアで加重平均する
            var vectorsToAverage = userResults.Select(tuple => tuple.reasonVector).ToList();
            var weightsForAverage = userResults.Select(tuple => tuple.weight).ToArray();
            
            double[] finalVector = CalculateWeightedAverageForVectors(vectorsToAverage, weightsForAverage);

            finalMangaVectors[mangaId] = finalVector;
        }

        return new ConcurrentDictionary<uint,double[]>(finalMangaVectors);
    }

    /// <summary>
    /// ベクトルのリストを指定された重みで加重平均するヘルパーメソッド。
    /// </summary>
    /// <param name="vectors">加重平均するベクトルのリスト</param>
    /// <param name="weights">各ベクトルに対応する重みの配列</param>
    /// <returns>加重平均された単一のベクトル</returns>
    private static double[] CalculateWeightedAverageForVectors(List<double[]> vectors, double[] weights)
    {
        if (vectors == null || vectors.Count == 0)
        {
            // 有効なベクトルがない場合は空の配列を返す
            return new double[0];
        }

        int vectorLength = vectors[0].Length;
        var weightedSum = new double[vectorLength];
        double totalWeight = 0;

        for (int i = 0; i < vectors.Count; i++)
        {
            // vectorsとweightsの数が合わない場合や、ベクトル長が不正な場合はスキップ
            if (i >= weights.Length || vectors[i].Length != vectorLength) continue;

            double weight = weights[i];
            if (weight <= 0) continue; // 重みが0以下の場合は計算に含めない

            totalWeight += weight;
            for (int j = 0; j < vectorLength; j++)
            {
                weightedSum[j] += vectors[i][j] * weight;
            }
        }

        if (totalWeight == 0)
        {
            // 全ての重みが0だった場合は、ゼロ除算を避けてゼロベクトルを返す
            return new double[vectorLength];
        }

        for (int i = 0; i < vectorLength; i++)
        {
            weightedSum[i] /= totalWeight;
        }

        return weightedSum;
    }
}