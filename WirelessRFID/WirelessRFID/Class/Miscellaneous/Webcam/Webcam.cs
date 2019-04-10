using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WirelessRFID.Class.Miscellaneous.Webcam
{
    class Webcam
    {
        VideoCaptureDevice frame;
        FilterInfoCollection Devices;

        public Webcam()
        {
            StartWebcam();
        }

        public void StartWebcam()
        {
            try
            {
                Devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                frame = new VideoCaptureDevice(Devices[0].MonikerString);
                frame.NewFrame += new NewFrameEventHandler(NewFrame_event);
                frame.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error : No Webcam Device found. " + ex.Message);
            }
        }

        public void StopWebcam()
        {
            frame.Stop();
        }

        void NewFrame_event(object send, NewFrameEventArgs e)
        {
            try
            {
                WirelessRFIDReader.WebCamImage = (Image)e.Frame.Clone();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error WebCam : " + ex.Message);
            }
        }
    }
}
