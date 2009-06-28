# refs: System.Core

import System.Xml
import AlbumArtDownloader.Scripts
import util

//Inheritors should override the Suffix property to return a valid amazon suffix (like com, co.uk, de, etc...). To change the search index, override the SearchIndex property.
abstract class Amazon(AlbumArtDownloader.Scripts.IScript):
	virtual Name as string:
		get: return "Amazon (.${Suffix})"
	Version as string:
		get: return "0.10"
	Author as string:
		get: return "Alex Vallat"
	abstract protected Suffix as string:
		get: pass
	virtual protected SearchIndex as string:
		get: return "Music"
	
	def Search(artist as string, album as string, results as IScriptResults):
		x = System.Xml.XmlDocument()
		n = XmlNamespaceManager(x.NameTable)
		n.AddNamespace("a", "http://webservices.amazon.com/AWSECommerceService/2005-10-05")
		
		x.Load("http://ecs.amazonaws.${Suffix}/onca/xml?Service=AWSECommerceService&AWSAccessKeyId=1MV23E34ARMVYMBDZB02&Operation=ItemSearch&SearchIndex=${SearchIndex}&ItemPage=1&ResponseGroup=ItemAttributes,Images&Keywords="+EncodeUrl(artist+" "+album))
		
		resultNodes=x.SelectNodes("a:ItemSearchResponse/a:Items/a:Item[a:ImageSets/a:ImageSet/a:LargeImage/a:URL]", n) //Only want results with large images
		results.EstimatedCount = resultNodes.Count
		
		uniqueResults = System.Collections.Generic.HashSet[of string]()
		
		for node in resultNodes:
			asin = node.SelectSingleNode("a:ASIN", n).InnerText
			title = node.SelectSingleNode("a:ItemAttributes/a:Title", n).InnerText //Can Title ever not exist? Assume it always exists.
			artistNode = node.SelectSingleNode("a:ItemAttributes/a:Artist", n)
			if artistNode != null:
				title = artistNode.InnerText + " - " + title
			
			imageSets = node.SelectNodes("a:ImageSets/a:ImageSet",n)
			results.EstimatedCount += (imageSets.Count - 1)
			for imageSetNode as XmlNode in imageSets:
				fullsize = imageSetNode.SelectSingleNode("a:LargeImage/a:URL", n).InnerText
				if(uniqueResults.Add(fullsize)): //Only add unique results
					width = -1
					height = -1
					widthNode = imageSetNode.SelectSingleNode("a:LargeImage/a:Width", n)
					heightNode = imageSetNode.SelectSingleNode("a:LargeImage/a:Height", n)
					category = imageSetNode.Attributes["Category"].Value
					coverType = CoverType.Unknown;
					if category == "primary":
						coverType = CoverType.Front
					
					if widthNode != null and heightNode != null:
						width = int.Parse(widthNode.InnerText)
						height = int.Parse(heightNode.InnerText)
					
					thumbnail as string = fullsize //Unless a Medium image is present, in which case use that instead
					thumbNode = imageSetNode.SelectSingleNode("a:MediumImage/a:URL", n)
					if thumbNode != null:
						thumbnail = thumbNode.InnerText
					
					results.Add(thumbnail, title, "http://amazon.${Suffix}/dp/"+asin, width, height, fullsize, coverType);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter

