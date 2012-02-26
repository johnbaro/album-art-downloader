import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class Deejay(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "deejay.de"
	Version as string:
		get: return "0.1"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)
		query = EncodeUrl("""${artist}"" ""${album}""")

		//Retrieve the search results page
		searchResultsHtml as string = GetPage("http://www.deejay.de/content.php?param=" + query)
		
		matches = Regex("href=\"(?<info>[^\"]+)\"[^>]+><img alt=\"Cover\" class=\"cover\" src=\"/images/m/(?<img>[^.]+).jpg\".+?>(?<artist>[^<]+)</a>.+?>(?<album>[^<]+)</a>", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count * 2

		for match as Match in matches:
			info = "http://www.deejay.de/" + match.Groups["info"].Value
			img = match.Groups["img"].Value
			title = match.Groups["artist"].Value + " - " + match.Groups["album"].Value
			
			results.Add("http://www.deejay.de/images/s/" + img + ".jpg", title, info, -1, -1, "http://www.deejay.de/images/xl/" + img + ".jpg", CoverType.Front)
			results.Add("http://www.deejay.de/images/s/" + img + "b.jpg", title, info, -1, -1, "http://www.deejay.de/images/xl/" + img + "b.jpg", CoverType.Back)

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter