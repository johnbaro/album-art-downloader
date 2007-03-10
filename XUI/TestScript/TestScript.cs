using System;
using System.Collections.Generic;
using System.Text;
using AlbumArtDownloader.Scripts;
using System.Drawing;

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
			results.EstimatedCount = 2;
			Bitmap thumbnail = new Bitmap(typeof(TestScript), "testThumbnail.jpg");
			Bitmap fullSize = new Bitmap(typeof(TestScript), "testFullsize.jpg");
			results.Add(thumbnail, "Bitmap", 600, 600, fullSize);
			results.Add(@"file://C:\Documents and Settings\David Vallat\My Documents\Alexander\foobar2000\CoverDownloader\AlbumArtDownloader\XUI\TestScript\testThumbnail.jpg", "String", 400, 400, @"file://C:\Documents and Settings\David Vallat\My Documents\Alexander\foobar2000\CoverDownloader\AlbumArtDownloader\XUI\TestScript\testFullsize.jpg");
			results.EstimatedCount = 5;			
			results.Add("http://ec2.images-amazon.com/images/P/B0000C7GG2.01._AA240_SCLZZZZZZZ_.jpg", "URL", "http://ec2.images-amazon.com/images/P/B0000C7GG2.01._SS500_SCLZZZZZZZ_.jpg");
			results.Add(thumbnail, "No Fullsize", null);
			results.Add(@"file://C:\Documents and Settings\David Vallat\My Documents\Alexander\foobar2000\CoverDownloader\AlbumArtDownloader\XUI\TestScript\testThumbnail.png", "PNG format", 600, 600, @"file://C:\Documents and Settings\David Vallat\My Documents\Alexander\foobar2000\CoverDownloader\AlbumArtDownloader\XUI\TestScript\testFullsize.png");
			results.Add(@"http://www.google.com/invalid", "Not valid url", null);
		}

		public object RetrieveFullSizeImage(object fullSizeCallbackParameter)
		{
			return fullSizeCallbackParameter;
		}
	}
}
