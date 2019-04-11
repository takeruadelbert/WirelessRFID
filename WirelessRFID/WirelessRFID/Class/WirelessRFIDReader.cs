using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WirelessRFID.Class.Helper;
using WirelessRFID.Class.BarrierGate;
using WirelessRFID.Class.API;
using AForge.Video;
using System.Drawing;
using System.Drawing.Imaging;
using WirelessRFID.Class.Miscellaneous.Webcam;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace WirelessRFID.Class
{
    class WirelessRFIDReader
    {
        [DllImport("ws2_32.dll")]
        public static extern Int32 WSAStartup(UInt16 wVersionRequested, ref WSAData wsaData);
        [DllImport("ws2_32.dll")]
        public static extern Int32 WSACleanup();

        static List<EPC_data> Tag_data = new List<EPC_data>();
        static bool bNewTag = false;
        static int nItemNo = 0;
        int RWBank = 0;
        int addStart = 0;
        int addEnd = 0;
        int counts = 1;
        bool bConnected = false;
        bool isReadOnce = true;
        public Dis.HANDLE_FUN f = new Dis.HANDLE_FUN(HandleData);
        static ResourceManager[] rmArray = new ResourceManager[2]{
                                                    new ResourceManager("DisDemo.SimpChinese", typeof(WirelessRFIDReader).Assembly),
                                                    new ResourceManager("DisDemo.English", typeof(WirelessRFIDReader).Assembly)};
        static ResourceManager rm = rmArray[0];
        static byte deviceNo = 0;
        public delegate void DeleConnectDev(byte[] ip, int CommPort, int PortOrBaudRate);
        public delegate void UpdateControlEventHandler();
        public static event UpdateControlEventHandler UpdateControl;
        private RESTAPI api = new RESTAPI();
        private string liveCameraURL = DataConfigJSON.IPCamera + "/snapshot";
        public static Image WebCamImage;
        private Webcam webcam;
        private const int width = 352, height = 240;
        private Gate barrierGate;

        public struct WSAData
        {
            public short wVersion;
            public short wHighVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
            public string szDescription;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
            public string szSystemStatus;
            [Obsolete] // Ignored for v2.0 and above 
            public int wMaxSockets;
            [Obsolete] // Ignored for v2.0 and above 
            public int wMAXUDPDG;
            public IntPtr dwVendorInfo;
        }
        private string port = "4196";

        public WirelessRFIDReader()
        {
            UpdateControl = new UpdateControlEventHandler(UpdateListView);
            barrierGate = new Gate();
            if (DataConfigJSON.WebCamUsage)
                webcam = new Webcam();
        }

        bool bWSAInit = false;
        public string AutoSearchDevice()
        {
            WSAData wsaData = new WSAData();
            if (!bWSAInit)
            {
                bWSAInit = true;
                WSAStartup(0x0202, ref wsaData);
            }
            ZLDM.m_DevCnt = ZLDM.StartSearchDev();
            if (ZLDM.m_DevCnt > 0)
            {
                return Marshal.PtrToStringAnsi(ZLDM.GetIP(0));
            }
            else
            {
                return null;
            }
        }

        public void ConnectDevice(byte[] ip, int CommPort, int PortOrBaudRate)
        {
            if (0 == Dis.DeviceInit(ip, CommPort, PortOrBaudRate))
            {
                string err = rm.GetString("strMsgInitFailure");
                Console.WriteLine(err);
            }

            if (0 == Dis.DeviceConnect())
            {
                Console.WriteLine("Error : Can't Connect Device.");
                return;
            }

            for (int i = 0; i < 3; ++i)
            {
                Dis.StopWork(deviceNo);
            }

            int mainVer = 0, minSer = 0;
            Dis.GetDeviceVersion(deviceNo, out mainVer, out minSer);
            string version = "Version: " + string.Format("{0}.{1}", mainVer, minSer);
            if (version != "Version:0.0")
            {
                bConnected = true;
            }
            else
            {
                string err = rm.GetString("strMsgConnectFailure");
                Console.WriteLine(err);
            }
        }

        public void Connect()
        {
            byte[] ip = new byte[32];
            int CommPort = 0;
            int PortOrBaudRate = 0;
            string ip_address = DataConfigJSON.IPAddressUHFDevice;
            if (!string.IsNullOrEmpty(ip_address))
            {
                if ((!Regex.IsMatch(ip_address, "^[0-9.]+$")) || ip_address.Length < 7 || ip_address.Length > 15)
                {
                    string err = rm.GetString("strMsgInvalidIPAdd");
                    return;
                }
                ip = Encoding.ASCII.GetBytes(ip_address);
                PortOrBaudRate = Int32.Parse(port);

                deviceNo = Byte.Parse("0");

                DeleConnectDev dcd = new DeleConnectDev(ConnectDevice);
                Console.WriteLine("IP Address     : " + ip_address);
                Console.WriteLine("Comm Port      : " + CommPort);
                Console.WriteLine("Port/Baud Rate : " + PortOrBaudRate);
                dcd.BeginInvoke(ip, CommPort, PortOrBaudRate, null, null);
                bConnected = false;
                Console.WriteLine("Successfully Connected to device.");
                Console.WriteLine("Starting ...");
                System.Threading.Thread.Sleep(3000);
            }
            else
            {
                Console.WriteLine("Error : Invalid IP Address.");
            }
        }

        public void Disconnect()
        {
            bConnected = false;
            Dis.ResetReader(deviceNo);
            Dis.DeviceDisconnect();
            Dis.DeviceUninit();
        }

        static string epc;
        public static void HandleData(IntPtr pData, int length)
        {
            epc = "";
            byte[] data = new byte[32];
            Marshal.Copy(pData, data, 0, length);
            for (int i = 1; i < length - 2; ++i)
            {
                epc += string.Format("{0:X2} ", data[i]);
            }

            bNewTag = true;
            for (int i = 0; i < Tag_data.Count; ++i)
            {
                if (epc == Tag_data[i].epc)
                {
                    Tag_data[i].count++;
                    Tag_data[i].epc = epc;
                    Tag_data[i].antNo = data[13];
                    Tag_data[i].devNo = data[0];
                    bNewTag = false;
                    nItemNo = i;
                    break;
                }
            }
            if (bNewTag)
            {
                EPC_data epcdata = new EPC_data();
                epcdata.epc = epc;
                epcdata.antNo = data[13];
                epcdata.devNo = data[0];
                epcdata.count = 1;
                Tag_data.Add(epcdata);
            }
            Console.WriteLine("\n");
            UpdateControl();
        }

        private void UpdateListView()
        {
            try
            {
                string epc = Tag_data[nItemNo].epc;
                TKHelper tk = new TKHelper();
                string UID = tk.ConvertEPCHexToNumber(epc);
                Console.WriteLine("UID : " + UID);

                string base64ImageIPCamera = "", base64ImageWebcam = "";

                // Take Snapshot of Camera
                Bitmap img = api.OpenIPCamera(liveCameraURL);
                if (img != null)
                {
                    base64ImageIPCamera = Base64Helper.ToBase64String(img, ImageFormat.Png);
                }
                if (Gate.type.ToLower() == "masuk")
                {
                    // check if UID is valid
                    JObject parameter = new JObject();
                    parameter["uid"] = UID;
                    var sent_parameter = JsonConvert.SerializeObject(parameter);
                    DataResponse checkMemberValidParkingIn = api.API_Post(DataConfigJSON.IPAddressServer, DataConfigJSON.APICheckMemberValidParkingIn, sent_parameter);
                    if (checkMemberValidParkingIn != null)
                    {
                        switch (checkMemberValidParkingIn.Status)
                        {
                            case 206: // valid
                                // Take Snapshot Webcam if it's enable.
                                if (DataConfigJSON.WebCamUsage)
                                {
                                    Bitmap bmpWebcam = new Bitmap(WebCamImage, width, height);
                                    if (bmpWebcam != null)
                                        base64ImageWebcam = bmpWebcam.ToBase64String(ImageFormat.Png);
                                }

                                // send data parking in to server
                                JObject param = new JObject();
                                param["uid"] = UID;
                                param["ip"] = DataConfigJSON.IPAddressUHFDevice;
                                param["face_image"] = base64ImageWebcam;
                                param["plate_image"] = base64ImageIPCamera;
                                var sent_param = JsonConvert.SerializeObject(param);
                                DataResponse dataResponseParkingIn = api.API_Post(DataConfigJSON.IPAddressServer, DataConfigJSON.APISaveDataIn, sent_param);
                                if (dataResponseParkingIn != null)
                                {
                                    switch (dataResponseParkingIn.Status)
                                    {
                                        case 206:
                                            // open barrier gate
                                            barrierGate.Open();
                                            break;
                                        default:
                                            Console.WriteLine(dataResponseParkingIn.Status + " -- " + dataResponseParkingIn.Message);
                                            break;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Error : Failed Send Data to Server.");
                                }
                                break;
                            case 505:
                                barrierGate.Open();
                                break;
                            default:
                                Console.WriteLine(checkMemberValidParkingIn.Message);
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error : Failed to Check Member Validity.");
                    }
                }
                else
                {
                    // check if UID is valid
                    JObject parameter_out = new JObject();
                    parameter_out["uid"] = UID;
                    var sent_parameter_out = JsonConvert.SerializeObject(parameter_out);
                    DataResponse checkMemberValidParkingOut = api.API_Post(DataConfigJSON.IPAddressServer, DataConfigJSON.APICheckMemberValidParkingOut, sent_parameter_out);
                    if (checkMemberValidParkingOut != null)
                    {
                        switch (checkMemberValidParkingOut.Status)
                        {
                            case 206:
                                int response = Convert.ToInt32(checkMemberValidParkingOut.Data[0].ToString());
                                if (response == 0)
                                {
                                    JObject param_out = new JObject();
                                    param_out["uid"] = UID;
                                    param_out["ip"] = DataConfigJSON.IPAddressUHFDevice;
                                    param_out["image"] = base64ImageIPCamera;
                                    var sent_param_out = JsonConvert.SerializeObject(param_out);
                                    DataResponse dataResponseParkingOut = api.API_Post(DataConfigJSON.IPAddressServer, DataConfigJSON.APISaveDataOut, sent_param_out);
                                    if (dataResponseParkingOut != null)
                                    {
                                        switch (dataResponseParkingOut.Status)
                                        {
                                            case 206:
                                                barrierGate.Open();
                                                break;
                                            default:
                                                Console.WriteLine(dataResponseParkingOut.Message);
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Error : Failed to Send Data to Server.");
                                    }
                                }
                                else
                                {
                                    barrierGate.Open();
                                }
                                break;
                            default:
                                Console.WriteLine(checkMemberValidParkingOut.Status + " -- " + checkMemberValidParkingOut.Message);
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error : Failed to Check Member Validity.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void run()
        {
            Console.WriteLine("running");
            Dis.BeginMultiInv(deviceNo, f);
        }
    }
}
