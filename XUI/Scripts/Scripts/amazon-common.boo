import System.Text
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

//Inheritors should override the Suffix property to return a valid amazon suffix (like com, co.uk, de, etc...).
abstract class Amazon(AlbumArtDownloader.Scripts.IScript):
	virtual Name as string:
		get: return "Amazon (.${Suffix})"
	Version as string:
		get: return "0.8s"
	Author as string:
		get: return "Alex Vallat"
	abstract protected Suffix as string:
		get: pass
	virtual protected CountryCode as string:
		get: return "01"
	virtual protected SearchIndex as string: //Deprectated, ignored.
		get: return "" 
	virtual protected def GetUrl(artist as string, album as string) as string:
		return "http://www.amazon.${Suffix}/gp/search/ref=sr_adv_m_pop/?search-alias=popular&field-artist=${EncodeUrlIsoLatin1(artist)}&field-title=${EncodeUrlIsoLatin1(album)}&sort=relevancerank"
	virtual protected PageEncoding as Encoding:
		get: return Encoding.GetEncoding("iso-8859-1")
	
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)
		
		url = GetUrl(artist, album)
		resultsPage = GetPage(GetPageStream(url, null, true), PageEncoding)
		
		resultsRegex = Regex("<div\\s[^>]*class\\s*=\\s*\"title\"[^>]*>\\s*<a\\s[^>]*href\\s*=\\s*\"(?<url>[^\"]+?/dp/(?<id>[^/]+)/)[^>]+>\\s*(?<title>.*?)</a>(?:\\s*<span\\s[^>]*class=\"ptBrand\"[^>]*>(?:[^<]*<a\\s[^>]*>)?\\s*(?:by |von |de |di )?(?<artist>[^<]+))?", RegexOptions.Singleline | RegexOptions.IgnoreCase)
		resultsMatches = resultsRegex.Matches(resultsPage)
		
		results.EstimatedCount = resultsMatches.Count
		
		for resultsMatch as Match in resultsMatches:
			id = resultsMatch.Groups["id"].Value
			url = resultsMatch.Groups["url"].Value
			title = System.Web.HttpUtility.HtmlDecode(resultsMatch.Groups["title"].Value)
			artist = System.Web.HttpUtility.HtmlDecode(resultsMatch.Groups["artist"].Value)
			imageBase = "http://ecx.images-amazon.com/images/P/${id}.${CountryCode}."

			thumbnail = TryGetImageStream(imageBase + "_THUMB_")

			results.Add(thumbnail, "${artist} - ${title}", url, -1, -1, imageBase, CoverType.Front)

	def RetrieveFullSizeImage(imageBase):
		imageStream = TryGetImageStream(imageBase + "_SCRM_")
		if imageStream != null:
			return imageStream

		//Fall back on Large size
		return TryGetImageStream(imageBase + "_SCL_")

	def TryGetImageStream(url):
		request as System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(url)
		try:
			response = request.GetResponse()
			if response.ContentLength > 43:
				return response.GetResponseStream()
			
			response.Close()
			return null
		except e as System.Net.WebException:
			return null
	