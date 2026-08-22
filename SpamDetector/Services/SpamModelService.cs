using Microsoft.ML;
using SpamDetector.Data;

namespace SpamDetector.Services
{
    public class SpamModelService
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _model;

        public SpamModelService()
        {
            _mlContext = new MLContext(seed: 42);

            var dataPath = Path.Combine(
                AppContext.BaseDirectory,
                "Data",
                "spam-data.csv"
                );

            IDataView data = _mlContext.Data.LoadFromTextFile<SpamData>(
                dataPath,
                hasHeader: true,
                separatorChar: ','
                );

            var split = _mlContext.Data.TrainTestSplit(
                data,
                testFraction: 0.2
                );

            var pipeline = _mlContext.Transforms.Text.FeaturizeText(
                    outputColumnName: "Features",
                    inputColumnName: nameof(SpamData.Menssage))
                .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                    labelColumnName: nameof(SpamData.label),
                    featureColumnName: "Features"
                    )
                );

            _model = pipeline.Fit(split.TrainSet);

            var predictions = _model.Transform(split.TestSet);

            var metrics = _mlContext.BinaryClassification.Evaluate(
                predictions,
                labelColumnName: nameof(SpamData.label)    
                );

            Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
        }

        public SpamPrediction Predict(string message)
        {
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<SpamData, SpamPrediction>(_model);

            return predictionEngine.Predict(new SpamData { 
                Menssage = message 
            });
        }
    }
}
