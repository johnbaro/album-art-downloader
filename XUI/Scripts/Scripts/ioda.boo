import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class Ioda(AlbumArtDownloader.Scripts.IScript, ICategorised):
	Name as string:
		get: return "IODA"
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
		searchResultsHtml as string = GetPage("http://www.iodalliance.com/catalog?type=release&query=" + EncodeUrl("\"${artist}\" \"${album}\""))
		
		matches = Regex("\"artist-name\">(?<artist>[^<]+)</span>\\s*<br\\s*/>\\s*<a href=\"(?<url>/album/[^/]+/(?<id>\\d+))\">(?<album>[^<]+)", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			id = match.Groups["id"].Value;
			title = match.Groups["artist"].Value + " - " + match.Groups["album"].Value

			results.Add("http://image.iodalliance.com/release/thumbs_150/${id}-72.jpg", title, "http://www.iodalliance.com" + match.Groups["url"].Value, -1, -1, "http://image.iodalliance.com/release/300dpi/${id}-300.jpg", CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;