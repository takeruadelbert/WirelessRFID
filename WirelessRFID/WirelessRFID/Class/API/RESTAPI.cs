using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WirelessRFID.Class.API
{
    class RESTAPI
    {
        private const int timeout_conn = 5000; // 3 seconds
        private int width = 352, height = 240; // (352 x 240 pixel)

        public RESTAPI()
        {

        }

        public DataResponse API_Post(string ip_address_server, string APIUrl, string sent_param = "")
        {
            string result = "";
            try
            {
                string full_API_URL = ip_address_server + APIUrl;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(full_API_URL);
                request.Method = "POST";
                request.Timeout = timeout_conn;

                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                Byte[] byteArray = encoding.GetBytes(sent_param);

                request.ContentLength = byteArray.Length;
                request.ContentType = @"application/json";
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }
                long length = 0;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    length = response.ContentLength;
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                    result = readStream.ReadToEnd();
                    var json = JsonConvert.DeserializeObject<DataResponse>(result);
                    return json;
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public DataResponseBarrierGate API_Post_BarrierGate(string ip_address_server, string APIUrl, string sent_param = "")
        {
            string result = "";
            try
            {
                string full_API_URL = ip_address_server + APIUrl;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(full_API_URL);
                request.Method = "POST";
                request.Timeout = timeout_conn;

                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                Byte[] byteArray = encoding.GetBytes(sent_param);

                request.ContentLength = byteArray.Length;
                request.ContentType = @"application/json";
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }
                long length = 0;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    length = response.ContentLength;
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                    result = readStream.ReadToEnd();
                    var json = JsonConvert.DeserializeObject<DataResponseBarrierGate>(result);
                    return json;
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public DataResponse API_Get(string ip_address_server, string API_URL)
        {
            string result = "";
            try
            {
                string full_URL_API = ip_address_server + API_URL;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(full_URL_API);
                request.Method = "GET";
                request.Timeout = timeout_conn;
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    result = reader.ReadToEnd();
                    var json = JsonConvert.DeserializeObject<DataResponse>(result);
                    return json;
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public Bitmap OpenIPCamera(string URL)
        {
            try
            {
                WebRequest req = WebRequest.Create(URL);
                WebResponse response = req.GetResponse();
                Stream stream = response.GetResponseStream();
                byte[] imageData = ReadFully(stream);
                MemoryStream mstream = new MemoryStream(imageData);
                var img = Image.FromStream(mstream);
                Bitmap bmp = new Bitmap(img, width, height);
                stream.Close();
                return bmp;
            }
            catch (WebException ex)
            {
                Console.WriteLine("Error : Failed to Open URL. " + ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error When Fetch Data Snapshot IP Camerta : " + ex.Message);
                return null;
            }
        }

        public byte[] ReadFully(Stream input)
        {
            try
            {
                byte[] buffer = new byte[16 * 1024];
                using (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Convert Stream To Byte[] : " + ex.Message);
                return null;
            }
        }
    }
}
