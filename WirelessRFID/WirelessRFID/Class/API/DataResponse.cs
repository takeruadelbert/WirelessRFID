using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WirelessRFID.Class.API
{
    class DataResponse
    {
        [JsonProperty("status")]
        public int Status
        {
            get;
            set;
        }

        [JsonProperty("message")]
        public string Message
        {
            get;
            set;
        }

        [JsonProperty("data")]
        public JArray Data
        {
            get;
            set;
        }

        public String toString()
        {
            return Status + " -- " + Message + " -- " + Data.ToString();
        }
    }
}
