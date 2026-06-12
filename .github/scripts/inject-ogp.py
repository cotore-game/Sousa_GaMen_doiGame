#!/usr/bin/env python3
"""
OGP / SNS プレビュー meta タグを Unity WebGL の index.html に注入するスクリプト。

.github/index/meta.yml が存在しない、または enabled: false の場合は何もしない。
LFS ポインタのままのファイルは自動検出してスキップし、ビルドを失敗させない。
"""

import argparse
import os
import re
import shutil
import sys
import pathlib


def parse_meta_yml(path: str) -> dict:
    """
    YAML パーサー不要の簡易パーサー。
    対応形式: key: value (クォートあり/なし)
    """
    result = {}
    with open(path, encoding="utf-8") as f:
        for line in f:
            line = line.strip()
            if not line or line.startswith("#"):
                continue
            if ":" not in line:
                continue
            key, _, val = line.partition(":")
            val = val.strip().strip('"\'')
            result[key.strip()] = val
    return result


def is_lfs_pointer(path: str) -> bool:
    """ファイルが LFS ポインタかどうか判定する。"""
    try:
        size = os.path.getsize(path)
        if size > 512:
            return False
        with open(path, "rb") as f:
            head = f.read(512)
        return b"oid sha256:" in head
    except OSError:
        return False


def build_meta_block(cfg: dict, repo_url: str, icon_copied: bool) -> str:
    """注入する meta タグブロックを組み立てる。"""
    tags = []
    title = cfg.get("title", "")
    description = cfg.get("description", "")
    icon_file = cfg.get("icon_file", "")
    theme_color = cfg.get("theme_color", "")

    if title:
        tags.append(f'  <meta property="og:title" content="{title}">')
        tags.append(f'  <meta name="twitter:title" content="{title}">')
    if description:
        tags.append(f'  <meta name="description" content="{description}">')
        tags.append(f'  <meta property="og:description" content="{description}">')
        tags.append(f'  <meta name="twitter:description" content="{description}">')

    tags.append(f'  <meta property="og:type" content="website">')
    tags.append(f'  <meta property="og:url" content="{repo_url}">')
    tags.append(f'  <meta name="twitter:card" content="summary_large_image">')

    if icon_copied and icon_file:
        img_url = f"{repo_url.rstrip('/')}/{icon_file}"
        tags.append(f'  <meta property="og:image" content="{img_url}">')
        tags.append(f'  <meta name="twitter:image" content="{img_url}">')
        tags.append(f'  <link rel="icon" href="{icon_file}">')

    if theme_color:
        tags.append(f'  <meta name="theme-color" content="{theme_color}">')

    return "\n".join(tags) + "\n"


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--meta",     required=True, help=".github/index/meta.yml のパス")
    parser.add_argument("--index",    required=True, help="対象の index.html のパス")
    parser.add_argument("--assets",   required=True, help="アイコン画像の元ディレクトリ")
    parser.add_argument("--out-dir",  required=True, help="アイコン画像のコピー先ディレクトリ")
    parser.add_argument("--repo-url", required=True, help="GitHub Pages のベース URL")
    args = parser.parse_args()

    # meta.yml が存在しない場合はスキップ
    if not os.path.exists(args.meta):
        print(f"{args.meta} が存在しないため OGP 注入をスキップします")
        return 0

    cfg = parse_meta_yml(args.meta)

    # enabled: false ならスキップ
    if cfg.get("enabled", "true").lower() == "false":
        print("enabled: false のため OGP 注入をスキップします")
        return 0

    # アイコン画像の処理
    icon_file = cfg.get("icon_file", "")
    icon_enabled = cfg.get("icon", "true").lower() != "false"
    icon_copied = False

    if icon_enabled and icon_file:
        src = os.path.join(args.assets, icon_file)
        dst = os.path.join(args.out_dir, icon_file)
        if not os.path.exists(src):
            print(f"{src} が見つかりません（アイコンをスキップ）")
        elif is_lfs_pointer(src):
            print(f"{src} が LFS ポインタのままです（checkout に lfs: true が必要）")
            print("   アイコンなしで OGP タグを注入します")
        else:
            shutil.copy2(src, dst)
            size = os.path.getsize(src)
            print(f"アイコン画像をコピー: {icon_file} ({size:,} bytes)")
            icon_copied = True

    # meta タグブロックを組み立て
    block = build_meta_block(cfg, args.repo_url, icon_copied)

    # index.html に注入
    index_path = pathlib.Path(args.index)
    if not index_path.exists():
        print(f"{args.index} が見つかりません", file=sys.stderr)
        return 1

    html = index_path.read_text(encoding="utf-8")
    if not re.search(r"(?i)</head>", html):
        print("index.html に </head> タグが見つかりません", file=sys.stderr)
        return 1

    patched = re.sub(r"(?i)(</head>)", block + r"\1", html, count=1)

    # バックアップを作成してから書き込み
    backup = index_path.with_suffix(".html.bak")
    shutil.copy2(index_path, backup)
    try:
        index_path.write_text(patched, encoding="utf-8")
        backup.unlink()
        tag_count = block.count("<meta") + block.count("<link")
        print(f"OGP meta タグを {tag_count} 件注入しました")
    except Exception as e:
        print(f"書き込み失敗: {e} → バックアップから復元します", file=sys.stderr)
        shutil.copy2(backup, index_path)
        backup.unlink()
        return 1

    return 0


if __name__ == "__main__":
    sys.exit(main())
