using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ADV.Core;
using System;
using ADV.Data;

namespace ADV.Presentation
{
    /// <summary>
    /// キャラクター表示全体を管理するプレゼンター
    /// </summary>
    public class CharacterPresenter : IDisposable
    {
        private readonly Transform _characterContainer;
        private readonly CharacterView _characterPrefab;

        // アクティブなキャラクター
        private readonly Dictionary<string, CharacterView> _activeCharacters = new();

        private readonly NovelAssetLoader _assetLoader;

        private CancellableTask _operationCancellable;
        private const float DEFAULT_FADE_DURATION = 0.5f;

        public CharacterPresenter(Transform container, CharacterView prefab, NovelAssetLoader assetLoader)
        {
            _characterContainer = container;
            _characterPrefab = prefab;
            _assetLoader = assetLoader;
            _operationCancellable = new CancellableTask();
        }

        /// <summary>
        /// 複数キャラクターを表示
        /// </summary>
        public async UniTask ShowCharacters(
            List<CharacterDisplayData> characters,
            CancellableTask cancellable = null)
        {
            var effectiveCancellable = cancellable ?? _operationCancellable;

            // 新しい画像が揃うまで、現在の表示を維持する。
            var sprites = await UniTask.WhenAll(characters.Select(c =>
                _assetLoader.LoadCharacterAsync(c.Name, c.Expression, effectiveCancellable.Token)));
            if (effectiveCancellable.IsCancellationRequested || _operationCancellable.IsCancellationRequested)
                throw new OperationCanceledException();

            // 現在と新規のキーセット
            var oldKeys = _activeCharacters.Keys.ToHashSet();
            var newKeys = characters
                .Select(c => $"{c.Name}_{c.Expression}")
                .ToHashSet();

            var toRemove = oldKeys.Except(newKeys).ToList();
            var toKeep = oldKeys.Intersect(newKeys).ToList();

            // フェードアウト＆削除
            var fadeOutTasks = toRemove.Select(key =>
            {
                if (_activeCharacters.TryGetValue(key, out var view))
                {
                    return view.FadeOut(DEFAULT_FADE_DURATION, effectiveCancellable);
                }
                return UniTask.CompletedTask;
            }).ToList();

            await UniTask.WhenAll(fadeOutTasks);
            if (effectiveCancellable.IsCancellationRequested || _operationCancellable.IsCancellationRequested)
                throw new OperationCanceledException();

            foreach (var key in toRemove)
            {
                if (_activeCharacters.TryGetValue(key, out var view))
                {
                    UnityEngine.Object.Destroy(view.gameObject);
                    _activeCharacters.Remove(key);
                }
            }

            // 位置計算（n分割）
            int count = characters.Count;
            float spacing = 2560f / (count + 1);
            float startX = -2560f / 2 + spacing;

            var fadeInTasks = new List<UniTask>();
            var newActiveChars = new Dictionary<string, CharacterView>();

            for (int i = 0; i < characters.Count; i++)
            {
                var charData = characters[i];
                string key = $"{charData.Name}_{charData.Expression}";
                Vector3 targetPos = new Vector3(startX + i * spacing, 0, 0);

                if (toKeep.Contains(key) && _activeCharacters.TryGetValue(key, out var existingView))
                {
                    // 既存キャラは位置更新のみ
                    fadeInTasks.Add(existingView.MoveTo(targetPos, DEFAULT_FADE_DURATION * 2, effectiveCancellable));
                    newActiveChars[key] = existingView;
                }
                else
                {
                    // 新規キャラ生成
                    var sprite = sprites[i];

                    if (sprite == null)
                    {
                        Debug.LogError($"[CharacterPresenter] Failed to load sprite: {charData.Name}_{charData.Expression}");
                        continue;
                    }

                    var newView = UnityEngine.Object.Instantiate(_characterPrefab, _characterContainer);
                    newView.transform.localPosition = targetPos;
                    newView.SetSprite(sprite);
                    newView.SetAlpha(0f);

                    newActiveChars[key] = newView;
                    fadeInTasks.Add(newView.FadeIn(charData.FadeTime, effectiveCancellable));
                }
            }

            _activeCharacters.Clear();
            foreach (var kvp in newActiveChars)
            {
                _activeCharacters[kvp.Key] = kvp.Value;
            }

            await UniTask.WhenAll(fadeInTasks);
        }

        /// <summary>
        /// 全キャラクターをクリア
        /// </summary>
        public async UniTask ClearAll(CancellableTask cancellable = null)
        {
            await ShowCharacters(new List<CharacterDisplayData>(), cancellable);
        }

        public void Dispose()
        {
            _operationCancellable?.Dispose();
        }
    }
}
