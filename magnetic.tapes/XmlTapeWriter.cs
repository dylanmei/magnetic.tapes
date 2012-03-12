using System;
using System.IO;
using System.Linq;
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
					TransformRequest(interaction.Request),
			        TransformResponse(interaction.Response)
			    )
			);
			
			tree.WriteTo(writer);
		}

		XElement TransformRequest(Request request) {
			return new XElement("request",
				new XElement("verb", request.Verb),
				new XElement("path", request.Path),
				new XElement("body", TransformBody(request.Body)),
				new XElement("headers",
					from entry in request.Headers
					select new XElement(entry.Key, entry.Value)));
		}

		XText TransformBody(string body) {                
			return string.IsNullOrEmpty(body)
				? new XText("")
				: new XCData(body);
		}
		
		XElement TransformResponse(Response response) {
			return new XElement("response",
				new XElement("status",
			    	new XElement("code", response.Status.Code),
			        new XElement("message", response.Status.Message)),
			    new XElement("version", response.Version),
			    new XElement("body", TransformBody(response.Body)),
				new XElement("headers",
					from entry in response.Headers
					select new XElement(entry.Key, entry.Value)));
		}
	}
}

