using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Net.Sockets;
using System.Net;
using System.ComponentModel;
using Java.Util.Concurrent;

namespace RFIDClientAppTry1
{
    class Client
    {
        public Socket clientSocket;

        private string text;
        private CountDownLatch latch = new CountDownLatch(0);

        public Client()
        {

        }

        private byte[] buffer;

        public void Connect()
        {
            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.BeginConnect(new IPEndPoint(IPAddress.Parse("208.44.252.155"), 3335), new AsyncCallback(ConnectCallback), null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndConnect(ar);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
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
                text = Encoding.ASCII.GetString(buffer);
                Array.Resize(ref buffer, clientSocket.ReceiveBufferSize);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            latch.CountDown();
        }

        public void LotClicked()
        {
            try
            {
                latch = new CountDownLatch(1);
                buffer = Encoding.ASCII.GetBytes("<LOT>");
                clientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public string LotReceive()
        {
            try
            {
                buffer = new byte[clientSocket.ReceiveBufferSize];
                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
                latch.Await();
                return text;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "ERROR";
            }
        }
    }
}