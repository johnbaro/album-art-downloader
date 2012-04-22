import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class Take2(AlbumArtDownloader.Scripts.IScript, ICategorised):
	Name as string:
		get: return "Take2"
	Version as string:
		get: return "0.3"
	Author as string:
		get: return "Alex Vallat"
	Category as string:
		get: return "South African"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		//Retrieve the search results page
		searchResultsHtml as string = GetPage("http://www.takealot.com/search?type=5&qsearch=" + EncodeUrl(artist + " " + album))
		
		matches = Regex("<a href=\"(?<info>[^\"]+)\">\\s*<img src=\"(?<thumbnail>http://media(?<server>\\d?)\\.takealot\\.com/covers/(?<albumId>[^/]+)/cover-serp\\.jpg\\?(?<coverId>[^\"]+))\" alt=\"(?<title>[^\"]+)\"", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			results.Add(match.Groups["thumbnail"].Value, System.Web.HttpUtility.HtmlDecode(match.Groups["title"].Value), "http://www.takealot.com" + match.Groups["info"].Value, -1, -1, "http://media" + match.Groups["server"].Value + ".takealot.com/covers/" + match.Groups["albumId"].Value + "/cover-full.jpg?" + match.Groups["coverId"].Value, CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;
