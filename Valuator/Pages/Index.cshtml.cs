using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SharedLibrary;

namespace Valuator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IMessageBroker _messageBroker;
        private readonly IStorage _storage;

        public IndexModel(IStorage storage, IMessageBroker messageBroker)
        {
            _storage = storage;
            _messageBroker = messageBroker;
        }

        public IActionResult OnPost(string text)
        {
            if (string.IsNullOrEmpty(text)) Redirect("/");

            var id = Guid.NewGuid().ToString();

            //Подсчёт similarity и сохранение в БД по ключу similarityKey
            _storage.Store(Constants.SimilarityKeyPrefix + id, GetSimilarity(text).ToString());

            //Сохраение в БД
            _storage.Store(Constants.TextKeyPrefix + id, text);

            //Подсчёт rank и сохранение в БД по ключу rankKey
            _messageBroker.Publish(Constants.RankKeyPrefix, id);

            return Redirect($"summary?id={id}");
        }

        private int GetSimilarity(string text)
        {
            var keys = _storage.GetKeys();

            return keys.Any(item => item.Substring(0, 5) == Constants.TextKeyPrefix && _storage.Load(item) == text)
                ? 1
                : 0;
        }
    }
}