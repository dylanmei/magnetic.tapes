using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Magnetic.Tapes
{
	public class XmlTape : ITape {
		XmlWriter writer;
		
		public XmlTape(TextWriter writer) {
			this.writer = new XmlTextWriter(writer);
		}
		
		public void Write(Interaction interaction) {
			var tree = new XElement("tape",
			    new XElement("interaction", 
					TransformRequest(interaction.Request),
				    new XElement("response")
			    )
			);
			
			tree.WriteTo(writer);
		}

		XElement TransformRequest(Request request) {
			return new XElement("request",
				new XElement("verb", request.Verb),
				new XElement("path", request.Path),
				new XElement("body", TransformRequestBody(request.Body)),
				new XElement("headers",
					from entry in request.Headers
					select new XElement(entry.Key, entry.Value))
			);
		}

		XText TransformRequestBody(string body) {                
			return string.IsNullOrEmpty(body)
				? new XText("")
				: new XCData(body);
		}
	}
}

