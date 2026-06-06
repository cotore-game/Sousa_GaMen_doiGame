<#
.SYNOPSIS
    Unity ビルド後の開発専用フォルダを削除します。

.PARAMETER BuildPath
    クリーンアップ対象のビルドディレクトリパス。

.EXAMPLE
    .github/scripts/cleanup-build.ps1 -BuildPath "build/StandaloneWindows64"
#>
param(
    [Parameter(Mandatory)]
    [string]$BuildPath
)

if (-not (Test-Path $BuildPath)) {
    Write-Error "❌ BuildPath が見つかりません: $BuildPath"
    exit 1
}

$patterns = @(
    "*DoNotShip*",
    "*_BackUpThisFolder_ButDontShipItWithYourGame*"
)

$removed = 0

foreach ($pattern in $patterns) {
    Get-ChildItem -Path $BuildPath -Recurse -Directory |
        Where-Object { $_.Name -like $pattern } |
        ForEach-Object {
            Write-Host "🗑️  Removing: $($_.FullName)"
            Remove-Item -Path $_.FullName -Recurse -Force
            $removed++
        }
}

if ($removed -eq 0) {
    Write-Host "ℹ️  削除対象のフォルダが見つかりませんでした"
} else {
    Write-Host "✅ $removed 個のフォルダを削除しました"
}
