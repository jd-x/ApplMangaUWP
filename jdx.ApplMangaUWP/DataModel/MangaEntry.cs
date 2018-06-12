using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jdx.ApplMangaUWP.DataModel {
    public class MangaEntry {
        private string _url;
        private string _localFilename;
        private string _title;
        private string _altTitle;
        private string _mangaThumbnail;
        private string _description;
        private int _chapterCount;
        private string _author;
        private string _artist;
        private string _publishingStatus;
        private DateTimeOffset _publishDate;
        private string _formattedPublishDate;

        /// <summary>
        /// Initializes a new instance of the <see cref="MangaEntry"/> class.
        /// </summary>
        public MangaEntry() {
        }
    }
}
