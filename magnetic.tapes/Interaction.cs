using System;
using System.Collections;
using System.Collections.Generic;
using NameValueDictionary = System.Collections.Generic.Dictionary<string, string>;

namespace Magnetic.Tapes
{
	public class Interaction {
		public Request Request = new Request();
		public Response Response = new Response();
	}
	
	public class Request {
		public string Verb;
		public string Path;
		public string Version;
		public bool KeepAlive;
		public NameValueDictionary Headers;
		public string Body;

		public Request() {
			Headers = new NameValueDictionary();
		}
	}
	
	public class Response {
		public int Status;
		public string Message;
		public string Version;
		public NameValueDictionary Headers;
		public string Body;
		
		public Response() {
			Headers = new NameValueDictionary();
		}
	}
}

