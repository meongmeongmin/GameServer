﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class Connector
{
    Func<Session> _sessionFactory;

    public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory, int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            // 휴대폰 설정
            Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sessionFactory = sessionFactory;

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += OnConnectComplete;
            args.RemoteEndPoint = endPoint;
            args.UserToken = socket;

            RegisterConnect(args);
        }
    }

    void RegisterConnect(SocketAsyncEventArgs args)
    {
        Socket socket = args.UserToken as Socket;
        if (socket == null)
            return;

        bool pending = socket.ConnectAsync(args);
        if (pending == false)
            OnConnectComplete(null, args);
    }

    void OnConnectComplete(object sender, SocketAsyncEventArgs args)
    {
        if (args.SocketError == SocketError.Success)
        {
            Session session = _sessionFactory.Invoke();
            session.Start(args.ConnectSocket);
            session.OnConnected(args.RemoteEndPoint);
        }
        else
        {
            Console.WriteLine($"OnConnectComplete Fail: {args.SocketError}");
        }
    }
}