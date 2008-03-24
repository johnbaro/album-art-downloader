import System.Xml
import System.Drawing
import AlbumArtDownloader.Scripts
import util

//Inheritors should override the Suffix property to return a valid amazon suffix (like com, co.uk, de, etc...)
abstract class Amazon(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "Amazon (.${Suffix})"
	Version as string:
		get: return "0.5"
	Author as string:
		get: return "Alex Vallat"
	abstract protected Suffix as string:
		get: pass
	def Search(artist as string, album as string, results as IScriptResults):
		x = System.Xml.XmlDocument()
		n = XmlNamespaceManager(x.NameTable);
		n.AddNamespace("a", "http://webservices.amazon.com/AWSECommerceService/2005-10-05");
			
		x.Load("http://ecs.amazonaws.${Suffix}/onca/xml?Service=AWSECommerceService&AWSAccessKeyId=1MV23E34ARMVYMBDZB02&Operation=ItemSearch&SearchIndex=Music&ItemPage=1&ResponseGroup=ItemAttributes,Images&Keywords="+EncodeUrl(artist+" "+album))

		resultNodes=x.SelectNodes("a:ItemSearchResponse/a:Items/a:Item[a:LargeImage/a:URL]", n) //Only want results with large images
		results.SetCountEstimate(resultNodes.Count)
		for node in resultNodes:
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
		  
		  thumbnail as string = fullsize //Unless a Medium image is present, in which case use that instead
		  thumbNode = node.SelectSingleNode("a:MediumImage/a:URL", n)
		  if thumbNode != null:
		    thumbnail = thumbNode.InnerText
		  
		  results.AddThumb(thumbnail, title, width, height, fullsize);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter

