using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using NUnit.Framework;
using FluentAssertions;

namespace Magnetic.Tapes.Tests
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
				new XElement("verb", request.Verb)
			);
		}
	}
	
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
	public class When_writing_an_interaction_to_tape : XmlTapeSpecification {

		[Test]
		public void it_should_write_a_document_element() {
			xdocument.Should ().HaveRoot("tape");
		}
		
		[Test]
		public void it_should_write_an_interaction_element() {
			xdocument.Should ().HaveElement ("interaction");
		}
		
		[Test]
		public void it_should_write_a_request_element() {
			xdocument.Descendants("interaction").First()
				.Should ().HaveElement("request");
		}
		
		[Test]
		public void it_should_write_a_response_element() {
			xdocument.Descendants("interaction").First()
				.Should ().HaveElement("response");
		}
	}
	
	[TestFixture]
	public class When_writing_a_request_to_tape : XmlTapeSpecification {
		XElement xrequest;
		
		protected override void Context() {
			base.Context();
			interaction.Request.Verb = "abc";
		}
		
		protected override void Because() {
			base.Because();
			xrequest = xdocument.Descendants("request").First();
		}
		
		[Test]
		public void it_should_write_the_http_verb() {
			xrequest.Should().HaveElement("verb");
			xrequest.Descendants("verb").First().Value.Should().Be("abc");
		}
	}
}

