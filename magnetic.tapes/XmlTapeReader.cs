using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using NameValueDictionary = System.Collections.Generic.Dictionary<string, string>;

namespace Magnetic.Tapes
{
	public class XmlTapeReader : ITapeReader {
		XmlReader reader;
		
		public XmlTapeReader(TextReader reader) {
			this.reader = new XmlTextReader(reader);
		}
		
		public Interaction Read() {
			var document = XDocument.Load(reader);
			var elements = document.XPathSelectElements("//interaction");
			return elements.Any()
				? TransformInteraction(elements.First())
				: null;
		}
		
		Interaction TransformInteraction(XElement element) {
			return new Interaction {
				Request = new Request {
					Verb = element.XPathSelectElement("request/verb").Value,
					Path = element.XPathSelectElement("request/path").Value,
					Body = element.XPathSelectElement("request/body").Value,
					Headers = MapHeaders(element.XPathSelectElement("request/headers"))
				},
				Response = new Response {
					Version = element.XPathSelectElement("response/version").Value,
					Status = MapStatus(element.XPathSelectElement("response/status")),
					Body = element.XPathSelectElement("response/body").Value,
					Headers = MapHeaders(element.XPathSelectElement("response/headers"))
				}
			};
		}
		
		NameValueDictionary MapHeaders(XElement headers) {
			var dictionary = new NameValueDictionary();
			foreach (var element in headers.Elements())
				dictionary.Add(element.Attribute("name").Value, element.Value);
			return dictionary;
		}
		
		ResponseStatus MapStatus(XElement status) {
			var code = 0;
			int.TryParse(status.Element("code").Value, out code);
			return new ResponseStatus {
				Code = code,
				Message = status.Element("message").Value
			};
		}
	}
}

