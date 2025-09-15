using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <summary>
/// ConcurrentDictionary<uint, OnePersonData> 形式のユーザーデータを
/// JSONファイルとして非同期に保存・読み込みするための静的ヘルパークラス。
/// </summary>
public static class UserDataManager
{
    /// <summary>
    /// すべてのユーザーデータを指定されたファイルパスにJSON形式で非同期に保存します。
    /// </summary>
    /// <param name="allUserData">保存するユーザーデータ</param>
    /// <param name="filePath">保存先のファイルパス</param>
    /// <returns>非同期操作を表すTask</returns>
    public static async Task SaveAsync(ConcurrentDictionary<uint, OnePersonData> allUserData, string filePath)
    {
        try
        {
            // ファイルを保存するディレクトリが存在しない場合は作成する
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                Console.WriteLine($"ディレクトリを作成しました: {directoryPath}");
            }

            // データをJSON文字列にシリアル化（インデント付きで見やすくする）
            string json = JsonConvert.SerializeObject(allUserData, Formatting.Indented);

            // JSON文字列をファイルに非同期で書き込む
            await File.WriteAllTextAsync(filePath, json);
        }
        catch (Exception ex)
        {
            // エラーが発生した場合はコンソールに出力
            Console.WriteLine($"[エラー] データの保存中に問題が発生しました: {ex.Message}");
            throw; // 必要に応じて例外を再スロー
        }
    }

    /// <summary>
    ///指定されたファイルパスからJSONデータを読み込み、
    ///ConcurrentDictionary<uint, OnePersonData>として非同期にデシリアライズします。
    /// </summary>
    /// <param name="filePath">読み込むファイルのパス</param>
    /// <returns>読み込まれたユーザーデータ。ファイルが存在しない場合や失敗した場合は新しい空の辞書を返します。</returns>
    public static async Task<ConcurrentDictionary<uint, OnePersonData>> LoadAsync(string filePath)
    {
        // ファイルが存在しない場合は、新しい空の辞書を返して処理を中断
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"[情報] ファイルが見つかりません: {filePath}。新しい辞書を作成します。");
            return new ConcurrentDictionary<uint, OnePersonData>();
        }

        try
        {
            // ファイルからJSON文字列を非同期で読み込む
            string json = await File.ReadAllTextAsync(filePath);

            // JSON文字列からデシリアライズする
            var loadedData = JsonConvert.DeserializeObject<ConcurrentDictionary<uint, OnePersonData>>(json);
            
            // デシリアライズ結果がnullの場合は、null参照を避けるために空の辞書を返す
            return loadedData ?? new ConcurrentDictionary<uint, OnePersonData>();
        }
        catch (Exception ex)
        {
            // エラーが発生した場合はコンソールに出力し、空の辞書を返す
            Console.WriteLine($"[エラー] データの読み込み中に問題が発生しました: {ex.Message}");
            return new ConcurrentDictionary<uint, OnePersonData>();
        }
    }
}
