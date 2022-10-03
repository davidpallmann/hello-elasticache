using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StackExchange.Redis;
using System.Web;

namespace hello_elasticache.Pages;

public class VotesModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private ConnectionMultiplexer Redis = null!;
    private IDatabase db = null;
    private readonly IConfiguration _config;
    public string Message { get; set; } = null;

    public int VotesBarbecue { get; set; } = 0;
    public int VotesCajun { get; set; } = 0;
    public int VotesItalian { get; set; } = 0;
    public int VotesJapanese { get; set; } = 0;
    public int VotesMexican  { get; set; } = 0;
    public int VotesThai  { get; set; } = 0;



    public VotesModel(IConfiguration config, ILogger<IndexModel> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        try
        {
            var primaryEndpoint = _config["redis:primaryEndpoint"];
            var readerEndpoint = _config["redis:readerEndpoint"];
            Redis = ConnectionMultiplexer.Connect($"{primaryEndpoint},{readerEndpoint}");
            IDatabase db = Redis.GetDatabase();

            if (Request != null && Request.Query != null && Request.Query.ContainsKey("reset"))
            {
                await db.StringSetAsync("cuisine-Barbecue", 0);
                await db.StringSetAsync("cuisine-Cajun", 0);
                await db.StringSetAsync("cuisine-Italian", 0);
                await db.StringSetAsync("cuisine-Japanese", 0);
                await db.StringSetAsync("cuisine-Mexican", 0);
                await db.StringSetAsync("cuisine-Thai", 0);
                Message = "Votes have been reset";
            }

            VotesBarbecue = (int)(await db.StringGetAsync("cuisine-Barbecue"));
            VotesCajun = (int)(await db.StringGetAsync("cuisine-Cajun"));
            VotesItalian = (int)(await db.StringGetAsync("cuisine-Italian"));
            VotesJapanese = (int)(await db.StringGetAsync("cuisine-Japanese"));
            VotesMexican = (int)(await db.StringGetAsync("cuisine-Mexican"));
            VotesThai = (int)(await db.StringGetAsync("cuisine-Thai"));
        }
        catch (Exception ex)
        {
            Message = ex.ToString() + "<br/>" + ex.StackTrace;
        }
    }
}
