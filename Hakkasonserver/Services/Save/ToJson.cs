// ファイル名: OnePersonData.Json.cs
// 元のOnePersonData.csと同じ名前空間に配置

using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
public partial class OnePersonData
{
    /// <summary>
    /// 現在のOnePersonDataオブジェクトをJSON文字列にシリアライズします。
    /// </summary>
    /// <returns>JSON形式の文字列</returns>
    public string ToJson()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
    public static OnePersonData FromJson(string json)
    {
        return JsonConvert.DeserializeObject<OnePersonData>(json);
    }
     /// <summary>
    /// シリアライズ対象のデータをJSON文字列に変換し、さらにUTF-8のバイト配列として取得します。
    /// </summary>
    /// <returns>オブジェクトのバイナリデータ</returns>
    public byte[] GetByte()
    {
        // 1. Newtonsoft.Jsonを使ってオブジェクトをJSON文字列にシリアライズ
        //    [JsonIgnore]属性が付いたプロパティは自動的に除外される
        string jsonString = JsonConvert.SerializeObject(this);

        // 2. JSON文字列をUTF-8エンコーディングでbyte配列に変換
        return Encoding.UTF8.GetBytes(jsonString);
    }

    /// <summary>
    /// UTF-8のバイト配列からJSON文字列を復元し、自身のシリアライズ対象フィールドを更新します。
    /// </summary>
    /// <param name="value">復元元のバイナリデータ</param>
    public void SetByte(byte[] value)
    {
        if (value == null || value.Length == 0)
        {
            return;
        }

        // 1. UTF-8エンコーディングのbyte配列をJSON文字列に変換
        string jsonString = Encoding.UTF8.GetString(value);

        // 2. JSON文字列から新しいOnePersonDataオブジェクトを一時的にデシリアライズ
        var deserializedData = JsonConvert.DeserializeObject<OnePersonData>(jsonString);

        if (deserializedData != null)
        {
            // 3. デシリアライズしたオブジェクトの値を、現在のオブジェクトのフィールドにコピー
            //    [JsonIgnore]が付いたフィールドはデシリアライズされないため、元の値を保持
            this.personID = deserializedData.personID;
            this.mangaDatas = deserializedData.mangaDatas;
        }
    }
}
    // /// <summary>
    // /// 現在のOnePersonDataオブジェクトをJSONファイルに保存します。
    // /// </summary>
    // /// <param name="filePath">保存先のファイルパス</param>
    // public async Task SaveToJsonAsync(string filePath)
    // {
    //     var jsonString = this.ToJson();
    //     await File.WriteAllTextAsync(filePath, jsonString);
    // }

    // /// <summary>
    // /// 指定されたJSONファイルからOnePersonDataオブジェクトをデシリアライズします。
    // /// </summary>
    // /// <param name="filePath">読み込むJSONファイルのパス</param>
    // /// <returns>デシリアライズされたOnePersonDataオブジェクト</returns>
    // public static async Task<OnePersonData> LoadFromJsonAsync(string filePath)
    // {
    //     if (!File.Exists(filePath)) return null;

    //             string jsonString = File.ReadAllText(filePath);
    //             return FromJson(jsonString);
    // }