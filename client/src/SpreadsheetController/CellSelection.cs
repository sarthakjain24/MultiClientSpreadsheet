using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SS
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CellSelection
    {
        [JsonProperty(PropertyName = "cellName")]
        private string cellName;
        
        [JsonProperty(PropertyName = "requestType")]
        private string requestType;

       public CellSelection(string cellName)
       {
            requestType = "selectCell";
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

        public void SetCellName(string cellName)
        {
            this.cellName = cellName;
        }
    }
}
