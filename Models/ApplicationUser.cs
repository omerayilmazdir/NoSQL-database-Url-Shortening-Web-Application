using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Driver;
using MongoDbGenericRepository.Attributes;
using System;

namespace CoreIdentityWithMongoDB.Models
{
    [CollectionName("Users")]
    public class ApplicationUser:MongoIdentityUser<Guid>
    {

    }
}
