# ノベル素材

`NovelAssetCatalog.asset` のInspectorで、CSVのキャラクター名・表情名・背景名と画像を対応付ける。画像参照には `AssetReferenceSprite` を使う。

画像を追加する場合は、Spriteとしてインポートし、Addressables Groupsの `Novel Sprites` へ登録してからCatalogに追加する。立ち絵は `Assets/Sprites/Novel`、背景は `Assets/Sprites/Backgrounds` に置く。CSVの名前と画像ファイル名は一致しなくてもよい。

ADVとADVDevの `AdvScenarioExecutor` にCatalogを設定する。`NovelAssetLoader` は初回表示時に非同期ロードし、ロード中の処理と結果を共有する。CSVの分岐・終了では保持し、ADVシーン破棄時にハンドルを解放する。

Editorの通常確認にはAddressablesのPlay Mode Scriptで `Use Asset Database` を使う。Bundleからの確認は、対象プラットフォームで `Build > New Build > Default Build Script` を実行し、`Use Existing Build` に切り替える。Playerビルド時にはAddressablesコンテンツも自動ビルドする設定。

このCatalogの対象は立ち絵と背景。シナリオCSVは `Resources/Datas` で管理する。
