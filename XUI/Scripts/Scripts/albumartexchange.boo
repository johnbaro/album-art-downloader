import System.Xml
import AlbumArtDownloader.Scripts
import util

class AlbumArtExchange(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "Album Art Exchange"
	Version as string:
		get: return "0.2"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
		x = System.Xml.XmlDocument()
			
		x.Load("http://www.albumartexchange.com/search.php?q="+EncodeUrl(artist+" "+album))

		resultNodes=x.SelectNodes("search-results/image-info")
		results.EstimatedCount = resultNodes.Count

		for node in resultNodes:
			thumbnail = node.SelectSingleNode("thumbnail").InnerText
			full = node.SelectSingleNode("image").InnerText
			title = node.SelectSingleNode("title").InnerText
			page = node.SelectSingleNode("gallery-page").InnerText
			width = System.Int32.Parse(node.SelectSingleNode("dimensions/width").InnerText)
			height = System.Int32.Parse(node.SelectSingleNode("dimensions/height").InnerText)
			
			results.Add(thumbnail, title, page, width, height, full);


	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;