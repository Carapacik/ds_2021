using System;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Valuator.Pages
{
    public class Summary : PageModel
    {
        private readonly IStorage _storage;

        public Summary(IStorage storage)
        {
            _storage = storage;
        }

        public double Rank { get; set; }
        public double Similarity { get; set; }

        public void OnGet(string id)
        {
            //Инициализация свойства Rank и Similarity с сохранёнными в БД значениями
            Rank = Convert.ToDouble(_storage.Load(Constants.RankKeyPrefix + id));
            Similarity = Convert.ToDouble(_storage.Load(Constants.SimilarityKeyPrefix + id));
        }
    }
}