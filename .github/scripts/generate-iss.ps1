<#
.SYNOPSIS
    .iss テンプレートからバージョン等を置換して Inno Setup スクリプトを生成します。

.PARAMETER Version
    インストーラーに埋め込むバージョン番号 (例: 1.2.3)

.PARAMETER RepoUrl
    GitHub リポジトリの URL (例: https://github.com/cotore-game/MyGame)

.PARAMETER TemplatePath
    .iss テンプレートのパス (省略時: .github/inno/installer.iss.template)

.PARAMETER OutputPath
    生成する .iss ファイルの出力先 (省略時: installer-script.iss)

.EXAMPLE
    .github/scripts/generate-iss.ps1 -Version "1.2.3" -RepoUrl "https://github.com/cotore-game/MyGame"
#>
param(
    [Parameter(Mandatory)] [string]$Version,
    [Parameter(Mandatory)] [string]$RepoUrl,
    [string]$TemplatePath = ".github/inno/installer.iss.template",
    [string]$OutputPath   = "installer-script.iss"
)

if (-not (Test-Path $TemplatePath)) {
    Write-Error "❌ テンプレートが見つかりません: $TemplatePath"
    exit 1
}

$content = Get-Content -Path $TemplatePath -Raw -Encoding UTF8

# プレースホルダーを実際の値に置換
$content = $content -replace '\{\{APP_VERSION\}\}',       $Version
$content = $content -replace '\{\{REPO_URL\}\}',          $RepoUrl
$content = $content -replace '\{\{REPO_ISSUES_URL\}\}',   "$RepoUrl/issues"
$content = $content -replace '\{\{REPO_RELEASES_URL\}\}', "$RepoUrl/releases"

$outputDir = Split-Path $OutputPath -Parent
if ($outputDir -and -not (Test-Path $outputDir)) {
    New-Item -ItemType Directory -Force -Path $outputDir | Out-Null
}

$content | Out-File -FilePath $OutputPath -Encoding UTF8 -NoNewline:$false
Write-Host "✅ Generated: $OutputPath (version: $Version)"
