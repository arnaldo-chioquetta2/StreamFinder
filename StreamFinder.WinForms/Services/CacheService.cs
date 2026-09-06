using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace StreamFinder.WinForms.Services
{
    public class CacheService
    {
        public CacheService()
        {
            CacheDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Cache");
        }

        public string CacheDirectory { get; private set; }

        public string GenerateKey(string input)
        {
            if (input == null)
            {
                return string.Empty;
            }

            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                var builder = new StringBuilder(bytes.Length * 2);
                foreach (var value in bytes)
                {
                    builder.Append(value.ToString("x2"));
                }

                return builder.ToString();
            }
        }

        public bool TryGet(string key, int validHours, out string content)
        {
            content = null;
            if (string.IsNullOrWhiteSpace(key) || validHours <= 0)
            {
                return false;
            }

            try
            {
                var filePath = GetCacheFilePath(key);
                if (!File.Exists(filePath))
                {
                    return false;
                }

                var json = File.ReadAllText(filePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return false;
                }

                var entry = JsonConvert.DeserializeObject<CacheEntry>(json);
                if (entry == null || entry.Content == null)
                {
                    return false;
                }

                var age = DateTime.Now - entry.CreatedAt;
                if (age.TotalHours > validHours)
                {
                    return false;
                }

                content = entry.Content;
                return true;
            }
            catch
            {
                content = null;
                return false;
            }
        }

        public void Set(string key, string content)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("A chave do cache não pode ser vazia.", "key");
            }

            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            try
            {
                Directory.CreateDirectory(CacheDirectory);
                var entry = new CacheEntry
                {
                    CreatedAt = DateTime.Now,
                    Content = content
                };
                var json = JsonConvert.SerializeObject(entry, Formatting.Indented);
                File.WriteAllText(GetCacheFilePath(key), json);
            }
            catch (Exception ex)
            {
                throw new CachePersistenceException("Não foi possível salvar o cache.", ex);
            }
        }

        public void Clear()
        {
            try
            {
                if (!Directory.Exists(CacheDirectory))
                {
                    return;
                }

                foreach (var filePath in Directory.GetFiles(CacheDirectory, "*.cache", SearchOption.TopDirectoryOnly))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                throw new CachePersistenceException("Não foi possível limpar o cache.", ex);
            }
        }

        private string GetCacheFilePath(string key)
        {
            var safeKey = GenerateKey(key);
            return Path.Combine(CacheDirectory, safeKey + ".cache");
        }

        private sealed class CacheEntry
        {
            public DateTime CreatedAt { get; set; }
            public string Content { get; set; }
        }
    }

    public class CachePersistenceException : Exception
    {
        public CachePersistenceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
