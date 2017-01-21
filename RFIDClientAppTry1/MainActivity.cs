using Android.App;
using Android.Widget;
using Android.OS;
using System.Net.Sockets;
using System.Net;
using System;

namespace RFIDClientAppTry1
{
    [Activity(Label = "Client App", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private Socket clientSocket;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);      

            Button connect = FindViewById<Button>(Resource.Id.button1);
            TextView msg = FindViewById<TextView>(Resource.Id.textView1);

            connect.Click += delegate
            {
                try
                {
                    clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    clientSocket.BeginConnect(new IPEndPoint(IPAddress.Loopback, 3333), new AsyncCallback(ConnectCallback), null);
                    msg.Text = "Button clicked.";
                }
                catch (Exception ex)
                {
                    msg.Text = ex.ToString();
                }

                if (clientSocket.Connected)
                    msg.Text = "Client Connected Successfully!";
            };
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
    }
}

