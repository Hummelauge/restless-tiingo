namespace Restless.Tiingo.Core
{
    /// <summary>
    /// Provides parameters for use when obtaining ticker definitions
    /// </summary>
    public class TickerDefinitionParameters
    {
        /// <summary>
        /// The asset type desired, or null for any asset type.
        /// Case insensitive, accepts partial match such as "mutual" for "Mutual Fund"
        /// </summary>
        public string AssetType { get; set; }

        /// <summary>
        /// The currency desired, or null for any currency.
        /// Case insensitive, does not accept partial match.
        /// The default value of this property is "USD"
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// The exchange desired, or null for any exchange.
        /// Case insensitive, accepts partial match
        /// </summary>
        public string Exchange { get; set; }

        /// <summary>
        /// Whether to ignore tickers with null start or end dates
        /// which may indicate a delisted ticker. The default value 
        /// of this property is true.
        /// </summary>
        public bool IgnoreNullDates { get; set; }

        /// <summary>
        /// The maximum number of items to return, or zero for no limit
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// The ticker desired
        /// </summary>
        public string Ticker { get; set; }

        public TickerDefinitionParameters()
        {
            //Currency = "USD";
            IgnoreNullDates = true;
        }
    }
}