using System;
using System.Threading.Tasks;
using jdx.ApplMangaUWP.Helpers;
using Microsoft.EntityFrameworkCore;
using Windows.Storage;

namespace jdx.ApplMangaUWP.DataModel {
    public class LocalMangaStorageContext : DbContext {
        private static AsyncInitializerHelper<LocalMangaStorageContext> _initializer = new AsyncInitializerHelper<LocalMangaStorageContext>();
        private DbSet<MangaEntry> _mangaEntryCache;

        static LocalMangaStorageContext() => _initializer.InitializeWith(CheckForDatabase);

        public DbSet<MangaEntry> MangaCache {
            get {
                _initializer.CheckInitialized();
                return _mangaEntryCache;
            }

            set => _mangaEntryCache = value;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite("Data Source=applmanga.db");
        }

        private static async Task CheckForDatabase() {
            var dbFileName = "applmanga.db";
            var dbAssetPath = $"ms-appx:///Assets/{dbFileName}";

            var data = ApplicationData.Current.LocalFolder;

            var dbExists = await data.TryGetItemAsync(dbFileName);

            if(dbExists == null) {
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(dbAssetPath)).AsTask().ConfigureAwait(false);
                var database = await file.CopyAsync(data).AsTask().ConfigureAwait(false);
            }
        }
    }
}
