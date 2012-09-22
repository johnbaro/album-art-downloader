# refs: System.Web.Extensions

import System.Text
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class AmazonAudiobooksCom(AlbumArtDownloader.Scripts.IScript, ICategorised):
	Name as string:
		get: return "Amazon Audiobooks (.com)"
	Category as string:
		get: return "Audiobooks"
	Version as string:
		get: return "0.1"
	Author as string:
		get: return "Alex Vallat"

	def Search(artist as string, album as string, results as IScriptResults):

		PageEncoding = Encoding.GetEncoding("iso-8859-1")

		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)
																	   
		url = "http://www.amazon.com/gp/search?search-alias=stripbooks&field-author=${EncodeUrl(artist, PageEncoding)}&field-title=${EncodeUrl(album, PageEncoding)}&sort=relevancerank&field-feature_browse-bin=618075011"
		resultsPage = GetPage(GetPageStream(url, null, true), PageEncoding)
		
		resultsRegex = Regex("class\\s*=\\s*\"image\"\\s*>\\s*<a[^>]+?href\\s*=\\s*\"(?<url>[^\"]+?/dp/(?<id>[^/]+)/)[^>]*?><img [^>]+src=\"(?<thumbnail>(?<full>http://ecx\\.images-amazon\\.com/images/[^.]+)\\.[^\"]+)\"[^>]+>.+?<a class\\s*=\\s*\"title\"\\s[^>]+>(?<title>.*?)</a>(?:\\s*<span\\s[^>]*>(?:[^<]*<a\\s[^>]*>)?\\s*(?:by |von |de |di )?(?<artist>[^<]+))?", RegexOptions.Singleline | RegexOptions.IgnoreCase)
		resultsMatches = resultsRegex.Matches(resultsPage)
		
		results.EstimatedCount = resultsMatches.Count
		
		for resultsMatch as Match in resultsMatches:
			url = resultsMatch.Groups["url"].Value
			title = System.Web.HttpUtility.HtmlDecode(resultsMatch.Groups["title"].Value)
			artist = System.Web.HttpUtility.HtmlDecode(resultsMatch.Groups["artist"].Value)
			full = resultsMatch.Groups["full"].Value + ".jpg"
			//thumbnail = resultsMatch.Groups["thumbnail"].Value
			thumbnail = resultsMatch.Groups["full"].Value + "._SL160_.jpg" // Ignore the thumbnail as provided, as it will have "Look inside" watermark on it.

			results.Add(thumbnail, "${artist} - ${title}", url, -1, -1, full, CoverType.Front)

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter