using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StackExchange.Redis;

namespace hello_elasticache.Pages;

public class VoteModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private ConnectionMultiplexer Redis = null!;
    private IDatabase db = null;
    private readonly IConfiguration _config;
    public string Message { get; set; } = "Please Wait";
    public long Count { get; set; } = 0;

    public VoteModel(IConfiguration config, ILogger<IndexModel> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task OnGetAsync(string id)
    {
        try
        {
            var primaryEndpoint = _config["redis:primaryEndpoint"];
            var readerEndpoint = _config["redis:readerEndpoint"];
            Redis = ConnectionMultiplexer.Connect($"{primaryEndpoint},{readerEndpoint}");
            IDatabase db = Redis.GetDatabase();
            Count = await db.StringIncrementAsync(id, 1, CommandFlags.DemandMaster);
            Message = "Vote recorded";
        }
        catch(Exception ex)
        {
            Message = ex.ToString() + "<br/>" + ex.StackTrace;
        }
    }

}
