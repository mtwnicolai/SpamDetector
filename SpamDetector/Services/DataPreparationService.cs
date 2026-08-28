using Microsoft.ML;
using SpamDetector.Data;

namespace SpamDetector.Services
{
    public class DataPreparationService
    {
        private readonly MLContext _mlContext;

        public DataPreparationService(MLContext mlContext)
        {
            _mlContext = mlContext;
        }

        public (IDataView TrainSet, IDataView TestSet) PrepareData(string dataPath)
        {
            var random = new Random(42);

            var data = _mlContext.Data.LoadFromTextFile<SpamData>(
                dataPath,
                hasHeader: true,
                separatorChar: ','
            );

            var dataList = _mlContext.Data
                .CreateEnumerable<SpamData>(data, reuseRowObject: false)
                .ToList();

            var spam = dataList
                .Where(x => x.label)
                .OrderBy(x => random.Next())
                .ToList();

            var notSpam = dataList
                .Where(x => !x.label)
                .OrderBy(x => random.Next())
                .ToList();

            var spamTest = spam.Take(20);
            var spamTrain = spam.Skip(20);

            var notSpamTest = notSpam.Take(20);
            var notSpamTrain = notSpam.Skip(20);

            var trainList = spamTrain
                .Concat(notSpamTrain)
                .ToList();

            var testList = spamTest
                .Concat(notSpamTest)
                .ToList();

            var trainSet = _mlContext.Data.LoadFromEnumerable(trainList);
            var testSet = _mlContext.Data.LoadFromEnumerable(testList);

            return (trainSet, testSet);
        }
    }
}
