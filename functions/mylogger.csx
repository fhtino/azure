// Ver. 1.0.0

#r "Microsoft.WindowsAzure.Storage"


using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

// Azure Functions C# script (.csx) developer reference
// https://docs.microsoft.com/en-us/azure/azure-functions/functions-reference-csharp


public class MyLoggerItem : TableEntity
{
    public string Area {get;set;}
    public int Level {get;set;}
    public string CorrelationID {get;set;}
    public double ElapsedFromStart {get;set;}
    public double ElapsedFromPrevious {get;set;}
    public DateTime DT {get;set;}
    public string Message { get; set; }
    public string Details { get; set; }
}


public class MyLogger
{
    private readonly string _azureStorageConnStringEnvVariable = "...";
    private readonly string _azureStorageTableName = "...";


    private string _area;
    private CloudTable _table;
    private string _correlationID;
    private DateTime _startDT;
    private DateTime _previousDT;

    public MyLogger(string area)
    {
        string azureStorageString = System.Environment.GetEnvironmentVariable(
                                        _azureStorageConnStringEnvVariable, 
                                        EnvironmentVariableTarget.Process);
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(azureStorageString);  
        CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
        _table = tableClient.GetTableReference(_azureStorageTableName);

        _correlationID = Guid.NewGuid().ToString().Replace("-","");
        _startDT = DateTime.UtcNow;
        _previousDT = _startDT;
        _area = area;
    }

    public async Task CreateTable()
    {
        await _table.CreateIfNotExistsAsync();
    }

    public async Task Write(int level, string message, string details)
    {
        var dt = DateTime.UtcNow;
        var rk = String.Format("{0:D19}", DateTime.MaxValue.Ticks - dt.Ticks) + 
                 "_" + _correlationID.Substring(0, 8);

        await Write(
            new MyLoggerItem()
            {
                PartitionKey = dt.ToString("yyyyMMdd"),
                RowKey = rk,
                Area = _area,
                Level = level,
                CorrelationID = _correlationID,
                ElapsedFromStart = dt.Subtract(_startDT).TotalSeconds,
                ElapsedFromPrevious = dt.Subtract(_previousDT).TotalSeconds,
                DT = dt,
                Message = message,
                Details = details
            });
        
        _previousDT = dt;
    }

    private async Task Write(MyLoggerItem logItem)
    {
        TableOperation insertOperation = TableOperation.Insert(logItem);
        await _table.ExecuteAsync(insertOperation);
    }
}


