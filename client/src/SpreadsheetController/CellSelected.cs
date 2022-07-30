using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
namespace SS
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CellSelected
    {
        [JsonProperty(PropertyName = "messageType")]
        private string messageType;

        [JsonProperty(PropertyName = "cellName")]
        private string cellName;

        [JsonProperty(PropertyName = "selector")]
        private int selectorID;

        [JsonProperty(PropertyName = "selectorName")]
        private string selectorName;

        public CellSelected(string cellName, int selectorID, string selectorName)
        {
            this.messageType = "cellSelected";
            this.cellName = cellName;
            this.selectorID = selectorID;
            this.selectorName = selectorName;
        }
        public string GetMesssageType()
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

        public int GetCellRow()
        {
            return Int32.Parse(cellName.Substring(1));
        }

        public int getCellCol()
        {
            return (int)cellName.ToCharArray()[0] - 64;
        }

        public string GetSelectorName()
        {
            return selectorName;
        }

        public void SetSelectorName(string selector)
        {
            selectorName = selector;
        }

        public int GetSelectorID()
        {
            return selectorID;
        }

        public void SetSelectorID(int selector)
        {
            selectorID = selector;
        }

    }
}
