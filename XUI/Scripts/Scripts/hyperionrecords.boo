import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class HyperionRecords(AlbumArtDownloader.Scripts.IScript, ICategorised):
	Name as string:
		get: return "Hyperion Records"
	Version as string:
		get: return "0.1"
	Author as string:
		get: return "Alex Vallat"
	Category as string:
		get: return "Classical"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		//Retrieve the search results
		searchResultsHtml as string = GetPage(GetPageStream("http://www.hyperion-records.co.uk/find.asp?f=" + EncodeUrl(artist + " " + album), null, true))
		
		matches = Regex("<a href=\"(?<url>[^\"]+)\"[^>]+>\\s*<img src=\"(?<thumb>[^\"]+)\" alt=\"Cover of '(?<title>[^\"]+)'", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			url = "http://www.hyperion-records.co.uk/" + match.Groups["url"].Value
			results.Add("http://www.hyperion-records.co.uk" + match.Groups["thumb"].Value, match.Groups["title"].Value, url, -1, -1, url, CoverType.Front)

	def RetrieveFullSizeImage(albumPageUrl):
		albumPageHtml as string = GetPage(albumPageUrl)

		fullSizeImageMatch = Regex("toggleLargeCover\\('(?<fullSize>[^']+)'\\)", RegexOptions.Singleline | RegexOptions.IgnoreCase).Match(albumPageHtml) //Expecting only one match
				
		if fullSizeImageMatch.Success:
			return "http://www.hyperion-records.co.uk" + fullSizeImageMatch.Groups["fullSize"].Value
		
		return null;