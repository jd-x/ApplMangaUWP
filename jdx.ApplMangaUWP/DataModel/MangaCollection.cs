using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jdx.ApplMangaUWP.DataModel {
    public class MangaCollection {
        public Uri Uri { get; internal set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public string Artist { get; set; }

        public Uri ImageUri { get; set; }

        public MangaCollection(Uri mangaUri, string title, string description, string author, string artist, Uri imageUri) {
            Uri = mangaUri;
            Title = title;
            Description = description;
            Author = author;
            Artist = artist;
            ImageUri = imageUri;
        }
    }
}