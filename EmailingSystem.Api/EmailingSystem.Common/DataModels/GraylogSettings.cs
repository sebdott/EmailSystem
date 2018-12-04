using ProtoBuf;

namespace EmailingSystem.Common.DataModels
{
    [ProtoContract]
    public class GraylogSettings
    {
        [ProtoMember(1)]
        public string Host { get; set; }
        [ProtoMember(2)]
        public int Port { get; set; }
    }

    
}
