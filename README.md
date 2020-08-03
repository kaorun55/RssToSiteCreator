# RssToSiteCreator
RssToSiteCreator は、指定された RSS を読み取って、静的 HTML ファイルを生成し、Netlify にデプロイするバッチプログラムです。  
Azure Functions の TimerTrigger で定期実行させるように構成されています。

## サンプルページ
同梱されているサンプルのテンプレートを使用して生成したページは、下記のURLから閲覧できます。

https://rss-to-site-creator-sample.netlify.app/sample.html

## 環境変数
- SCHEDULE_EXPRESSION (既定値: :"0 5 */12 * * *")
  - [NCRONTAB式](https://docs.microsoft.com/ja-jp/azure/azure-functions/functions-bindings-timer?tabs=csharp#ncrontab-expressions)で実行間隔を指定します
- RssSiteUrls
  - 読み取り先 RSS の URL を指定します
  - セミコロン区切りで複数指定に対応します
- SummaryLimit
  - サマリの文字数の上限を指定します
- Site:PostsLimit
  - サイトに出力するポスト数を指定します
- Netlify:AccessToken
  - Netlify のアクセストークンを指定します
  - アクセストークンは、User settings -> Applications -> Personal access tokens から取得できます
- Netlify:SiteId
  - Netlify のデプロイ先サイトID(= API ID)を指定します
  - API ID は、Site settings -> General -> Site information に記載されています