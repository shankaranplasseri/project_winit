using SQLite;
using VanSalesJourneyPlan.Mobile.Models;

namespace VanSalesJourneyPlan.Mobile.Services;

public interface ILocalCacheService
{
    Task InitializeAsync();
    Task SaveJourneyPlansAsync(List<JourneyPlanCache> plans);
    Task<List<JourneyPlanCache>> GetJourneyPlansAsync();
    Task SaveCustomerAsync(CustomerCache customer);
    Task<CustomerCache?> GetCustomerAsync(int customerId);
    Task SaveVisitLogAsync(VisitLogCache log);
    Task<List<VisitLogCache>> GetVisitLogsAsync(int planItemId);
    Task ClearCacheAsync();
}

public class LocalCacheService : ILocalCacheService
{
    private SQLiteAsyncConnection? _connection;
    private readonly string _dbPath;

    public LocalCacheService()
    {
        _dbPath = Path.Combine(FileSystem.AppDataDirectory, "cache.db3");
    }

    public async Task InitializeAsync()
    {
        if (_connection != null) return;

        _connection = new SQLiteAsyncConnection(_dbPath);
        await _connection.CreateTableAsync<JourneyPlanCache>();
        await _connection.CreateTableAsync<CustomerCache>();
        await _connection.CreateTableAsync<VisitLogCache>();
    }

    public async Task SaveJourneyPlansAsync(List<JourneyPlanCache> plans)
    {
        if (_connection == null) await InitializeAsync();
        
        await _connection!.DeleteAllAsync<JourneyPlanCache>();
        foreach (var plan in plans)
        {
            await _connection.InsertOrReplaceAsync(plan);
        }
    }

    public async Task<List<JourneyPlanCache>> GetJourneyPlansAsync()
    {
        if (_connection == null) await InitializeAsync();
        return await _connection!.Table<JourneyPlanCache>().ToListAsync();
    }

    public async Task SaveCustomerAsync(CustomerCache customer)
    {
        if (_connection == null) await InitializeAsync();
        await _connection!.InsertOrReplaceAsync(customer);
    }

    public async Task<CustomerCache?> GetCustomerAsync(int customerId)
    {
        if (_connection == null) await InitializeAsync();
        return await _connection!.FindAsync<CustomerCache>(customerId);
    }

    public async Task SaveVisitLogAsync(VisitLogCache log)
    {
        if (_connection == null) await InitializeAsync();
        await _connection!.InsertOrReplaceAsync(log);
    }

    public async Task<List<VisitLogCache>> GetVisitLogsAsync(int planItemId)
    {
        if (_connection == null) await InitializeAsync();
        return await _connection!
            .Table<VisitLogCache>()
            .Where(v => v.JourneyPlanItemId == planItemId)
            .ToListAsync();
    }

    public async Task ClearCacheAsync()
    {
        if (_connection == null) await InitializeAsync();
        await _connection!.DeleteAllAsync<JourneyPlanCache>();
        await _connection!.DeleteAllAsync<CustomerCache>();
        await _connection!.DeleteAllAsync<VisitLogCache>();
    }
}

// Cache Models
[Table("JourneyPlans")]
public class JourneyPlanCache
{
    [PrimaryKey]
    public int JourneyPlanId { get; set; }
    public int AssignedUserId { get; set; }
    public string AssignedUserName { get; set; } = string.Empty;
    public string PlanDate { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ItemsJson { get; set; } = "[]"; // JSON array of JourneyPlanItemCache
}

[Table("Customers")]
public class CustomerCache
{
    [PrimaryKey]
    public int CustomerId { get; set; }
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? ContactNumber { get; set; }
    public double? LocationLatitude { get; set; }
    public double? LocationLongitude { get; set; }
    public string? Route { get; set; }
}

[Table("VisitLogs")]
public class VisitLogCache
{
    [PrimaryKey]
    public int VisitLogId { get; set; }
    public int JourneyPlanItemId { get; set; }
    public string VisitDate { get; set; } = string.Empty;
    public string? VisitTime { get; set; }
    public string? Notes { get; set; }
    public decimal? SalesAmount { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class JourneyPlanItemCache
{
    public int JourneyPlanItemId { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerCode { get; set; }
    public string? CustomerName { get; set; }
    public int SequenceNumber { get; set; }
    public string? Notes { get; set; }
    public string? PlannedVisitTime { get; set; }
    public bool IsCompleted { get; set; }
}
