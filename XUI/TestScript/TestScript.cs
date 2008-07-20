using System;
using System.Collections.Generic;
using System.Text;
using AlbumArtDownloader.Scripts;
using System.Drawing;
using System.Xml;

namespace TestScript
{
	public class TestScript : IScript
	{
		public string Name
		{
			get { return "Test Script"; }
		}

		public string Author
		{
			get { return "Alex Vallat"; }
		}

		public string Version
		{
			get { return typeof(TestScript).Assembly.GetName().Version.ToString(); }
		}

		public void Search(string artist, string album, IScriptResults results)
		{
			/*
			var x= new XmlDocument();
			x.Load("http://ecs.amazonaws.com/onca/xml?Service=AWSECommerceService&AWSAccessKeyId=1MV23E34ARMVYMBDZB02&Operation=ItemSearch&SearchIndex=Music&ItemPage=1&ResponseGroup=ItemAttributes,Images&Keywords="+artist+" "+album);

			XmlNamespaceManager nsmgr = new XmlNamespaceManager(x.NameTable);
			nsmgr.AddNamespace("a", "http://webservices.amazon.com/AWSECommerceService/2005-10-05");
			XmlNodeList nodes=x.SelectNodes("a:ItemSearchResponse/a:Items/a:Item", nsmgr);
			results.SetCountEstimate(nodes.Count);
			foreach (XmlNode node in nodes)
			{
				results.AddThumb(node.SelectSingleNode("a:MediumImage/a:URL", nsmgr).InnerText, node.SelectSingleNode("a:ItemAttributes/a:Artist", nsmgr).InnerText + " - " + node.SelectSingleNode("a:ItemAttributes/a:Title", nsmgr).InnerText, int.Parse(node.SelectSingleNode("a:LargeImage/a:Width", nsmgr).InnerText), int.Parse(node.SelectSingleNode("a:LargeImage/a:Height", nsmgr).InnerText), node.SelectSingleNode("a:LargeImage/a:URL", nsmgr).InnerText);
			}
			
			results.EstimatedCount = 2;
			Bitmap thumbnail = new Bitmap(typeof(TestScript), "testThumbnail.jpg");
			Bitmap fullSize = new Bitmap(typeof(TestScript), "testFullsize.jpg");
			results.Add(thumbnail, "Bitmap", 600, 600, fullSize);
			results.Add(@"file://C:\Documents and Settings\David Vallat\My Documents\Alexander\foobar2000\CoverDownloader\AlbumArtDownloader\XUI\TestScript\testThumbnail.jpg", "String", 400, 400, @"file://C:\Documents and Settings\David Vallat\My Documents\Alexander\foobar2000\CoverDownloader\AlbumArtDownloader\XUI\TestScript\testFullsize.jpg");
			results.Add(thumbnail, "No Fullsize", null);
			
			results.EstimatedCount = 5;			
			System.Threading.Thread.Sleep(3000);
			results.Add("http://ec2.images-amazon.com/images/P/B0000C7GG2.01._AA240_SCLZZZZZZZ_.jpg", "URL", "http://ec2.images-amazon.com/images/P/B0000C7GG2.01._SS500_SCLZZZZZZZ_.jpg");
			results.Add(@"file://C:\Documents and Settings\David Vallat\My Documents\Alexander\foobar2000\CoverDownloader\AlbumArtDownloader\XUI\TestScript\testThumbnail.png", "PNG format", 600, 600, @"file://C:\Documents and Settings\David Vallat\My Documents\Alexander\foobar2000\CoverDownloader\AlbumArtDownloader\XUI\TestScript\testFullsize.png");
			results.Add(@"http://www.google.com/invalid", "Not valid url", null);
			 */

			int numberOfResults = 50;
			results.EstimatedCount = numberOfResults;
			Random rnd = new Random();
			for (int i = 0; i < numberOfResults; i++)
			{
				Bitmap thumbnail = new Bitmap(typeof(TestScript), "testThumbnail.jpg");
				Bitmap fullSize = new Bitmap(typeof(TestScript), "testFullsize.jpg");
				results.Add(thumbnail, i.ToString(), "notauri", rnd.Next(10, 1000), rnd.Next(10, 1000), fullSize);
				System.Threading.Thread.Sleep(1000);
			}
		}

		public object RetrieveFullSizeImage(object fullSizeCallbackParameter)
		{
			return fullSizeCallbackParameter;
		}
	}
}
