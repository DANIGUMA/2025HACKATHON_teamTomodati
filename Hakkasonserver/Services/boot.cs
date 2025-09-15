using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Concurrent; // スレッドセーフな辞書
public static class Boot
{
    public const string directoryPath = "";
    /// <summary>
    /// ユーザーデータを非同期で読み込み、各ユーザーの計算処理を並列で実行します。
    /// 正しい非同期パターンを使用し、デッドロックのリスクを回避します。
    /// </summary>
    /// <param name="filePath">読み込むデータファイルのパス</param>
    /// <returns>すべての計算が完了した後のユーザーデータ</returns>
    public static async Task<ConcurrentDictionary<uint, OnePersonData>> Run(string filePath)
    {
        // 1. UserDataManager.LoadAsyncを正しく 'await' で呼び出し、結果を取得
        ConcurrentDictionary<uint, OnePersonData> allPersonData = await UserDataManager.LoadAsync(filePath);

        // 2. 読み込んだデータがnullまたは空の場合は、そのまま返す
        if (allPersonData == null || allPersonData.IsEmpty)
        {
            // 空の場合でも空の辞書を返すことで、呼び出し元のnullチェックを不要にする
            return new ConcurrentDictionary<uint, OnePersonData>();
        }

        // 3. 各ユーザーデータのCalculations()メソッドからTaskのリストを作成
        //    この時点ではまだ実行は待機しない
        var calculationTasks = allPersonData.Values.Select(personData => personData.Calculations());

        // 4. Task.WhenAll を使って、すべての計算タスクが完了するのを非同期に待つ
        //    これにより、スレッドをブロックすることなく効率的に並列処理の完了を待機できる
        await Task.WhenAll(calculationTasks);

        // 5. すべての計算が完了した後のデータを返す
        return allPersonData;
    }
}