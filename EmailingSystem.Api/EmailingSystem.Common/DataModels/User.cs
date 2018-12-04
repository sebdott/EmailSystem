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
    public class User
    {
        public User() { }
        public User(User user)
        {
            Username = user.Username;
            Email = user.Email;
            Status = user.Status;
            ProcessingID = user.ProcessingID;
            LastLoginDatetime = user.LastLoginDatetime;
            SendEmailDatetime = user.SendEmailDatetime;
        }

        [ProtoMember(1)]
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string UserId { get; set; }
        [ProtoMember(2)]
        public string Username { get; set; }
        [ProtoMember(3)]
        public string Email { get; set; }
        [ProtoMember(4)]
        public string ProcessingID { get; set; }
        [ProtoMember(5)]
        public Status Status { get; set; }
        [ProtoMember(6)]
        [BsonDateTimeOptions]
        public DateTime LastLoginDatetime { get; set; }
        [ProtoMember(7)]
        [BsonDateTimeOptions]
        public DateTime LastModifiedDate { get; set; }
        [ProtoMember(8)]
        [BsonDateTimeOptions]
        public DateTime SendEmailDatetime { get; set; }
        [ProtoMember(9)]
        public bool IsSentEmailValidation { get; set; }
    }
}
