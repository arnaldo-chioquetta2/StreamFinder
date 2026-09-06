using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using StreamFinder.WinForms.Models;

namespace StreamFinder.WinForms.Services
{
    public class SearchHistoryService
    {
        private const int MaxHistoryItems = 100;

        public SearchHistoryService()
        {
            FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "history.json");
        }

        public string FilePath { get; private set; }

        public List<SearchHistoryItem> GetAll()
        {
            if (!File.Exists(FilePath))
            {
                return new List<SearchHistoryItem>();
            }

            try
            {
                var json = File.ReadAllText(FilePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<SearchHistoryItem>();
                }

                var items = JsonConvert.DeserializeObject<List<SearchHistoryItem>>(json);
                return (items ?? new List<SearchHistoryItem>())
                    .Where(item => item != null)
                    .OrderByDescending(item => item.SearchedAt)
                    .ToList();
            }
            catch
            {
                return new List<SearchHistoryItem>();
            }
        }

        public void Add(string query, string searchType)
        {
            var normalizedQuery = query == null ? string.Empty : query.Trim();
            if (string.IsNullOrWhiteSpace(normalizedQuery))
            {
                return;
            }

            var normalizedSearchType = searchType ?? string.Empty;
            var items = GetAll();
            var mostRecent = items.FirstOrDefault();
            if (mostRecent != null &&
                string.Equals(mostRecent.Query, normalizedQuery, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(mostRecent.SearchType, normalizedSearchType, StringComparison.Ordinal))
            {
                mostRecent.SearchedAt = DateTime.Now;
            }
            else
            {
                items.Insert(0, new SearchHistoryItem
                {
                    Query = normalizedQuery,
                    SearchType = normalizedSearchType,
                    SearchedAt = DateTime.Now
                });
            }

            SaveAll(items.Take(MaxHistoryItems).ToList());
        }

        public void Clear()
        {
            SaveAll(new List<SearchHistoryItem>());
        }

        private void SaveAll(List<SearchHistoryItem> items)
        {
            try
            {
                var json = JsonConvert.SerializeObject(items ?? new List<SearchHistoryItem>(), Formatting.Indented);
                File.WriteAllText(FilePath, json);
            }
            catch (Exception ex)
            {
                throw new SearchHistoryPersistenceException("Não foi possível salvar o histórico de pesquisas.", ex);
            }
        }
    }

    public class SearchHistoryPersistenceException : Exception
    {
        public SearchHistoryPersistenceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
