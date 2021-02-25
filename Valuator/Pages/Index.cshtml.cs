﻿using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Valuator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IStorage _storage;

        public IndexModel(IStorage storage)
        {
            _storage = storage;
        }

        public IActionResult OnPost(string text)
        {
            if (string.IsNullOrEmpty(text)) Redirect("/");

            var id = Guid.NewGuid().ToString();

            var similarityKey = Constants.SimilarityKeyPrefix + id;
            //Подсчёт similarity и сохранение в БД по ключу similarityKey
            var similarity = GetSimilarity(text);
            _storage.Store(similarityKey, similarity.ToString());

            var textKey = Constants.TextKeyPrefix + id;
            //Сохраение в БД text по ключу textKey
            _storage.Store(textKey, text);

            var rankKey = Constants.RankKeyPrefix + id;
            //Подсчёт rank и сохранение в БД по ключу rankKey
            _storage.Store(rankKey, GetRank(text));
            return Redirect($"summary?id={id}");
        }

        private int GetSimilarity(string text)
        {
            var keys = _storage.GetKeys();

            return keys.Any(item => item.Substring(0, 5) == Constants.TextKeyPrefix && _storage.Load(item) == text)
                ? 1
                : 0;
        }

        private static string GetRank(string text)
        {
            var nonLetterCount = text.Count(x => !char.IsLetter(x));

            return (nonLetterCount / text.Length).ToString(CultureInfo.CurrentCulture);
        }
    }
}