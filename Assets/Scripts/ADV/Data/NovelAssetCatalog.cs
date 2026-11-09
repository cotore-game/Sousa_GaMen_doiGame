using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ADV.Data
{
    /// <summary>台本で使う名前と、Addressablesの画像参照を対応付ける。</summary>
    [CreateAssetMenu(menuName = "ADV/Novel Asset Catalog")]
    public sealed class NovelAssetCatalog : ScriptableObject
    {
        [Serializable]
        public sealed class CharacterEntry
        {
            public string characterName;
            public string expression;
            public AssetReferenceSprite sprite;
        }

        [Serializable]
        public sealed class BackgroundEntry
        {
            public string backgroundName;
            public AssetReferenceSprite sprite;
        }

        [SerializeField]
        private List<CharacterEntry> characters = new();
        [SerializeField]
        private List<BackgroundEntry> backgrounds = new();

        public AssetReferenceSprite GetCharacter(string characterName, string expression)
        {
            foreach (var entry in characters)
                if (entry.characterName == characterName && entry.expression == expression)
                    return entry.sprite;
            throw new KeyNotFoundException($"立ち絵がCatalogにありません: {characterName}/{expression}");
        }

        public AssetReferenceSprite GetBackground(string backgroundName)
        {
            foreach (var entry in backgrounds)
                if (entry.backgroundName == backgroundName)
                    return entry.sprite;
            throw new KeyNotFoundException($"背景がCatalogにありません: {backgroundName}");
        }
    }
}
