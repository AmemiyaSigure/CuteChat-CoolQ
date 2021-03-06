﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace cx.rain.cq.cutechat.Code
{
    class SocketWorker
    {
        public static void OnReceive(IAsyncResult ar)
        {
            try
            {
                var listenSocket = ar.AsyncState as Socket;
                var client = listenSocket.EndAccept(ar);
                listenSocket.BeginAccept(OnReceive, listenSocket);

                CuteChat.Sockets.Add(client);

                while(true)
                {
                    byte[] recv_buffer = new byte[1024 * 1024 * 10];
                    string message_data = "";
                    int real_recv = 0;

                    real_recv = client.Receive(recv_buffer);
                    message_data = Encoding.UTF8.GetString(recv_buffer, 0, real_recv);

                    if (message_data != "")
                    {
                        CuteChat.Api.SendGroupMessage(CuteChat.GroupId, message_data);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public static void Send(string str)
        {
            foreach (var socket in CuteChat.Sockets)
            {
                try
                {
                    using (var stream = new NetworkStream(socket))
                    {
                        using (var writer = new StreamWriter(stream))
                        {
                            writer.WriteLine(str);
                            writer.Flush();
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
