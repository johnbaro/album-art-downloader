import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class Chartstats(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "Chartstats"
	Version as string:
		get: return "0.1"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		//Retrieve the search results page
		searchResultsHtml as string = GetPage("http://www.chartstats.com/search.php?q=" + EncodeUrl(artist + " " + album))
		
		matches = Regex("<a href=\"release\\.php\\?release=(?<id>[^\"]+)\">(?<title>[^<]+)<", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count

		for match as Match in matches:
			title = match.Groups["title"].Value
			id = match.Groups["id"].Value

			results.Add("http://www.chartstats.com/image/r${id}_100.jpg", title, "http://www.chartstats.com/release.php?release=${id}", -1, -1, id, CoverType.Front);
			

	def RetrieveFullSizeImage(id):
		artHtml as string = GetPage("http://www.chartstats.com/art.php?release=${id}")
		url = Regex("<img src=\"(?<url>images/artwork/[^\"]+)\"", RegexOptions.Singleline | RegexOptions.IgnoreCase).Match(artHtml).Groups["url"].Value

		return "http://www.chartstats.com/" + url