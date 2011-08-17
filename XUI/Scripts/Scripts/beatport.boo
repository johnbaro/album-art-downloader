import System
import System.Xml
import AlbumArtDownloader.Scripts
import util

class Beatport(AlbumArtDownloader.Scripts.IScript, ICategorised):
	Name as string:
		get: return "Beatport"
	Version as string:
		get: return "0.1"
	Author as string:
		get: return "Alex Vallat"
	Category as string:
		get: return "Dance, Club, Electronic"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		x = XmlDocument()
		x.Load("http://api.beatport.com/catalog/search?v=2.0&format=xml&highlight=false&perPage=40&facets=fieldType:release,performerName:${EncodeUrl(artist)}&query=${EncodeUrl(album)}")
		
		resultNodes = x.SelectNodes("response/result/document/release")
		results.EstimatedCount = resultNodes.Count

		for node as XmlNode in resultNodes:
			thumbnail = node.SelectSingleNode("image[@width=60 and @ref='release']")
			image = node.SelectSingleNode("image[@width=500 and @ref='release']")
			if thumbnail != null and image != null:
				url = "http://www.beatport.com/release/" + node.SelectSingleNode("urlName").InnerText + "/" + node.Attributes.GetNamedItem("id").InnerText
			
				title = StringBuilder()

				performers = node.SelectNodes("performer/name")
				if performers.Count > 8:
					title.Append("Various Artists, ")
				else:
					for performerNode in performers:
						title.Append(performerNode.InnerText + ", ")

				// Remove the last ", "
				title.Remove(title.Length - 2, 2)

				title.Append(" - " + node.SelectSingleNode("name").InnerText)

				results.Add(GetImageUrl(thumbnail), title.ToString(), url, 500, 500, GetImageUrl(image), CoverType.Front);
	
	def GetImageUrl(imageNode as XmlNode):
		return imageNode.Attributes.GetNamedItem("url").InnerText

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;
		