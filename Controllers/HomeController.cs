using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using CoreIdentityWithMongoDB.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Authentication;
using MongoDB.Driver.Linq;
using shortid;



namespace CoreIdentityWithMongoDB.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly IMongoDatabase _mongoDatabase;
        private const string ServiceUrl = "http://localhost:5000/Home/ShortUrl";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;

            var connectionString = "mongodb://localhost:27017/";
            var mongoClient = new MongoClient(connectionString);
            _mongoDatabase = mongoClient.GetDatabase("IdentityAuthDb");

        }

        public IActionResult Index()
        {
            var shortenedUrlCollection = _mongoDatabase.GetCollection<ShortenedUrl>("shortened-urls");
            var user = _mongoDatabase.GetCollection<ShortenedUrl>("Users");
            // kısaltılan link sayısını verir
            var shortenedUrl = shortenedUrlCollection
                .AsQueryable().Count();
            // kullanıcı sayısını verir
            var users = user.AsQueryable().Count();

            ViewData["CountLink"] = shortenedUrl;
            ViewData["CountUser"] = users;

            var pipeline = new[] {
                new BsonDocument("$group", new BsonDocument {
                    { "_id", "$OriginalUrl" },
                    { "createdBy", new BsonDocument("$first", "$CreatedBy") },
                    { "count", new BsonDocument("$sum", 1) }
                }),
                new BsonDocument("$sort", new BsonDocument("count", -1)),
                new BsonDocument("$limit", 1),
                new BsonDocument("$project", new BsonDocument {
                    { "OriginalUrl", "$_id" },
                    { "createdBy", 1 },
                    { "count", 1 }
                })
            };



            var result = shortenedUrlCollection.Aggregate<BsonDocument>(pipeline).FirstOrDefault();

            if (result != null)
            {
                var model = new ShortenedUrl
                {
                    OriginalUrl = result["OriginalUrl"].AsString,
                    CreatedBy = result["createdBy"].AsString,
                    ShortCode = result["count"].AsInt32.ToString()
                };
                return View(model);
            }
            else
            {
                return View();
            }


        }

        public IActionResult ShortUrl()
        {


            return View();
        }


        [HttpGet]
        public async Task<IActionResult> ShortUrl(string u)
        {
            // kısaltılmış url collection
            var shortenedUrlCollection = _mongoDatabase.GetCollection<ShortenedUrl>("shortened-urls");
            // kısa kod kontrolü
            var shortenedUrl = await shortenedUrlCollection
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.ShortCode == u);

            // kısa kod yok ise ana sayfaya dön
            if (shortenedUrl == null)
            {
                return View();
            }

            return Redirect(shortenedUrl.OriginalUrl);
        }

        [HttpPost]
        public async Task<IActionResult> ShortenUrl(string longUrl)
        {

            var shortenedUrlCollection = _mongoDatabase.GetCollection<ShortenedUrl>("shortened-urls");
            // link kontrolü ve atama işlemi
            var shortenedUrl = await shortenedUrlCollection
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.OriginalUrl == longUrl);



            var shortCode = ShortId.Generate();
            shortenedUrl = new ShortenedUrl
            {

                CreatedAt = DateTime.UtcNow,
                OriginalUrl = longUrl,
                ShortCode = shortCode,
                ShortUrl = $"{ServiceUrl}?u={shortCode}",
                CreatedBy = User.Identity.Name

            };
            // veritabanına ekleme işlemi
            await shortenedUrlCollection.InsertOneAsync(shortenedUrl);

            return View(shortenedUrl);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}