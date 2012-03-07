using System;
using System.Net;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Magnetic.Example.Tests.RSS
{
	public class Reader
	{
		string uri;
		
		public Reader (string uri)
		{
			this.uri = uri;
		}
		
		public Feed Read() {
			var response = new WebClient().DownloadString(uri);
			var document = XDocument.Parse(response);
			var channel = document.Descendants("channel").First();
			return new Feed {
				Version = document.Root.Attribute("version").Value,
				Title = channel.Element("title").Value,
				Link = channel.Element("link").Value,
				Language = channel.Element("language").Value,
				Description = channel.Element("description").Value
			};
		}
	}

	public class Feed {
		public string Version;
		public string Title;
		public string Link;
		public string Language;
		public string Description;
	}
}

