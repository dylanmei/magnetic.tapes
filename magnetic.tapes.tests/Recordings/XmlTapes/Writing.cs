using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using NUnit.Framework;
using FluentAssertions;

namespace Magnetic.Tapes.Tests.Recordings
{
	public class XmlTapeWriterSpecification :Specification
	{
		protected XmlTapeWriter writer;
		protected Interaction interaction;
		protected XDocument document;
		StringBuilder buffer;
		
		protected override void Context()
		{
			interaction = new Interaction();
			buffer = new StringBuilder();
			writer = new XmlTapeWriter(new StringWriter(buffer));
		}
		
		protected override void Because() {
			writer.Write(interaction);
			document = XDocument.Parse(buffer.ToString());
		}
	}
	
	[TestFixture]
	public class When_writing_an_interaction_to_tape
		: XmlTapeWriterSpecification {

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
			document.Should().HaveRoot("tape");
		}
		
		[Test]
		public void it_should_write_an_interaction_element() {
			document.Root.Should().HaveElement("interaction");
		}
		
		[Test]
		public void it_should_write_the_request_verb() {
			document.XPathSelectElement("//request/verb").Value
				.Should().Be("abc");
		}
		
		[Test]
		public void it_should_write_the_request_path() {
			document.XPathSelectElement("//request/path").Value
				.Should().Be("xyz");
		}
		
		[Test]
		public void it_should_write_the_request_body() {
			document.XPathSelectElement("//request/body").Value
				.Should().Be("ice cream");
		}
		
		[Test]
		public void it_should_write_the_request_header_names_and_values() {
			document.XPathSelectElement("//request/headers/header[@name='a']").Value
				.Should().Be("raspberry");
			document.XPathSelectElement("//request/headers/header[@name='b']").Value
				.Should().Be("strawberry");
		}

		[Test]
		public void it_should_write_the_response_status_code() {
			document.XPathSelectElement("//response/status/code").Value
				.Should().Be("123");
		}

		[Test]
		public void it_should_write_the_response_status_message() {
			document.XPathSelectElement("//response/status/message").Value
				.Should().Be("abc");
		}

		[Test]
		public void it_should_write_the_http_response_version() {
			document.XPathSelectElement("//response/version").Value
				.Should().Be("xyz");
		}
		
		[Test]
		public void it_should_write_the_response_body() {
			document.XPathSelectElement("//response/body").Value
				.Should().Be("cup cakes");
		}

		[Test]
		public void it_should_write_the_response_header_names_and_values() {
			document.XPathSelectElement("//response/headers/header[@name='a']").Value
				.Should().Be("vanilla");
			document.XPathSelectElement("//response/headers/header[@name='b']").Value
				.Should().Be("chocolate");
		}
	}
}

