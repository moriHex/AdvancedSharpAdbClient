﻿// <copyright file="TcpSocket.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion, yungd1plomat, wherewhere">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion, yungd1plomat, wherewhere. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace AdvancedSharpAdbClient
{
    /// <summary>
    /// Implements the <see cref="ITcpSocket" /> interface using the standard <see cref="Socket"/> class.
    /// </summary>
    public partial class TcpSocket : ITcpSocket
    {
        private Socket socket;
        private EndPoint endPoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpSocket"/> class.
        /// </summary>
        public TcpSocket() => socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        /// <inheritdoc/>
        public bool Connected => socket.Connected;

        /// <inheritdoc/>
        public int ReceiveBufferSize
        {
            get => socket.ReceiveBufferSize;
            set => socket.ReceiveBufferSize = value;
        }

        /// <inheritdoc/>
        public void Connect(EndPoint endPoint)
        {
            if (endPoint is not (IPEndPoint or DnsEndPoint))
            {
                throw new NotSupportedException("Only TCP endpoints are supported");
            }

            socket.Connect(endPoint);
            socket.Blocking = true;
            this.endPoint = endPoint;
        }

        /// <inheritdoc/>
        public void Reconnect()
        {
            if (socket.Connected)
            {
                // Already connected - nothing to do.
                return;
            }

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Connect(endPoint);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            socket.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public int Send(byte[] buffer, int offset, int size, SocketFlags socketFlags) =>
            socket.Send(buffer, offset, size, socketFlags);

        /// <inheritdoc/>
        public int Receive(byte[] buffer, int size, SocketFlags socketFlags) =>
            socket.Receive(buffer, size, socketFlags);

        /// <inheritdoc/>
        public Stream GetStream() => new NetworkStream(socket);
    }
}
