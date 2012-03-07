using System;
using NUnit.Framework;

namespace Magnetic.Example.Tests.RSS
{
	[TestFixture]
	public class HeaderTests
	{
		Feed feed;
		
		[SetUp]
		public void Setup() {
			feed = new Reader("http://www.nasa.gov/rss/breaking_news.rss").Read();
		}
		
		[Test]
		public void TestVersion () {
			Assert.AreEqual("2.0", feed.Version);
		}

		[Test]
		public void TestTitle() {
			Assert.AreEqual("NASA Breaking News", feed.Title);
		}
		
		[Test]
		public void TestReferenceLink() {
			Assert.AreEqual("http://www.nasa.gov/audience/formedia/features/index.html", feed.Link);
		}
		
		[Test]
		public void TestLanguage() {
			Assert.AreEqual("en-us", feed.Language);
		}
		
		[Test]
		public void TestDescription() {
			Assert.AreEqual("A RSS news feed containing the latest NASA news articles and press releases.", feed.Description);
		}
	}
}
