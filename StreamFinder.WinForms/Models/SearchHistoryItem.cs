using System;

namespace StreamFinder.WinForms.Models
{
    public class SearchHistoryItem
    {
        public string Query { get; set; }
        public DateTime SearchedAt { get; set; }
        public string SearchType { get; set; }
    }
}
