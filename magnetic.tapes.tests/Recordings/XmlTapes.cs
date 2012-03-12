using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
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

		protected override void Context() {
			base.Context();

			interaction.Request.Verb = "abc";
			interaction.Request.Path = "xyz";
			interaction.Request.Body = "ice cream";
			interaction.Request.Headers.Add("a", "raspberry");
			interaction.Request.Headers.Add("b", "strawberry");
			
			interaction.Response.Status.Code = 123;
			interaction.Response.Status.Message = "abc";
			interaction.Response.Version = "xyz";
			interaction.Response.Body = "cup cakes";
			interaction.Response.Headers.Add("a", "vanilla");
			interaction.Response.Headers.Add("b", "chocolate");
		}
		
		[Test]
		public void it_should_write_a_document_element() {
			xdocument.Should().HaveRoot("tape");
		}
		
		[Test]
		public void it_should_write_an_interaction_element() {
			xdocument.Root.Should().HaveElement("interaction");
		}
		
		[Test]
		public void it_should_write_the_http_verb() {
			xdocument.XPathSelectElement("//request/verb").Value
				.Should().Be("abc");
		}
		
		[Test]
		public void it_should_write_the_http_path() {
			xdocument.XPathSelectElement("//request/path").Value
				.Should().Be("xyz");
		}
		
		[Test]
		public void it_should_write_the_request_body() {
			xdocument.XPathSelectElement("//request/body").Value
				.Should().Be("ice cream");
		}
		
		[Test]
		public void it_should_write_the_request_header_names_and_values() {
			xdocument.XPathSelectElement("//request/headers/a").Value
				.Should().Be("raspberry");
			xdocument.XPathSelectElement("//request/headers/b").Value
				.Should().Be("strawberry");
		}

		[Test]
		public void it_should_write_the_http_status_code() {
			xdocument.XPathSelectElement("//response/status/code").Value
				.Should().Be("123");
		}

		[Test]
		public void it_should_write_the_http_status_message() {
			xdocument.XPathSelectElement("//response/status/message").Value
				.Should().Be("abc");
		}

		[Test]
		public void it_should_write_the_http_version() {
			xdocument.XPathSelectElement("//response/version").Value
				.Should().Be("xyz");
		}
		
		[Test]
		public void it_should_write_the_response_body() {
			xdocument.XPathSelectElement("//response/body").Value
				.Should().Be("cup cakes");
		}

		[Test]
		public void it_should_write_the_response_header_names_and_values() {
			xdocument.XPathSelectElement("//response/headers/a").Value
				.Should().Be("vanilla");
			xdocument.XPathSelectElement("//response/headers/b").Value
				.Should().Be("chocolate");
		}
	}
}

