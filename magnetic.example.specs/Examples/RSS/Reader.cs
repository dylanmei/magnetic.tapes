using System;
using System.Net;
using Magnetic.Tapes;
using Machine.Specifications;
using System.Linq;
using System.Xml.Linq;

namespace Magnetic.Example.Specs.RSS
{
	public class Reader
	{
		string uri;
		
		public Reader (string uri)
		{
			this.uri = uri;
		}
		
		public Feed Read() {
			var client = new WebClient();
			var response = client.DownloadString(uri);
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

	[Tape]
	public class When_reading_a_feed {
		static Feed feed;
		static Reader reader;
		
		Establish context = () =>
			reader = new Reader("http://www.nasa.gov/rss/breaking_news.rss");
		Because of = () => feed = reader.Read();
		
		It should_show_the_RSS_version = () =>
			feed.Version.ShouldEqual("2.0");

		It should_show_a_title = () =>
			feed.Title.ShouldEqual("NASA Breaking News");
		
		It should_declare_a_reference_link = () =>
			feed.Link.ShouldEqual("http://www.nasa.gov/audience/formedia/features/index.html");
		
		It should_declare_a_language = () =>
			feed.Language.ShouldEqual("en-us");
		
		It should_declare_a_description = () =>
			feed.Description.ShouldEqual("A RSS news feed containing the latest NASA news articles and press releases.");
	}
}

