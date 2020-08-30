# 事前準備

1. Twitter Developer Portal でプロジェクトを作成しておく
   - Bearer トークンの値をメモっておく
   - Twitter API　(Version 2)を使えるようにしておく
2. node.js (NPM) を使えるようにしておく
3. .NET Core を使えるようにしておく

# 動作確認バージョン

- .NET Core - 3.1.105
- node - ver 14.4.0
- npm - ver 6.14.7

# 開発環境作成手順

git からコードをpull

```
git clone https://github.com/higuhi/HeySearch.git
```

フロントエンドのパッケージをインストール

```
cd HeySearch
cd ClientApp
npm install 
cd ..
```

バックエンド用に Twitter API のBearer トークンを設定 （.NETの Secret Managerを使います）

```
dotnet user-secrets init
dotnet user-secrets set "HeySearch:Twitter:BearerToken" "Twitter API のBearer トークン"
```

サーバー実行

```
dotnet run 
```

ブラウザを開いて、 https://localhost:5001/ を開く

