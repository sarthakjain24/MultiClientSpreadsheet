using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
namespace SS
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CellUpdate
    {
        [JsonProperty(PropertyName = "messageType")]
        public string messageType;

        [JsonProperty(PropertyName = "cellName")]
        public string cellName;

        [JsonProperty(PropertyName = "contents")]
        public string contents;


        public CellUpdate(string cellName, string contents)
        {
            this.messageType = "cellUpdated";
            this.cellName = cellName;
            this.contents = contents;
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

        public void SetCellName(string cellName)
        {
            this.cellName = cellName;
        }

        public string GetContents()
        {
            return contents;
        }

        public void SetContents(string content)
        {
            contents = content;
        }

    }
}
