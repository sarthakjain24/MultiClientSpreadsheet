using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SS
{
    [JsonObject(MemberSerialization.OptIn)]
    public class RequestCellEdit
    {
        [JsonProperty(PropertyName = "cellName")]
        private string cellName;

        [JsonProperty(PropertyName = "requestType")]
        private string requestType;
        
        [JsonProperty(PropertyName = "contents")]
        private string contents;


        public RequestCellEdit(string cellName, string contents)
        {
            requestType = "editCell";
            this.cellName = cellName;
            this.contents = contents;
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
