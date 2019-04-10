using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WirelessRFID.Class.API;
using WirelessRFID.Class;

namespace WirelessRFID.Class.BarrierGate
{
    class Gate
    {
        private const string ipv4_address_gate = "http://192.168.1.76:23567";
        private static RESTAPI api;
        public static string type = "";

        public Gate()
        {
            api = new RESTAPI();
        }

        public void Open()
        {
            JObject param = new JObject();
            param["code"] = 700;

            var sent_param = JsonConvert.SerializeObject(param);
            
            DataResponseBarrierGate response = api.API_Post_BarrierGate(ipv4_address_gate, "", sent_param);
            if (response != null)
            {
                Console.WriteLine(response.Code + " : " + response.Message);
            }
            else
            {
                Console.WriteLine("Error : Can't Connect to the gate.");
            }
        }

        public string GetTypeGate()
        {
            string ipv4_device = DataConfigJSON.IPAddressUHFDevice;

            JObject param = new JObject();
            param["ip"] = ipv4_device;

            var sent_param = JsonConvert.SerializeObject(param);
            DataResponse response = api.API_Post(DataConfigJSON.IPAddressServer, DataConfigJSON.APIGetTypeGate, sent_param);
            if(response != null)
            {
                switch(response.Status)
                {
                    case 206:
                        return response.Data[0].ToString();
                    default:
                        return response.Message;
                }
            } else
            {
                string err = "Error : Invalid Data Response From Server.";
                Console.WriteLine(err);
                return err;
            }
        }
    }
}
