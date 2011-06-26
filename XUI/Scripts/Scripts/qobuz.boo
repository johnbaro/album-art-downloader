import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class Qobuz(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "Qobuz"
	Version as string:
		get: return "0.1"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		//Retrieve the search results page
		searchResultsHtml as string = GetPage("http://www.qobuz.com/recherche?i=boutique&q=" + EncodeUrl(artist + " " + album))
		
		matches = Regex("<a href=\"(?<url>[^\"]+)\"[^>]+>\\s+<img alt=\"(?<title>[^\"]+)\"[^>]+?rel=\"(?<id>(?<idPrefix>[^\"]{4})[^\"]+)\"", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count

		for match as Match in matches:
			infoUrl = match.Groups["url"].Value
			title = match.Groups["title"].Value
			id = match.Groups["id"].Value
			idPrefix = match.Groups["idPrefix"].Value
			urlBase = "http://static.qobuz.com/images/jaquettes/${idPrefix}/${id}"

			results.Add(urlBase + "_100.jpg", title, "http://www.qobuz.com" + infoUrl, 600, 600, urlBase + "_600.jpg", CoverType.Front)

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter