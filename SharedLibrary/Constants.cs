using System;

namespace SharedLibrary
{
    public static class Constants
    {
        public const string SimilarityKeyPrefix = "SIMILARITY-";
        public const string RankKeyPrefix = "RANK-";
        public const string TextKeyPrefix = "TEXT-";

        public const string RankKeyProcessing = "valuator.processing.rank";
        public const string SimilarityKeyCalculated = "valuator.similarity_calculated";
        public const string RankKeyCalculated = "rank_calculator.rank_calculated";

        public const string ShardIdRus = "DB_RUS";
        public const string ShardIdEu = "DB_EU";
        public const string ShardIdOther = "DB_OTHER";

        public static string HostName
        {
            get
            {
                var hostName = Environment.GetEnvironmentVariable("MACHINE_IP");
                return string.IsNullOrWhiteSpace(hostName) ? "localhost" : hostName;
            }
        }
    }
}