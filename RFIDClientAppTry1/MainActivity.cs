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
        private Client client;

        private TextView msg;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);
            client = new Client();    

            Button connect = FindViewById<Button>(Resource.Id.button1);
            msg = FindViewById<TextView>(Resource.Id.textView1);

            connect.Click += delegate
            {
                try
                {
                    client.Connect();
                    msg.Text = "Button clicked.";
                }
                catch (Exception ex)
                {
                    msg.Text = ex.ToString();
                }

                if (client.clientSocket.Connected)
                    msg.Text = "Client Connected Successfully!";
            };

            Button lot = FindViewById<Button>(Resource.Id.button2);

            lot.Click += delegate
            {
                try
                {
                    client.LotClicked();
                    msg.Text = client.LotReceive();
                }
                catch (Exception ex)
                {
                    msg.Text = ex.ToString();
                }
            };
        }
    }
}

