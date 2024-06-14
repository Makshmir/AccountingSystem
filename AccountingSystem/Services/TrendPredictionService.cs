using AccountingSystem.Models;
using Microsoft.ML;
using System.Globalization;
using CsvHelper;

public class TrendPredictionService
{
    private readonly MLContext _mlContext;
    private ITransformer _model;
    private const string ModelPath = "model.zip";
    private const string DataPath = "salesData.csv"; 

    public TrendPredictionService()
    {
        _mlContext = new MLContext();
        _model = LoadOrTrainModel(new List<SalesData>());
    }

    private ITransformer LoadOrTrainModel(List<SalesData> newSalesData)
    {
        ITransformer model = null;
        if (File.Exists(ModelPath))
        {
            using var stream = new FileStream(ModelPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            model = _mlContext.Model.Load(stream, out var modelInputSchema);
        }

        List<SalesData> previousSalesData = new List<SalesData>();
        if (File.Exists(DataPath))
        {
            previousSalesData = LoadSalesDataFromCsv(DataPath);
        }

        List<SalesData> combinedSalesData = previousSalesData.Concat(newSalesData).ToList();

        if (combinedSalesData.Count == 0)
        {
            Console.WriteLine("No sales data available for training.");
            return model;
        }

        var dataView = _mlContext.Data.LoadFromEnumerable(combinedSalesData);
        var pipeline = _mlContext.Transforms.CopyColumns("Label", "TotalSales")
            .Append(_mlContext.Transforms.Categorical.OneHotEncoding("Category"))
            .Append(_mlContext.Transforms.Concatenate("Features", "Category"))
            .Append(_mlContext.Regression.Trainers.Sdca(labelColumnName: "Label", featureColumnName: "Features"));

        model = pipeline.Fit(dataView);

        var transformedData = model.Transform(dataView);
        var previewTransformedData = transformedData.Preview();

        //foreach (var row in previewTransformedData.RowView)
        //{
        //    Console.WriteLine($"Features: {string.Join(",", row.Values.Where(v => v.Key == "Features").Select(v => v.Value))}");
        //}

        var predictions = model.Transform(dataView);
        var metrics = _mlContext.Regression.Evaluate(predictions, "Label", "Score");
        _mlContext.Model.Save(model, dataView.Schema, "model.zip");
        SaveSalesDataToCsv(combinedSalesData, DataPath);
        Console.WriteLine("Model saved successfully.");
        return model;
    }

    public ITransformer TrainModel(List<SalesData> salesData)
    {
        

        if (salesData == null || salesData.Count == 0)
        {
            Console.WriteLine("No sales data available for training.");
            return null;
        }

        _model = LoadOrTrainModel(salesData);
        return _model;
    }

    public float Predict(string category)
    {
        if (_model == null)
        {
            Console.WriteLine("Model is not trained.");
            return 0f;
        }

        var predictionFunction = _mlContext.Model.CreatePredictionEngine<SalesData, SalesPrediction>(_model);
        var sample = new SalesData { Category = category };
        var result = predictionFunction.Predict(sample);
        //Console.WriteLine($"Category: {category}, Score: {result.Score}");
        return result.Score;
    }

    private void SaveSalesDataToCsv(List<SalesData> salesData, string path)
    {
        using (var writer = new StreamWriter(path))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(salesData);
        }
    }

    private List<SalesData> LoadSalesDataFromCsv(string path)
    {
        using (var reader = new StreamReader(path))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            return csv.GetRecords<SalesData>().ToList();
        }
    }
}
