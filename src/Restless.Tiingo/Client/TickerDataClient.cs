using CsvHelper;
using Restless.Tiingo.Core;
using Restless.Tiingo.Data;
using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Restless.Tiingo.Client
{
    /// <summary>
    /// Represents a client for obtaining ticker data via the supported tickers zip file
    /// </summary>
    /// <remarks>
    /// This class provides access to the supported_tickers.zip file published and updated by Tiingo.
    /// Another alternative for obtaining information about supported tickers is via the search api
    /// which can be accessed using the <see cref="SearchClient"/>.
    /// </remarks>
    public class TickerDataClient : BaseClient
    {
        private const string TickerZipUrl = "https://apimedia.tiingo.com/docs/tiingo/daily/supported_tickers.zip";
        //private const string TickerTempFile = "TICKERS-DB6E92E3-59AC-40DD-8A06-279950AC7CB9";
        private const string TickerTempFile = "supported_tickers.zip";
        private const int MaxTempFileAgeMinutes = 60 * 24;

        internal TickerDataClient(IHttpClientWrapper client, string apiToken) : base(client, apiToken)
        {
        }

        /// <summary>
        /// Gets ticker definitions
        /// </summary>
        /// <param name="parms">The operation parms</param>
        /// <returns>A <see cref="TickerDefinitionCollection"/></returns>
        /// <remarks>
        /// This method downloads the ticker definition file from Tiiango and extracts the contents.
        /// Subsequent calls to this method use a cached copy of the file saved in the system temp
        /// directory. The cached file will continue to be used until it has aged 24 hours.
        /// If you want to force the download to happen before the aging period has elapsed,
        /// call the <see cref="RemoveDefinitionCacheFile"/> method.
        /// </remarks>
        public async Task<TickerDefinitionCollection> GetDefinitionsAsync(TickerDefinitionParameters parms)
        {
            TickerDefinitionCollection result = new();

            byte[] bytes = await GetZipByteArrayAsync();

            using (MemoryStream stream = new(bytes))
            {
                using (ZipArchive zip = new(stream))
                {
                    foreach (ZipArchiveEntry item in zip.Entries)
                    {
                        if (item.Name.EndsWith(".csv"))
                        {
                            PopulateFromZipItem(item, result, parms);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Removes the ticker definition cache file
        /// </summary>
        public void RemoveDefinitionCacheFile()
        {
            string fileName = GetCacheFileName();
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }

        private void PopulateFromZipItem(ZipArchiveEntry item, TickerDefinitionCollection result, TickerDefinitionParameters parms)
        {
            using (Stream itemStream = item.Open())
            {
                using (StreamReader reader = new(itemStream))
                {
                    using (CsvReader csv = new(reader, CultureInfo.InvariantCulture))
                    {
                        foreach (TickerDefinition def in csv.GetRecords<TickerDefinition>())
                        {
                            if (def.Include(parms))
                            {
                                result.Add(def);
                                if (result.Count == parms.Limit)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private async Task<byte[]> GetZipByteArrayAsync()
        {
            string fileName = GetCacheFileName();

            if (File.Exists(fileName))
            {
                return File.ReadAllBytes(fileName);
                /*
                                DateTime date = File.GetLastWriteTime(fileName);
                                if ((DateTime.Now - date).Minutes < MaxTempFileAgeMinutes)
                                {
                                    return File.ReadAllBytes(fileName);
                                }
                */
            }

            byte[] bytes = await GetRawByteArrayAsync(TickerZipUrl);
            File.WriteAllBytes(fileName, bytes);
            return bytes;
        }

        private string GetCacheFileName()
        {
            //return Path.Combine(Path.GetTempPath(), TickerTempFile);
            return Path.Combine("C:\\Users\\Internet\\Downloads", TickerTempFile);
        }
    }
}