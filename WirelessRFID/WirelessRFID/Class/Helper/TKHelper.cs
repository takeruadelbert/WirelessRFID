using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WirelessRFID.Class.API;

namespace WirelessRFID.Class.Helper
{
    class TKHelper
    {
        public string ConvertEPCHexToNumber(string EPC_Hex)
        {
            if (EPC_Hex != "")
            {
                EPC_Hex = EPC_Hex.Replace(" ", "");
                string hex_part1 = EPC_Hex.Substring(0, 8);
                string hex_part2 = EPC_Hex.Substring(8, 8);
                string hex_part3 = EPC_Hex.Substring(16, 8);
                UInt32 part1 = UInt32.Parse(hex_part1, System.Globalization.NumberStyles.HexNumber);
                UInt32 part2 = UInt32.Parse(hex_part2, System.Globalization.NumberStyles.HexNumber);
                UInt32 part3 = UInt32.Parse(hex_part3, System.Globalization.NumberStyles.HexNumber);
                string result = part1 + "" + part2 + "" + part3;
                return result;
            }
            return "";
        }

        public DataConfigJSON ConvertConfigJSON()
        {
            string dataConfigJSONPath = GetApplicationExecutableDirectoryName() + @"\config.json";
            using (StreamReader r = new StreamReader(dataConfigJSONPath))
            {
                string json = r.ReadToEnd();
                DataConfigJSON data = JsonConvert.DeserializeObject<DataConfigJSON>(json);
                return data;
            }
        }

        public string GetApplicationExecutableDirectoryName()
        {
            string workingDirectory = Environment.CurrentDirectory;
            return Directory.GetParent(workingDirectory).Parent.FullName;
        }
    }
}
