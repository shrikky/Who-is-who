﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UDP
{
	public class SocketConnection : MonoBehaviour
	{
		private Socket _udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		private Socket _tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		private const int bufSize = 12;
		private State state = new State();
		private EndPoint epFrom = new IPEndPoint(IPAddress.Any, 0);
		private AsyncCallback recv = null;
		//private KeyValuePair<int, Vector3> _inputStreamData;
		public Queue<KeyValuePair<int, Vector3>> _inputStreamData;
		public UIPositionManager dataManager;
		public class State
		{
			public byte[] buffer = new byte[bufSize];
		}

		public void InitConnectionToRaspberryPI(string address, int port)
		{
			_tcpSocket.Connect(IPAddress.Parse(address), port);  //3300 TCP connect
		}

		public void ListenForData(string address, int port)
		{
			_udpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
			_udpSocket.Bind(new IPEndPoint(IPAddress.Any, port));
			Receive();
		}

		//public void InitConnectionToServer(string address, int port)
		//{
		//	_udpSocket.Connect(IPAddress.Parse(address), port);
		//	Receive();
		//}

		public void SendViaTCP(string text)
		{
			byte[] data = Encoding.ASCII.GetBytes(text);
			_tcpSocket.BeginSend(data, 0, data.Length, SocketFlags.None, (ar) =>
			{
				State so = (State)ar.AsyncState;
				int bytes = _tcpSocket.EndSend(ar);
				Debug.LogFormat("SEND: {0}, {1}", bytes, text);
			}, state);
		}

		public void SendBytes(byte[] data)
		{
			_udpSocket.BeginSend(data, 0, data.Length, SocketFlags.None, (ar) =>
			{
				State so = (State)ar.AsyncState;
				int bytes = _udpSocket.EndSend(ar);
				Debug.LogFormat("SEND: {0}, {1}", bytes, data);
			}, state);
		}

		private void Receive()
		{
			
			_udpSocket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv = (ar) =>
			{
				State so = (State)ar.AsyncState;			
				int bytes = _udpSocket.EndReceiveFrom(ar, ref epFrom);
				_udpSocket.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv, so);
				Debug.LogFormat("RECV: {0}: {1}, {2}", epFrom.ToString(), bytes, Encoding.ASCII.GetString(so.buffer, 0, bytes));
				var displayData = Encoding.ASCII.GetString(so.buffer, 0, bytes);
				//var data = DeSerialize(so.buffer);
				//dataManager.SetUIData(data);
			}, state);
			
	
		}

		public void Start()
		{
			InitConnectionToRaspberryPI("172.17.4.58", 3300); // Initialize connection by sending a message to 3300 port
			SendViaTCP("S 3301");
			ListenForData("127.0.0.1", 3301);  // Start listening on 3301
		}

		public static byte[] SerializeVec(Vector3[] vect)
		{
			byte[] buff = new byte[sizeof(float) * 3 * vect.Length];  //3 * 4 * 3 = 36 bbytes

			for (int i = 0; i < vect.Length; i++)
			{
				Buffer.BlockCopy(BitConverter.GetBytes(vect[i].x), 0, buff, (i * 3 + 0) * sizeof(float), sizeof(float)); // 0 * 4  3 * 4  6 * 4
				Buffer.BlockCopy(BitConverter.GetBytes(vect[i].y), 0, buff, (i * 3 + 1) * sizeof(float), sizeof(float)); // 1 * 4  4 * 4  7 * 4
				Buffer.BlockCopy(BitConverter.GetBytes(vect[i].z), 0, buff, (i * 3 + 2) * sizeof(float), sizeof(float)); // 2 * 4  5 * 4  8 * 4
			}

			return buff;
		}
		public void Update()
		{
			if(Input.GetKeyDown(KeyCode.A))
			{
				_tcpSocket.Shutdown(SocketShutdown.Both);
				_tcpSocket.Close();
				_udpSocket.Shutdown(SocketShutdown.Both);
				_udpSocket.Close();
			}
		}
		public void OnApplicationQuit()
		{
			_tcpSocket.Shutdown(SocketShutdown.Both);
				_tcpSocket.Close();
				_udpSocket.Shutdown(SocketShutdown.Both);
				_udpSocket.Close();
		}
		public static Vector3[] DeSerialize(byte[] data)
		{
			byte[] buff = data;
			Vector3[] vect = new Vector3[data.Length / (sizeof(float) * 3)];
			for (int i = 0; i < vect.Length; i++)
			{
				vect[i].x = BitConverter.ToSingle(buff, (i * 3 + 0) * sizeof(float));
				vect[i].y = BitConverter.ToSingle(buff, (i * 3 + 1) * sizeof(float));
				vect[i].z = BitConverter.ToSingle(buff, (i * 3 + 2) * sizeof(float));
			}
			return vect;
		}
	}
}