using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EmployeeAPI
{
    public class Employee
    {
        [BsonId]
        public ObjectId MongoId { get; set; }  // MongoDB's default _id field

        [BsonElement("Id")]
        public string? Id { get; set; }  // This is the custom Id field in your document

        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Gender { get; set; }
        public string? Department { get; set; }
        public string? Skills { get; set; }
    }
}

