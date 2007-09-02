namespace CoverSources
import System.Xml
import System.Drawing
import util

class Amazon:
	static SourceName as string:
		get: return "Amazon"
	static SourceVersion as string:
		get: return "0.4"
	static SourceCreator as string:
		get: return "Alex Vallat"
	static def GetThumbs(coverart,artist,album):
		x = System.Xml.XmlDocument()
		n = XmlNamespaceManager(x.NameTable);
		n.AddNamespace("a", "http://webservices.amazon.com/AWSECommerceService/2005-10-05");
			
		x.Load("http://ecs.amazonaws.com/onca/xml?Service=AWSECommerceService&AWSAccessKeyId=1MV23E34ARMVYMBDZB02&Operation=ItemSearch&SearchIndex=Music&ItemPage=1&ResponseGroup=ItemAttributes,Images&Keywords="+EncodeUrl(artist+" "+album))

		results=x.SelectNodes("a:ItemSearchResponse/a:Items/a:Item[a:LargeImage/a:URL]", n) //Only want results with large images
		coverart.SetCountEstimate(results.Count)
		for node in results:
		  title = node.SelectSingleNode("a:ItemAttributes/a:Title", n).InnerText //Can Title ever not exist? Assume it always exists.
		  artistNode = node.SelectSingleNode("a:ItemAttributes/a:Artist", n)
		  if artistNode != null:
		    title = artistNode.InnerText + " - " + title
		    
		  width = -1
		  height = -1
		  widthNode = node.SelectSingleNode("a:LargeImage/a:Width", n)
		  heightNode = node.SelectSingleNode("a:LargeImage/a:Height", n)
		  if widthNode != null and heightNode != null:
		    width = int.Parse(widthNode.InnerText)
		    height = int.Parse(heightNode.InnerText)
		  
		  fullsize = node.SelectSingleNode("a:LargeImage/a:URL", n).InnerText
		  
		  thumbnail = fullsize //Unless a Medium image is present, in which case use that instead
		  thumbNode = node.SelectSingleNode("a:MediumImage/a:URL", n)
		  if thumbNode != null:
		    thumbnail = thumbNode.InnerText
		  
		  coverart.AddThumb(thumbnail, title, width, height, fullsize);

	static def GetResult(param):
		return param

