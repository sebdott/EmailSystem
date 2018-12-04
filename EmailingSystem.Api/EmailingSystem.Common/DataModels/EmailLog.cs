using EmailingSystem.Common.Enums;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmailingSystem.Common.DataModels
{
    [ProtoContract]
    public class EmailLog
    {
        public EmailLog() {}
        [ProtoMember(1)]
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string UserId { get; set; }
        [ProtoMember(2)]
        public string Username { get; set; }
        [ProtoMember(3)]
        public string Email { get; set; }
        [BsonDateTimeOptions]
        [ProtoMember(4)]
        public DateTime SendEmailDatetime { get; set; }
        [ProtoMember(5)]
        public bool IsSentEmailValidation { get; set; }
    }
}
