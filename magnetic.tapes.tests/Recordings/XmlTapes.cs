using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using NUnit.Framework;
using FluentAssertions;

namespace Magnetic.Tapes.Tests
{
	public class XmlTapeSpecification :Specification
	{
		protected XmlTape tape;
		protected Interaction interaction;
		protected XDocument xdocument;
		StringBuilder buffer;
		
		protected override void Context()
		{
			interaction = new Interaction();
			buffer = new StringBuilder();
			tape = new XmlTape(new StringWriter(buffer));
		}
		
		protected override void Because() {
			tape.Write(interaction);
			xdocument = XDocument.Parse(buffer.ToString());
		}
	}
	
	[TestFixture]
	public class When_writing_an_interaction_to_tape
		: XmlTapeSpecification {

		[Test]
		public void it_should_write_a_document_element() {
			xdocument.Should().HaveRoot("tape");
		}
		
		[Test]
		public void it_should_write_an_interaction_element() {
			xdocument.Should().HaveElement ("interaction");
		}
		
		[Test]
		public void it_should_write_a_request_element() {
			xdocument.Root.Element("interaction")
				.Should().HaveElement("request");
		}
		
		[Test]
		public void it_should_write_a_response_element() {
			xdocument.Root.Element("interaction")
				.Should().HaveElement("response");
		}
	}
	
	[TestFixture]
	public class When_writing_a_request_to_tape
		: XmlTapeSpecification {
		
		XElement xrequest;
		XElement xheaders;
		
		protected override void Context() {
			base.Context();
			interaction.Request.Verb = "abc";
			interaction.Request.Path = "xyz";
			interaction.Request.Body = "123";
			interaction.Request.Headers.Add("a", "1");
			interaction.Request.Headers.Add("b", "2");
		}
		
		protected override void Because() {
			base.Because();
			xrequest = xdocument.Descendants("request").First();
			xheaders = xrequest.Descendants("headers").First();
		}
		
		[Test]
		public void it_should_write_the_http_verb() {
			xrequest.Should().HaveElement("verb");
			xrequest.Element("verb").Value.Should().Be("abc");
		}
		
		[Test]
		public void it_should_write_the_http_path() {
			xrequest.Should().HaveElement("path");
			xrequest.Element("path").Value.Should().Be("xyz");
		}
		
		[Test]
		public void it_should_write_the_request_body() {
			xrequest.Should().HaveElement("body");
			xrequest.Element("body").Value.Should().Be("123");
		}
		
		[Test]
		public void it_should_write_the_request_headers() {
			xheaders.Should ().NotBeNull();
		}
		
		[Test]
		public void it_should_write_the_request_header_names_and_values() {
			xheaders.Should().HaveElement("a");
			xheaders.Element ("a").Value.Should().Be("1");

			xheaders.Should().HaveElement("b");
			xheaders.Element ("b").Value.Should().Be("2");
		}
	}
}

