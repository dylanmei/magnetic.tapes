using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using NUnit.Framework;
using FluentAssertions;

using NameValueDictionary = System.Collections.Generic.Dictionary<string, string>;

namespace Magnetic.Tapes.Tests.Recordings
{
	public class XmlTapeReaderSpecification : Specification
	{
		protected XmlTapeReader reader;
		protected Interaction interaction;
		protected string contents;
		
		protected override void Context()
		{
			reader = new XmlTapeReader(
				new StringReader(contents ?? "<tape />"));
		}
		
		protected override void Because() {
			interaction = reader.Read();
		}
	}
	
	[TestFixture]
	public class When_reading_an_empty_tape
		: XmlTapeReaderSpecification {
		
		[Test]
		public void it_should_not_return_interactions() {
			interaction.Should().BeNull();
		}
	}
	
	[TestFixture]
	public class When_reading_an_interaction_from_tape
		: XmlTapeReaderSpecification {
		
		protected override void Context() {
			contents = @"
			<tape>
				<interaction>
					<request>
						<verb>abc</verb>
						<path>easy/street</path>
						<body><![CDATA[cookies & cream]]></body>
						<headers>
							<header name=""dodge"">dart</header>
							<header name=""ford"">falcon</header>
						</headers>
					</request>
					<response>
						<status>
							<code>123</code>
							<message>minty</message>
						</status>
						<version>1.2.3</version>
						<body><![CDATA[popcorn & butter]]></body>
						<headers>
							<header name=""ethel"">merman</header>
							<header name=""woody"">herman</header>
						</headers>
					</response>
				</interaction>
			</tape>";
			base.Context();
		}
		
		[Test]
		public void it_should_read_the_interaction() {
			interaction.Should().NotBeNull();
		}
		
		[Test]
		public void it_should_read_the_request_verb() {
			interaction.Request.Verb.Should().Be("abc");
		}
		
		[Test]
		public void it_should_read_the_request_path() {
			interaction.Request.Path.Should().Be("easy/street");
		}
		
		[Test]
		public void it_should_read_the_request_body() {
			interaction.Request.Body.Should().Be("cookies & cream");
		}
		
		[Test]
		public void it_should_read_the_request_header_names_and_values() {
			interaction.Request.Headers["dodge"].Should().Be("dart");
			interaction.Request.Headers["ford"].Should().Be("falcon");
		}
		
		[Test]
		public void it_should_read_the_response_status_code() {
			interaction.Response.Status.Code.Should().Be(123);
		}
		
		[Test]
		public void it_should_read_the_response_status_message() {
			interaction.Response.Status.Message.Should().Be("minty");
		}

		[Test]
		public void it_should_read_the_response_version() {
			interaction.Response.Version.Should().Be("1.2.3");
		}
		
		[Test]
		public void it_should_read_the_response_body() {
			interaction.Response.Body.Should().Be("popcorn & butter");
		}
		
		[Test]
		public void it_should_read_the_response_header_names_and_values() {
			interaction.Response.Headers["ethel"].Should().Be("merman");
			interaction.Response.Headers["woody"].Should().Be("herman");
		}
	}
}