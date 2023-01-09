using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreIdentityWithMongoDB.Models
{
    public class ShortenedUrl
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string OriginalUrl { get; set; }
        public string ShortCode { get; set; }
        public string ShortUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
    }
}
