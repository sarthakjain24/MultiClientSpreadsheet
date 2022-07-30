using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SS
{
    [JsonObject(MemberSerialization.OptIn)]
    public class InvalidRequest
    {
        [JsonProperty(PropertyName = "messageType")]
        private string messageType;

        [JsonProperty(PropertyName = "cellName")]
        private string cellName;


        [JsonProperty(PropertyName = "message")]
        private string message;

        public InvalidRequest(string cellName, string message)
        {
            messageType = "requestError";
            this.cellName = cellName;
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


        public string GetCellName()
        {
            return cellName;
        }

        public void SetCellName(string cell)
        {
            cellName = cell;
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
