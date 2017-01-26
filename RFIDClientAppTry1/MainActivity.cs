using Android.App;
using Android.Widget;
using Android.OS;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using static Android.Bluetooth.BluetoothClass;
using System.Threading;
using Java.Util.Concurrent;

namespace RFIDClientAppTry1
{
    [Activity(Label = "Client App", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private Socket clientSocket;
        private byte[] buffer;
        private TextView msg;
        private string textToWrite = "NothingYet";
        private ManualResetEvent doneUpdatingUI = new ManualResetEvent(false);
        private CountDownLatch latch;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);      

            Button connect = FindViewById<Button>(Resource.Id.button1);
            msg = FindViewById<TextView>(Resource.Id.textView1);

            connect.Click += delegate
            {
                try
                {
                    clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    clientSocket.BeginConnect(new IPEndPoint(IPAddress.Parse("208.44.252.155"), 3335), new AsyncCallback(ConnectCallback), null);
                    msg.Text = "Button clicked.";
                }
                catch (Exception ex)
                {
                    msg.Text = ex.ToString();
                }

                if (clientSocket.Connected)
                    msg.Text = "Client Connected Successfully!";
            };

            Button lot = FindViewById<Button>(Resource.Id.button2);

            lot.Click += delegate
            {
                try
                {
                    latch = new CountDownLatch(1);
                    buffer = Encoding.ASCII.GetBytes("<LOT>");
                    clientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
                    
                    RunOnUiThread(() => UpdateUI());
                    latch.Await();
                    msg.Text = textToWrite;

                }
                catch (Exception ex)
                {
                    msg.Text = ex.ToString();
                }
            };
        }

        private void UpdateUI()
        {
            
            buffer = new byte[clientSocket.ReceiveBufferSize];
            clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
              
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndConnect(ar);
                //UpdateControlStates(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndSend(ar);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int received = clientSocket.EndReceive(ar);

                Array.Resize(ref buffer, received);
                string text = Encoding.ASCII.GetString(buffer);
                //Console.WriteLine(text);

                TextView msg = FindViewById<TextView>(Resource.Id.textView2);
                textToWrite = text;
                //Array.Resize(ref buffer, clientSocket.ReceiveBufferSize);
                //clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            latch.CountDown();
        }
    }
}

