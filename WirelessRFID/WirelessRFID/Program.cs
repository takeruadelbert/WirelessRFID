using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WirelessRFID.Class;
using WirelessRFID.Class.Helper;
using WirelessRFID.Class.BarrierGate;

namespace WirelessRFID
{
    class Program
    {
        private static string[] gateType = { "masuk", "keluar" };

        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                // Parse Data Config JSON first
                TKHelper tk = new TKHelper();
                tk.ConvertConfigJSON();

                // Check and Get Type Gate
                Gate gate = new Gate();
                string type = gate.GetTypeGate();

                if (gateType.Contains(type.ToLower()))
                {
                    Gate.type = type;
                    DisplayGateType(type);
                    WirelessRFIDReader wirelessRFID = new WirelessRFIDReader();
                    wirelessRFID.Connect();
                    wirelessRFID.run();
                    while (!Console.KeyAvailable)
                    {
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        {
                            wirelessRFID.Disconnect();
                            break;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Unknown Type Gate : Press Any Key to Abort Program.");
                    Console.ReadKey();
                }
            }
            catch (AccessViolationException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void DisplayGateType(string type)
        {
            Console.WriteLine("===========");
            Console.WriteLine("GATE " + type.ToUpper());
            Console.WriteLine("===========");
        }
    }
}
