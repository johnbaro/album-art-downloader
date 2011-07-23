import AlbumArtDownloader.Scripts
import util

class iTunesGoogle(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "iTunes/Google"
	Version as string:
		get: return "0.3"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		searchResultsHtml as string = GetPage("http://www.google.com/search?filter=0&q=site%3Aitunes.apple.com%2F+" + EncodeUrl("\"" + artist + "\" \"download+" + album + "\""))
		
		matches = Regex("<a href=\"(?<url>http://itunes\\.apple\\.com/[^/]+/album/[^\"]+)\"", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			url = match.Groups["url"].Value
			
			// Now fetch the iTunes page
			albumResultHtml as string = GetPage(url)
			albumMatch = Regex("<img [^>]+?alt=\"(?<title>[^\"]+)\" class=\"artwork\" src=\"(?<image>[^\"]+?)170x170-75\\.jpg\"", RegexOptions.Singleline | RegexOptions.IgnoreCase).Match(albumResultHtml)

			imageUrlBase = albumMatch.Groups["image"].Value
			
			results.Add(imageUrlBase + "170x170-75.jpg", albumMatch.Groups["title"].Value, url, -1, -1, imageUrlBase + "jpg", CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter