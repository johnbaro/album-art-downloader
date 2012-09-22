import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class Ioda(AlbumArtDownloader.Scripts.IScript, ICategorised):
	Name as string:
		get: return "The Orchard"
	Version as string:
		get: return "0.1"
	Author as string:
		get: return "Alex Vallat"
	Category as string:
		get: return "Independent"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		//Retrieve the search results
		searchResultsHtml as string = GetPage("http://www.theorchard.com/search_catalog/list?artist=" + EncodeUrl(artist) + "&release=" + EncodeUrl(album) + "&type=music&redirected=1")
		
		matches = Regex("<a href=\"(?<url>http://www\\.theorchard\\.com/release/(?<id>\\d+)[^\"]+)\">.+?class=\"name\">(?<artist>[^<]+)<.+?class=\"name\">(?<album>[^<]+)<", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			id = match.Groups["id"].Value;
			title = match.Groups["artist"].Value + " - " + match.Groups["album"].Value

			results.Add("http://www.theorchard.com/images/coverart/c_${id}.jpg", title, match.Groups["url"].Value, -1, -1, "http://www.theorchard.com/images/lg_coverart/${id}.jpg", CoverType.Front)

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;