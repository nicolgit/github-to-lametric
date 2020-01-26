
using System;
using System.Collections.Generic;

namespace github_to_lametric.Models
{
    public class Author
    {
        public string name { get; set; }
        public string uri { get; set; }
    }

    public class Entry
    {
        public string id { get; set; }
        public string title { get; set; }
        public DateTime updated { get; set; }
        public Author author { get; set; }
    }

    public class Feed
    {
        public string id { get; set; }
        public string title { get; set; }
        public DateTime updated { get; set; }
        public List<Entry> entry { get; set; }
    }

    public class AtomRootObject
    {
        public Feed feed { get; set; }
    }
}