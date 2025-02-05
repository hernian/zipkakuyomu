using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zipkakuyomu
{
    internal class ZipBook
    {
        private class Episode(int episodeNumber, string title, IEnumerable<string> paragraphs)
        {
            public readonly int EpisodeNumber = episodeNumber;
            public readonly string Title = title;
            public readonly IEnumerable<string> Paragraphs = paragraphs;
        }
        
        private readonly List<Episode> _listEpisode = [];
        private int _episodeNumberMax = 0;
        public void AddEpisode(int episodeNumber, string title, IEnumerable<string> paragraphs)
        {
            var episode = new Episode(episodeNumber, title, paragraphs);
            _listEpisode.Add(episode);
            _episodeNumberMax = Math.Max(_episodeNumberMax, episodeNumber);
        }

        public Task SaveAsync(string pathDest, CancellationToken ct)
        {
            return Task.Run(() => SaveInternal(pathDest, ct), ct);
        }

        private void SaveInternal(string pathDest, CancellationToken ct)
        {
            var completed = false;
            var pathTemp = $"{pathDest}.temp";
            try
            {
                ct.ThrowIfCancellationRequested();
                var enc = new UTF8Encoding(false);
                var dirOut = Path.GetDirectoryName(pathDest);
                if (!string.IsNullOrEmpty(dirOut))
                {
                    Directory.CreateDirectory(dirOut);
                }
                if (File.Exists(pathTemp))
                {
                    File.Delete(pathTemp);
                }
                var format = GetEpisodeNumberFormat();
                // zipArchive.Dispose()を呼び出した後にFile.Move()する必要がある
                // なので、旧いusing形式にしてzipArchiveのスコープを明確に制限した
                using (var zipArchive = ZipFile.Open(pathTemp, ZipArchiveMode.Create))
                {
                    foreach (var episode in _listEpisode)
                    {
                        ct.ThrowIfCancellationRequested();
                        var entryName = $"episode_{episode.EpisodeNumber.ToString(format)}.txt";
                        var entry = zipArchive.CreateEntry(entryName);
                        using var entryStream = entry.Open();
                        using var writer = new StreamWriter(entryStream, enc);
                        writer.Write(episode.Title);
                        foreach (var paragraph in episode.Paragraphs)
                        {
                            writer.WriteLine(paragraph);
                        }
                    }
                }
                File.Move(pathTemp, pathDest, true);
                completed = true;
            }
            finally
            {
                if (!completed)
                {
                    try
                    {
                        File.Delete(pathTemp);
                    }
                    catch
                    {
                        // リカバリできることはないので、例外を無視する
                    }
                }
            }
        }

        private string GetEpisodeNumberFormat()
        {
            var sb = new StringBuilder("0");
            for (int i = _episodeNumberMax / 10; i > 0; i /= 10)
            {
                sb.Append('0');
            }
            return sb.ToString();
        }
    }
}
