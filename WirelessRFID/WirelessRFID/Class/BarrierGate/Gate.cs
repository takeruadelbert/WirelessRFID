using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WirelessRFID.Class.API;

namespace WirelessRFID.Class.BarrierGate
{
    class Gate
    {
        private const string ipv4_address_gate = "http://192.168.1.76:23567";
        private RESTAPI api;
        public Gate()
        {
            api = new RESTAPI();
        }

        public void Open()
        {
            JObject param = new JObject();
            param["code"] = 700;

            var sent_param = JsonConvert.SerializeObject(param);
            
            DataResponse response = api.API_Post(ipv4_address_gate, "", sent_param);
            if (response != null)
            {
                Console.WriteLine(response.Code + " : " + response.Message);
            }
            else
            {
                Console.WriteLine("Error : Can't Connect to the gate.");
            }
        }
    }
}
