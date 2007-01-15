namespace CoverSources
import System.Xml
import System.Drawing
import util

class Amazon:
	static ThumbSize = Size(150, 150)	//Size here should match thumbnail size in CoverDownloader settings for best results
	static SourceName as string:
		get: return "Amazon"
	static SourceVersion as string:
		get: return "0.2"
	static def GetThumbs(coverart,artist,album):
		x=System.Xml.XmlDocument()
		x.Load("http://xml.amazon.com/onca/xml3?f=xml&t=webservices-20&dev-t=1MV23E34ARMVYMBDZB02&type=lite&page=1&mode=music&KeywordSearch="+EncodeUrl(artist+" "+album))
		results=x.GetElementsByTagName("Details")
		coverart.SetCountEstimate(results.Count)
		for node in results:
			large = System.Drawing.Bitmap.FromStream(GetPageStream(node["ImageUrlLarge"].InnerText))
			if large.Height>10:
				width = large.Width
				height = large.Height

				//Create the thumbnail.
				thumb = ResizeBitmap(large, ThumbSize.Width, ThumbSize.Height)
				large.Dispose()

				//Add the size to the label
				label = node["ProductName"].InnerText

				coverart.AddThumb(thumb,label,width,height,node["ImageUrlLarge"].InnerText)
	static def GetResult(param):
		return param

