using Microsoft.ML.Data;

namespace SpamDetector.Data
{
    public class SpamData
    {
        [LoadColumn(0)]
        public bool label { get; set; }
        [LoadColumn(1)]
        public string Menssage { get; set; }
    }
}
