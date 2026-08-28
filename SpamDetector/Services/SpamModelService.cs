using Microsoft.ML;
using SpamDetector.Data;

namespace SpamDetector.Services
{
    public class SpamModelService
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _model;

        private readonly DataPreparationService _dataPreparationService;

        public SpamModelService(
            MLContext mlContext,
            DataPreparationService dataPreparationService)
        {
            _mlContext = new MLContext(seed: 42);
            _dataPreparationService = dataPreparationService;

            var dataPath = Path.Combine(
                AppContext.BaseDirectory,
                "Data",
                "spam-data.csv"
            );

            var (trainSet, testSet) = _dataPreparationService.PrepareData(dataPath);

            var pipeline = _mlContext.Transforms.Text.FeaturizeText(
                    outputColumnName: "Features",
                    inputColumnName: nameof(SpamData.Menssage))
                .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                    labelColumnName: nameof(SpamData.label),
                    featureColumnName: "Features"
                    )
                );

            _model = pipeline.Fit(trainSet);

            var predictions = _model.Transform(testSet);

            var metrics = _mlContext.BinaryClassification.Evaluate(
                predictions,
                labelColumnName: nameof(SpamData.label)    
                );

            Console.WriteLine("Avaliação:");
            Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
            Console.WriteLine($"Precision: {metrics.PositivePrecision:P2}");
            Console.WriteLine($"Recall: {metrics.PositiveRecall:P2}");
            Console.WriteLine($"F1 Score: {metrics.F1Score:P2}");

            Console.WriteLine();
            Console.WriteLine("Matriz de Confusão:");
            Console.WriteLine($"Verdadeiro Negativo: {metrics.ConfusionMatrix.Counts[0][0]}");
            Console.WriteLine($"Falso Negativo: {metrics.ConfusionMatrix.Counts[0][1]}");
            Console.WriteLine($"Falso Positivo: {metrics.ConfusionMatrix.Counts[1][0]}");
            Console.WriteLine($"Verdadeiro Positivo: {metrics.ConfusionMatrix.Counts[1][1]}");

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
