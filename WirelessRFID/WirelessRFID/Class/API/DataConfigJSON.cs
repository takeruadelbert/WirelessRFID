using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WirelessRFID.Class.API
{
    class DataConfigJSON
    {
        [JsonProperty("IPAddressServer")]
        public static string IPAddressServer { get; set; }
        [JsonProperty("API_GetTypeGate")]
        public static string APIGetTypeGate { get; set; }
        [JsonProperty("API_CheckMemberValidParkingOut")]
        public static string APICheckMemberValidParkingOut { get; set; }
        [JsonProperty("API_SaveDataIn")]
        public static string APISaveDataIn { get; set; }
        [JsonProperty("API_SaveDataOut")]
        public static string APISaveDataOut { get; set; }
        [JsonProperty("API_CheckMemberValidParkingIn")]
        public static string APICheckMemberValidParkingIn { get; set; }
        [JsonProperty("IPAddressUHFDevice")]
        public static string IPAddressUHFDevice { get; set; }
        [JsonProperty("IPCamera")]
        public static string IPCamera { get; set; }
        [JsonProperty("WebCamUsage")]
        public static bool WebCamUsage { get; set; }
    }
}
