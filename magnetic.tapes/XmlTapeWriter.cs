using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace Magnetic.Tapes
{
	public class XmlTapeWriter : ITapeWriter {
		XmlWriter writer;
		
		public XmlTapeWriter(TextWriter writer) {
			this.writer = new XmlTextWriter(writer);
		}
		
		public void Write(Interaction interaction) {
			var tree = new XElement("tape",
			    new XElement("interaction", 
					MapRequest(interaction.Request),
			        MapResponse(interaction.Response)
			    )
			);
			
			tree.WriteTo(writer);
		}

		XElement MapRequest(Request request) {
			return new XElement("request",
				new XElement("verb", request.Verb),
				new XElement("path", request.Path),
				new XElement("body", MapBody(request.Body)),
				new XElement("headers", MapHeaders(request.Headers)));
		}

		XElement MapResponse(Response response) {
			return new XElement("response",
				new XElement("status",
			    	new XElement("code", response.Status.Code),
			        new XElement("message", response.Status.Message)),
			    new XElement("version", response.Version),
			    new XElement("body", MapBody(response.Body)),
				new XElement("headers", MapHeaders(response.Headers)));
		}

		XText MapBody(string body) {                
			return string.IsNullOrEmpty(body)
				? new XText("")
				: new XCData(body);
		}
		
		IEnumerable<XElement> MapHeaders(Dictionary<string, string> dictionary) {
			return from entry in dictionary
			select new XElement("header", new XAttribute("name", entry.Key), entry.Value);
		}
	}
}

