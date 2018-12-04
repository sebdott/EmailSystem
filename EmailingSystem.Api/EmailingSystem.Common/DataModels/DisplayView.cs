using EmailingSystem.Common.Enums;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmailingSystem.Common.DataModels
{
    public class DisplayView<T>
    {
        public DisplayView() {
            ListofDisplay = new List<T>();
            RecordTotalCount = 0;
        }
        public List<T> ListofDisplay { get; set; }
        public long RecordTotalCount { get; set; }
    }
}
