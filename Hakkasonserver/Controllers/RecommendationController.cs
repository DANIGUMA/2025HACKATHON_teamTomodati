using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;
using Microsoft.OpenApi.Writers;

// Modelsフォルダに配置するデータモデル
public class UserIdRequest
{
    public uint UserId { get; set; }
}

public class CosPointRe
{
    public uint[] OtherUser { get; set; }
    public uint Target { get; set; }
}

public class UserDataRequest
{
    public string Data { get; set; }
}


[ApiController]
[Route("api/v1/[controller]")]
public class RecommendationController : ControllerBase
{
    // ★ DIの正しいパターン: readonlyフィールドとコンストラクタインジェクション
    private readonly EndPoint _endpoint;
    public static string FilePath = "/app/TestData/test_data.json";

    public RecommendationController(EndPoint endpoint)
    {
        _endpoint = endpoint;
    }
    [HttpGet("Server")]
    public async Task<IActionResult> Start()
    {
        await _endpoint.StartServer();
        return Ok("ServerStarted");
    }
    [HttpGet]
    public IActionResult GetStatus()
    {

        return Ok("Ok");
    }
    [HttpGet("SaveData")]
    public async Task<IActionResult> SaveData()
    {
        await UserDataManager.SaveAsync(_endpoint.AllUserData, FilePath);
        return Ok("Recommendation API is running.");
    }
    [HttpGet("LordData")]
    public async Task<IActionResult> LordData()
    {
        _endpoint.AllUserData = await UserDataManager.LoadAsync(FilePath);
        return Ok("Recommendation API is running.");
    }
    [HttpPost("AddUser")]
    public async Task<IActionResult> AddData()
    {

        // リクエストのボディが空でないか確認
        if (Request.Body == null)
        {
            return BadRequest("Invalid request body.");
        }

        // リクエストボディをバイト配列に読み込む
        byte[] receivedBytes;
        using (var memoryStream = new MemoryStream())
        {
            // Request.Body (ストリーム) の内容を memoryStream にコピー
            await Request.Body.CopyToAsync(memoryStream);
            receivedBytes = memoryStream.ToArray();
        }
        // 受信したデータが空でないか確認
        if (receivedBytes.Length == 0)
        {
            return BadRequest("Received data is empty.");
        }

        try
        {
            // ★★★ バイト配列を処理する新しいメソッドを呼び出す ★★★
            _endpoint.PUTUserData(receivedBytes);
        }
        catch (Exception ex)
        {
            // エラーの詳細をログに出力するとデバッグに役立つ
            // Log.Error(ex, "Failed to process user data.");
            return BadRequest("Invalid user data format or processing failed.");
        }

        return Ok("AddData successful");
    }
    [HttpGet("GetUser")]
    public async Task<IActionResult> GetUserData([FromBody] uint ID)
    {
        try
        {
            OnePersonData personData = new();
            if (_endpoint.AllUserData.TryGetValue(ID, out personData))
            {
                return Ok(personData);
            }
            else
            {
                return BadRequest("Invalid user vector data.");
            }
        }
        catch
        {
            return BadRequest("Invalid user vector data.");
        }
    }
    [HttpPost("GetNear")]
    public async Task<IActionResult> GetNear([FromBody] uint userID)
    {
        uint[] ans = _endpoint.GetNereUserData(userID);
        // ★ オブジェクトを直接Ok()に渡す（自動シリアライズ）
        return Ok(ans);
    }

    [HttpPost("GetReson")]
    public async Task<IActionResult> GetReson([FromBody] uint MangaID)
    {
        int[] ans = _endpoint.GetSortedIndicesByValue(MangaID);
        // ★ オブジェクトを直接Ok()に渡す（自動シリアライズ）
        return Ok(ans);
    }
    [HttpPost("GetCos")]
    public async Task<IActionResult> GetCosPoint([FromBody] CosPointRe reqest)
    {
        if (reqest == null)
        {
            return BadRequest("Invalid user vector data.");
        }
        double ans = _endpoint.CosinPoint(reqest.Target, reqest.OtherUser);
        // ★ オブジェクトを直接Ok()に渡す（自動シリアライズ）
        return Ok(ans);
    }

    [HttpPost("GetComic")]
    public async Task<IActionResult> GetComic([FromBody] uint reqest)
    {
        uint[] ans = _endpoint.GetHighPointManga(reqest);
        return Ok(ans);
    }
[HttpGet("GetImage/{imageName}")]
public async Task<IActionResult> GetImage(string imageName)
{
    // 画像が保存されているディレクトリのパスを設定
    var imagePath = Path.Combine("path/to/your/image/folder", imageName);

    // ファイルが存在するか確認
    if (!System.IO.File.Exists(imagePath))
    {
        return NotFound(); // ファイルが存在しない場合は404エラーを返す
    }

    // ファイルのバイトデータを読み込む
    var imageBytes = await System.IO.File.ReadAllBytesAsync(imagePath);

    // 画像のMIMEタイプを特定（例: .jpgなら "image/jpeg"）
    var mimeType = GetMimeType(imageName);

    // ファイルのバイトデータをHTTPレスポンスとして返す
    return File(imageBytes, mimeType);
}

// ファイル名からMIMEタイプを取得するヘルパーメソッド
private string GetMimeType(string fileName)
{
    var extension = Path.GetExtension(fileName).ToLowerInvariant();
    return extension switch
    {
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".gif" => "image/gif",
        // 他のファイル形式もここに追加
        _ => "application/octet-stream",
    };
}
}