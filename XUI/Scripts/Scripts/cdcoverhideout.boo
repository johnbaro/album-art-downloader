import System.Text.RegularExpressions
import System.Net
import AlbumArtDownloader.Scripts
import util

class CDCoverHideout(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "CDCoverHideout"
	Version as string:
		get: return "0.2"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):

		cookies = CookieContainer()
		imagesHtml = Post("http://cdcoverhideout.com/search.htm", "search_keywords=" + EncodeUrl(artist + " " + album) + "&search_user=&search_terms=all&cat_id=1&search_fields=all", cookies)

		matches = Regex("<img src=\"\\./(?<thumbURI>data/thumbnails/[^\"]+)\".+?alt=\"(?<name>[^.(\"]+) (?:\\((?<type>[^)]+)\\)).+?href=\"\\./(?<fullURI>download.php\\?image_id=[^\"]+)\".+?Dimensions:.+?\"top\">(?<width>\\d+)x(?<height>\\d+) px", RegexOptions.IgnoreCase | RegexOptions.Singleline).Matches(imagesHtml)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			name = match.Groups["name"].Value
			thumbURI = "http://cdcoverhideout.com/" + match.Groups["thumbURI"].Value
			fullSize = "http://cdcoverhideout.com/" + match.Groups["fullURI"].Value
			type = GetCoverType(match.Groups["type"].Value);
			width = System.Int32.Parse(match.Groups["width"].Value)
			height = System.Int32.Parse(match.Groups["height"].Value)
			
			resultInfo = ResultInfo()
			resultInfo.URI = fullSize
			resultInfo.Cookies = cookies
			
			results.Add(thumbURI, name, null, width, height, resultInfo, type);

	def RetrieveFullSizeImage(resultInfo):
		request = System.Net.HttpWebRequest.Create(resultInfo.URI) as System.Net.HttpWebRequest
		request.CookieContainer = resultInfo.Cookies
		return request.GetResponse().GetResponseStream()
		
	static def GetCoverType(typeName as string):
		if(typeName == "Front"):
			return CoverType.Front;
		elif(typeName == "Back"):
			return CoverType.Back;
		elif(typeName == "Inside" or typeName == "Inlay"):
			return CoverType.Inlay;
		elif(typeName.StartsWith("CD")):
			return CoverType.CD;
		else:
			return CoverType.Unknown;
		
	static def Post(url as string, content as string, cookies as CookieContainer):
		request = System.Net.HttpWebRequest.Create(url) as System.Net.HttpWebRequest
		request.CookieContainer = cookies
		request.Method="POST"
		request.ContentType = "application/x-www-form-urlencoded"
		bytes = System.Text.UTF8Encoding().GetBytes(content)
		request.ContentLength = bytes.Length
		stream = request.GetRequestStream()
		stream.Write(bytes,0,bytes.Length)
		stream.Close()
		streamresponse = request.GetResponse().GetResponseStream()
		return System.IO.StreamReader(streamresponse).ReadToEnd()

class ResultInfo:
	public URI as string
	public Cookies as CookieContainer		