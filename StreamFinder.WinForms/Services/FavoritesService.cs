using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using StreamFinder.WinForms.Models;

namespace StreamFinder.WinForms.Services
{
    public class FavoritesService
    {
        public FavoritesService()
        {
            FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "favorites.json");
        }

        public string FilePath { get; private set; }

        public List<MediaItem> GetAll()
        {
            if (!File.Exists(FilePath))
            {
                return new List<MediaItem>();
            }

            try
            {
                var json = File.ReadAllText(FilePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<MediaItem>();
                }

                return JsonConvert.DeserializeObject<List<MediaItem>>(json) ?? new List<MediaItem>();
            }
            catch
            {
                return new List<MediaItem>();
            }
        }

        public bool IsFavorite(MediaItem media)
        {
            if (media == null)
            {
                return false;
            }

            return GetAll().Any(item => HasSameIdentity(item, media));
        }

        public void Add(MediaItem media)
        {
            if (media == null || IsFavorite(media))
            {
                return;
            }

            var favorites = GetAll();
            favorites.Add(media);
            SaveAll(favorites);
        }

        public void Remove(MediaItem media)
        {
            if (media == null)
            {
                return;
            }

            var favorites = GetAll();
            var removed = favorites.RemoveAll(item => HasSameIdentity(item, media)) > 0;
            if (removed)
            {
                SaveAll(favorites);
            }
        }

        public bool Toggle(MediaItem media)
        {
            if (media == null)
            {
                return false;
            }

            if (IsFavorite(media))
            {
                Remove(media);
                return false;
            }

            Add(media);
            return true;
        }

        private void SaveAll(List<MediaItem> favorites)
        {
            try
            {
                var json = JsonConvert.SerializeObject(favorites ?? new List<MediaItem>(), Formatting.Indented);
                File.WriteAllText(FilePath, json);
            }
            catch (Exception ex)
            {
                throw new FavoritesPersistenceException("Não foi possível salvar os favoritos.", ex);
            }
        }

        private static bool HasSameIdentity(MediaItem first, MediaItem second)
        {
            return first != null && second != null &&
                string.Equals(first.Id, second.Id, StringComparison.Ordinal) &&
                first.MediaType == second.MediaType;
        }
    }

    public class FavoritesPersistenceException : Exception
    {
        public FavoritesPersistenceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
