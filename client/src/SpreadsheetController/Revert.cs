using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
namespace SS
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Revert
    {
        [JsonProperty(PropertyName = "requestType")]
        private string requestType;

        [JsonProperty(PropertyName = "cellName")]
        private string cellName;

        public Revert(string cellName)
        {
            requestType = "revertCell";
            this.cellName = cellName;
        }

        public string GetRequestType()
        {
            return requestType;
        }

        public void SetRequestType(string rqst)
        {
            requestType = rqst;
        }

        public string GetCellName()
        {
            return cellName;
        }

        public void SetCellName(string cell)
        {
            cellName = cell;
        }
    }
}
