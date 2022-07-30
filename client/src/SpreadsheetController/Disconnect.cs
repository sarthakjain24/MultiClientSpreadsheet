using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SS
{

    [JsonObject(MemberSerialization.OptIn)]
    public class Disconnect
    {
        [JsonProperty(PropertyName = "messageType")]
        private string messageType;
        
        [JsonProperty(PropertyName = "user")]
        private int id;

        public Disconnect(int userId)
        {
            messageType = "disconnected";
            id = userId;
        }

        public string GetMessageType()
        {
            return messageType;
        }

        public void SetMessageType(string msg)
        {
            messageType = msg;
        }

        public int GetUserID()
        {
            return id;
        }

        public void SetUserID(int uid)
        {
            id = uid;
        }
    }
}
