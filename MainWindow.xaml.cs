using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.IO.Pipes;
using System.Diagnostics;
using System.Threading;
using System.Timers;

namespace BDMJClient_Single
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NamedPipeServerStream pipeServer = null;
        public MainWindow()
        {
            InitializeComponent();
            Dong.Zuowei.Text = "东";
            Xi.Zuowei.Text = "西";
            Nan.Zuowei.Text = "南";
            Bei.Zuowei.Text = "北";

            Dong.strZuowei = "东";
            Xi.strZuowei = "西";
            Nan.strZuowei = "南";
            Bei.strZuowei = "北";

            ConnectToServer();
            Thread.Sleep(250);


            streamClient.WriteString("Request");
            string ss = streamClient.ReadString();

            Dong.InitialUI(ss);
            Xi.InitialUI(ss);
            Nan.InitialUI(ss);
            Bei.InitialUI(ss);

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 100;
            timer.Elapsed += GameStartLoop;
            timer.AutoReset = false;
            timer.Enabled = true;
            //timer.Start();
        }

        private void GameStartLoop(Object source, System.Timers.ElapsedEventArgs e)
        {
            if ((string.IsNullOrEmpty(Global.Command[ZUOWEI.Dong]) == false) && (Global.isFinished[ZUOWEI.Dong] == false))
            {
                streamClient.WriteString(Global.Command[ZUOWEI.Dong]);
                Global.Command[ZUOWEI.Dong] = string.Empty;
                Global.isFinished[ZUOWEI.Dong] = true;
            }
            else if((string.IsNullOrEmpty(Global.Command[ZUOWEI.Nan]) == false) && (Global.isFinished[ZUOWEI.Nan] == false))
            {
                streamClient.WriteString(Global.Command[ZUOWEI.Nan]);
                Global.Command[ZUOWEI.Nan] = string.Empty;
                Global.isFinished[ZUOWEI.Nan] = true;
            }
            else if ((string.IsNullOrEmpty(Global.Command[ZUOWEI.Xi]) == false) && (Global.isFinished[ZUOWEI.Xi] == false))
            {
                streamClient.WriteString(Global.Command[ZUOWEI.Xi]);
                Global.Command[ZUOWEI.Xi] = string.Empty;
                Global.isFinished[ZUOWEI.Xi] = true;
            }
            else if ((string.IsNullOrEmpty(Global.Command[ZUOWEI.Bei]) == false) && (Global.isFinished[ZUOWEI.Bei] == false))
            {
                streamClient.WriteString(Global.Command[ZUOWEI.Bei]);
                Global.Command[ZUOWEI.Bei] = string.Empty;
                Global.isFinished[ZUOWEI.Bei] = true;
            }

            if (Global.isFinished[ZUOWEI.Dong] && Global.isFinished[ZUOWEI.Nan] && Global.isFinished[ZUOWEI.Xi] && Global.isFinished[ZUOWEI.Bei])
            {
                streamClient.WriteString("Request");
                string ss = streamClient.ReadString();
                Global.isFinished[ZUOWEI.Dong] = false;
                Global.isFinished[ZUOWEI.Nan] = false;
                Global.isFinished[ZUOWEI.Xi] = false;
                Global.isFinished[ZUOWEI.Bei] = false;
                Dong.UpdateUI(ss);
                Xi.UpdateUI(ss);
                Nan.UpdateUI(ss);
                Bei.UpdateUI(ss);
            }
            System.Timers.Timer timer = source as System.Timers.Timer;
            timer.Enabled = true;
        }

        //void GameStartLoop()
        //{
        //    streamClient.WriteString("Request");
        //    string ss = streamClient.ReadString();

        //    Dong.InitialUI(ss);
        //    Xi.InitialUI(ss);
        //    Nan.InitialUI(ss);
        //    Bei.InitialUI(ss);
        //    do
        //    {
        //        if (string.IsNullOrEmpty(Nan.command) == false)
        //        {

        //        }
        //    } while (true);
        //}

        NamedPipeClientStream pipeClient = null;
        StreamString streamClient = null;
        private void ConnectToServer()
        {
            pipeClient = new NamedPipeClientStream(".", "testpipe", PipeDirection.InOut);
            pipeClient.Connect();

            streamClient = new StreamString(pipeClient);
            if(streamClient.ReadString() == "BDMJServer")
            {
                streamClient.WriteString("BDMJClient");
            }
        }

        private static void ServerThread(object data)
        {
            NamedPipeServerStream pipeServer = new NamedPipeServerStream("testpipe", PipeDirection.InOut);

            //string ProgramPath = Environment.CurrentDirectory + "\\Program\\BDMJprogram.exe";
            //Process.Start(ProgramPath);

            // Wait for a client to connect
            pipeServer.WaitForConnection();

            try
            {
                // Read the request from the client. Once the client has
                // written to the pipe its security token will be available.

                StreamString ss = new StreamString(pipeServer);

                ss.WriteString("Connecting");
                // Verify our identity to the connected client using a
                // string that the client anticipates.

                string msg = ss.ReadString();
                msg = ss.ReadString();
            }
            // Catch the IOException that is raised if the pipe is broken
            // or disconnected.
            catch (IOException e)
            {
                Console.WriteLine("ERROR: {0}", e.Message);
            }
            while (true) ;
            //pipeServer.Close();
        }
    }

    // Defines the data protocol for reading and writing strings on our stream
    public class StreamString
    {
        private Stream ioStream;
        private UnicodeEncoding streamEncoding;

        public StreamString(Stream ioStream)
        {
            this.ioStream = ioStream;
            streamEncoding = new UnicodeEncoding();
        }

        public string ReadString()
        {
            int len = 0;

            len = ioStream.ReadByte() * 256;
            len += ioStream.ReadByte();
            byte[] inBuffer = new byte[len];
            ioStream.Read(inBuffer, 0, len);

            return streamEncoding.GetString(inBuffer);
        }

        public int WriteString(string outString)
        {
            byte[] outBuffer = streamEncoding.GetBytes(outString);
            int len = outBuffer.Length;
            if (len > UInt16.MaxValue)
            {
                len = (int)UInt16.MaxValue;
            }
            ioStream.WriteByte((byte)(len / 256));
            ioStream.WriteByte((byte)(len & 255));
            ioStream.Write(outBuffer, 0, len);
            ioStream.Flush();

            return outBuffer.Length + 2;
        }
    }

    // Contains the method executed in the context of the impersonated user
    public class ReadFileToStream
    {
        private string fn;
        private StreamString ss;

        public ReadFileToStream(StreamString str, string filename)
        {
            fn = filename;
            ss = str;
        }

        public void Start()
        {
            string contents = File.ReadAllText(fn);
            ss.WriteString(contents);
        }
    }
}
