using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SS
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Undo
    {
        [JsonProperty(PropertyName = "requestType")]
        private string requestType;

        public Undo()
        {
            requestType = "undo";
        }

        public string GetRequestType()
        {
            return requestType;
        }

        public void SetRequestType(string rqst)
        {
            requestType = rqst;
        }
    }
}
