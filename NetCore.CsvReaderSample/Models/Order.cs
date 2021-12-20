using System;

namespace NetCore.CsvReaderSample.Models
{
    public enum Status
    {
        UNKNOWN_STATUS,
        FILLED,
        CANCELED
    }

    public enum Side
    {
        BUY,
        SELL
    }

    public class Order
    {
        [CsvProperty("Date(UTC)")]
        public DateTime DateUTC { get; set; }
        public string Pair { get; set; }
        public Side Side { get; set; }

        [CsvProperty("Trading total")]
        public string TradingTotal { get; set; }
        public Status Status { get; set; }
        public string Executed { get; set; }

        [CsvProperty("Order Price")]
        public double OrderPrice { get; set; }
    }
}
