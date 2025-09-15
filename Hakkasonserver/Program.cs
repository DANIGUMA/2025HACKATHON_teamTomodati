var builder = WebApplication.CreateBuilder(args);

// サービスを登録
// ★ ここにサービスを登録する ★
builder.Services.AddControllers();
builder.Services.AddSingleton<EndPoint>();
var app = builder.Build();

//app.UseHttpsRedirection();
app.MapControllers();

app.Run();