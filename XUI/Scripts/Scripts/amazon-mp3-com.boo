# refs: System.Web.Extensions

import System.Text
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class AmazonMp3Com(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "Amazon MP3 (.com)"
	Version as string:
		get: return "0.1"
	Author as string:
		get: return "Alex Vallat"

	def Search(artist as string, album as string, results as IScriptResults):

		PageEncoding = Encoding.GetEncoding("iso-8859-1")

		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)
		
		url = "http://www.amazon.com/gp/search?search-alias=digital-music&field-author=${EncodeUrl(artist, PageEncoding)}&field-title=${EncodeUrl(album, PageEncoding)}&sort=relevancerank"
		resultsPage = GetPage(GetPageStream(url, null, true), PageEncoding)
		
		resultsRegex = Regex("<a[^>]+?href\\s*=\\s*\"(?<url>[^\"]+?/dp/(?<id>[^/]+)/)[^>]*?class\\s*=\\s*\"image-title\"[^>]*><img src=\"(?<thumbnail>(?<full>http://ecx\\.images-amazon\\.com/images/[^.]+)\\.[^\"]+)\"[^>]+>.+?<span\\s[^>]+>(?<title>.*?)</span></a>(?:\\s*<span\\s[^>]*class=\"byline\"[^>]*>(?:[^<]*<a\\s[^>]*>)?\\s*(?:by |von |de |di )?(?<artist>[^<]+))?", RegexOptions.Singleline | RegexOptions.IgnoreCase)
		resultsMatches = resultsRegex.Matches(resultsPage)
		
		results.EstimatedCount = resultsMatches.Count
		
		for resultsMatch as Match in resultsMatches:
			url = resultsMatch.Groups["url"].Value
			title = System.Web.HttpUtility.HtmlDecode(resultsMatch.Groups["title"].Value)
			artist = System.Web.HttpUtility.HtmlDecode(resultsMatch.Groups["artist"].Value)
			thumbnail = resultsMatch.Groups["thumbnail"].Value
			full = resultsMatch.Groups["full"].Value + ".jpg"

			results.Add(thumbnail, "${artist} - ${title}", url, -1, -1, full, CoverType.Front)

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter