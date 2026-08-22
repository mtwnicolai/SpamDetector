using Microsoft.ML.Data;

namespace SpamDetector.Data
{
    public class SpamPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool IsSpam { get; set; }

        public float Probability { get; set; }
        public float Score { get; set; }
    }
}
