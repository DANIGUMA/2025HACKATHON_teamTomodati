# ----------------------------------------------------
# ステージ1: アプリケーションをビルドする
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# プロジェクトの依存関係を先にコピーしてキャッシュを最適化
# .csprojファイルだけをコピーして、dotnet restoreを実行
COPY ["Hakkason.csproj", "."]
RUN dotnet restore

# すべてのソースコードをコンテナにコピー
COPY . .

# アプリケーションをビルドして発行
RUN dotnet publish "Hakkason.csproj" -c Release -o /app

# ----------------------------------------------------
# ステージ2: 実行環境を構築する
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 80

# ビルドステージから発行されたファイルをコピー
COPY --from=build /app .

# コンテナが起動したときに実行されるコマンド
ENTRYPOINT ["dotnet", "Hakkason.dll"]