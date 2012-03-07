using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using NameValueDictionary = System.Collections.Generic.Dictionary<string, string>;

namespace Magnetic.Tapes
{
	public class ProxyClient : IDisposable
	{
		Socket clientSocket;
		Socket remoteSocket;
		Action successHandler;
		Action completeHandler;
		Byte[] clientBuffer;	
		Byte[] remoteBuffer;	
		
		public ProxyClient(Socket socket)
		{
//			Console.WriteLine ("Creating Client...");
			
			remoteBuffer = new byte[4096];
			clientBuffer = new byte[4096];
			clientSocket = socket;
		}
		
		public void Dispose() {
//			Console.WriteLine ("Disposing Client...");
			if (clientSocket != null) clientSocket.Kill();
			if (remoteSocket != null) remoteSocket.Kill();
			if (completeHandler != null)
				completeHandler();
		}

		public ProxyClient Complete(Action completeHandler) {
			this.completeHandler = completeHandler;
			return this;
		}

		public ProxyClient Handshake(Action successHandler) {
			this.successHandler = successHandler;
			clientSocket.BeginReceive(clientBuffer, 0, clientBuffer.Length, SocketFlags.None, new AsyncCallback(Client_BeginHandshake), null);
			return this;
		}
		
		void Client_BeginHandshake(IAsyncResult ar) {
			var length = clientSocket.EndReceive(ar);
			if (length > 0) ProcessHandshake(Encoding.ASCII.GetString(clientBuffer, 0, length));
			else {
				Console.WriteLine("Client_BeginHandshake recieved nothing");
				Dispose();
			}
		}
		
		void ProcessHandshake(string clientMessage) {
//			Console.WriteLine ("Client_BeginHandshake");
//			Console.WriteLine (clientMessage);

			var request = ParseRequest(clientMessage);
			var remoteEndpoint = ParseEndpoint(request.Headers["Host"]);
			
			remoteSocket = new Socket(remoteEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

			if (request.KeepAlive) {
				remoteSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, 1);
			}
			
			remoteSocket.BeginConnect(remoteEndpoint, new AsyncCallback(Remote_CompleteHandshake), request);
		}
		
		IPEndPoint ParseEndpoint(string host) {
			var port = 80;
			if (host.IndexOf(":") != -1) {
				var values = host.Split (':');
				host = values[0];
				port = Int32.Parse(values[1]);
			}

			return new IPEndPoint(Dns.GetHostEntry(host).AddressList[0], port);
		}
		
		Request ParseRequest(string clientMessage) {
			var query = ParseQuery(clientMessage);
			var headers = ParseHeaders(clientMessage);
			var request = new Request();
			request.Verb = query["verb"];
			request.Path = ParseRelativePath(query["path"]);
			request.Version = query["version"];
			
			foreach (var key in headers.Keys) {
				if (key == "Proxy-Connection") {
					request.KeepAlive = string.Compare(headers["Proxy-Connection"], "keep-alive", true) == 0;
				}
				else {
					request.Headers[key] = headers[key];
				}
			}
			
			return request;
		}
		
		string ParseRelativePath(string path) {
			if (!path.StartsWith("http://")) return path;

			var slash = path.IndexOf("/", 7);
			return slash < 0 ? "/" : path.Substring(slash);
		}
		
		NameValueDictionary ParseQuery(string request) {
			var query = new NameValueDictionary();
			var match = Regex.Match(request,
				@"^(?<verb>\w+) (?<path>[^\s]+) (?<version>.*)\r\n");
			query["verb"] = match.Groups["verb"].Value;
			query["path"] = match.Groups["path"].Value;
			query["version"] = match.Groups["version"].Value;
			return query;
		}
		
		NameValueDictionary ParseHeaders(string request) {
			Match match;
			var headers = new NameValueDictionary();
			
			match = Regex.Match(request, @"Host: (.*?)\r\n", RegexOptions.Multiline);
			headers["Host"] = match.Success
				? match.Groups[1].Value : null;

			match = Regex.Match(request, @"Proxy-Connection: (.*?)\r\n", RegexOptions.Multiline);
			headers["Proxy-Connection"] = match.Success
				? match.Groups[1].Value : null;
			
			
			return headers;
		}
		
		void Remote_CompleteHandshake(IAsyncResult ar) {
			remoteSocket.EndConnect(ar);
			
			var request = (Request)ar.AsyncState;
			var remoteMessage = StringifyRequest(request);
			
//			Console.WriteLine("Remote_CompleteHandshake");
//			Console.WriteLine(remoteMessage);
			
			remoteSocket.BeginSend(Encoding.ASCII.GetBytes (remoteMessage),
			    0, remoteMessage.Length, SocketFlags.None, new AsyncCallback(Remote_RequestSent), null);
		}
		
		string StringifyRequest(Request request) {
			var builder = new StringBuilder();
			builder.AppendFormat("{0} {1} {2}\r\n", request.Verb, request.Path, request.Version);
			foreach (var key in request.Headers.Keys) {
				builder.AppendFormat("{0}: {1}\r\n", key, request.Headers[key]);
			}
			builder.Append ("\r\n");
			return builder.ToString ();
		}
		
		void Remote_RequestSent(IAsyncResult ar) {
			if (remoteSocket.EndSend (ar) != -1) {
				Relay();
				if (successHandler != null) {
					successHandler();
				}
			}
			else {
				Console.WriteLine("Remote_CompleteRequest sent nothing");
				Dispose();
			}
		}
		
		void Relay() {
//			Console.WriteLine ("Starting relay...");
			clientSocket.BeginReceive(clientBuffer, 0, clientBuffer.Length, SocketFlags.None, new AsyncCallback(Client_RelayRecieved), null);
			remoteSocket.BeginReceive(remoteBuffer, 0, remoteBuffer.Length, SocketFlags.None, new AsyncCallback(Remote_RelayRecieved), null);
		}
		
		void Client_RelayRecieved(IAsyncResult ar) {
			if (!clientSocket.Connected) return;

			var length = clientSocket.EndReceive(ar);
//			Console.WriteLine ("Client_RelayRecieved ({0})", length);

			if (length > 0) {
				remoteSocket.BeginSend(clientBuffer, 0, length, SocketFlags.None, new AsyncCallback(Remote_RelaySent), null);
			}
			else {
				Console.WriteLine("Client_RelayRecieved recieved nothing");
				Dispose ();
			}
		}

		void Client_RelaySent(IAsyncResult ar) {
			if (!clientSocket.Connected) return;
			
			var length = clientSocket.EndSend (ar);
//			Console.WriteLine ("Client_RelaySent ({0})", length);

			if (length > 0) {
				remoteSocket.BeginReceive(remoteBuffer, 0, remoteBuffer.Length, SocketFlags.None, new AsyncCallback(Remote_RelayRecieved), null);
			}
			else {
				Console.WriteLine("Client_RelaySent sent nothing");
				Dispose ();
			}
		}
		
		void Remote_RelayRecieved(IAsyncResult ar) {
			if (!remoteSocket.Connected) return;

			var length = remoteSocket.EndReceive(ar);
//			Console.WriteLine ("Remote_RelayRecieved ({0})", length);

			if (length > 0) {
				clientSocket.BeginSend (remoteBuffer, 0, length, SocketFlags.None, new AsyncCallback(Client_RelaySent), null);
			}
			else {
				Console.WriteLine("Remote_RelayRecieve recieved nothing");
				Dispose ();
			}
		}
		
		void Remote_RelaySent(IAsyncResult ar) {
			if (!remoteSocket.Connected) return;

			var length = remoteSocket.EndSend (ar);
//			Console.WriteLine ("Remote_RelaySent ({0})", length);

			if (length > 0) {
				clientSocket.BeginReceive(clientBuffer, 0, clientBuffer.Length, SocketFlags.None, new AsyncCallback(Client_RelayRecieved), null);
			}
			else {
				Console.WriteLine("Remote_RelaySent sent nothing");
				Dispose ();
			}
		}
	}
}

