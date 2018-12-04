using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmailingSystem.Common.DataModels
{
    [ProtoContract]
    public class KafkaSettings
    {
        [ProtoMember(1)]
        public string BrokerList { get; set; }
        public string ConsumerGroupId { get; set; }
        [ProtoMember(3)]
        public string ProducerGroupId { get; set; }
        [ProtoMember(4)]
        public int Partition { get; set; }
    }

    
}
