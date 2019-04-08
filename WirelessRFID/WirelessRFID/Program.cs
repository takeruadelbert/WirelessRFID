using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WirelessRFID.Class;
using WirelessRFID.Class.Helper;

namespace WirelessRFID
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
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
            catch (AccessViolationException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
