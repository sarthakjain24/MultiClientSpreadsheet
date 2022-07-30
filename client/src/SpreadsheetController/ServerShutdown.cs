using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SS
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ServerShutdown
    {
        [JsonProperty(PropertyName = "messageType")]
        private string messageType;
        [JsonProperty(PropertyName = "message")]
        private string message;
        public ServerShutdown(string message)
        {
            messageType = "serverError";
            this.message = message;
        }

        public string GetMessageType()
        {
            return messageType;
        }

        public void SetMessageType(string msg)
        {
            messageType = msg;
        }

        public string GetMessage()
        {
            return message;
        }

        public void SetMessage(string msg)
        {
            message = msg;
        }
    }
}
