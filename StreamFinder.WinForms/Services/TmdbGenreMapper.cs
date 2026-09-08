using System.Collections.Generic;
using StreamFinder.WinForms.Models;

namespace StreamFinder.WinForms.Services
{
    public static class TmdbGenreMapper
    {
        private static readonly Dictionary<int, string> MovieGenres = new Dictionary<int, string>
        {
            { 28, "Ação" },
            { 12, "Aventura" },
            { 16, "Animação" },
            { 35, "Comédia" },
            { 80, "Crime" },
            { 99, "Documentário" },
            { 18, "Drama" },
            { 10751, "Família" },
            { 14, "Fantasia" },
            { 36, "História" },
            { 27, "Terror" },
            { 10402, "Música" },
            { 9648, "Mistério" },
            { 10749, "Romance" },
            { 878, "Ficção científica" },
            { 10770, "Filme para TV" },
            { 53, "Thriller" },
            { 10752, "Guerra" },
            { 37, "Faroeste" }
        };

        private static readonly Dictionary<int, string> TvGenres = new Dictionary<int, string>
        {
            { 10759, "Ação e aventura" },
            { 16, "Animação" },
            { 35, "Comédia" },
            { 80, "Crime" },
            { 99, "Documentário" },
            { 18, "Drama" },
            { 10751, "Família" },
            { 10762, "Infantil" },
            { 9648, "Mistério" },
            { 10763, "Notícias" },
            { 10764, "Reality" },
            { 10765, "Ficção científica e fantasia" },
            { 10766, "Novela" },
            { 10767, "Talk show" },
            { 10768, "Guerra e política" },
            { 37, "Faroeste" }
        };

        public static List<string> Map(MediaType mediaType, IEnumerable<int> genreIds)
        {
            var genres = new List<string>();
            if (genreIds == null)
            {
                return genres;
            }

            var map = mediaType == MediaType.Movie ? MovieGenres : TvGenres;
            foreach (var genreId in genreIds)
            {
                string genreName;
                if (map.TryGetValue(genreId, out genreName) && !genres.Contains(genreName))
                {
                    genres.Add(genreName);
                }
            }

            return genres;
        }
    }
}
