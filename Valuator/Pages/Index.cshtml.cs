using System;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SharedLibrary;

namespace Valuator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IMessageBroker _messageBroker;
        private readonly IStorage _storage;

        public IndexModel(ILogger<IndexModel> logger, IStorage storage, IMessageBroker messageBroker)
        {
            _storage = storage;
            _messageBroker = messageBroker;
            _logger = logger;
        }

        public IActionResult OnPost(string text)
        {
            _logger.LogDebug(text);

            if (string.IsNullOrEmpty(text)) Redirect("/");

            var id = Guid.NewGuid().ToString();

            //Подсчёт similarity и сохранение в БД по ключу similarityKey
            var similarity = GetSimilarity(text);
            _storage.Store(Constants.SimilarityKeyPrefix + id, similarity.ToString());

            _messageBroker.Publish(Constants.SimilarityKeyCalculated,
                JsonSerializer.Serialize(new Similarity {Id = id, Value = similarity}));

            //Сохраение в БД
            _storage.Store(Constants.TextKeyPrefix + id, text);

            _messageBroker.Publish(Constants.RankKey, id);

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