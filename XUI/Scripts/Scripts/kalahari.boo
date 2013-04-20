import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class Kalahari(AlbumArtDownloader.Scripts.IScript, ICategorised):
	Name as string:
		get: return "Kalahari"
	Version as string:
		get: return "0.6"
	Author as string:
		get: return "Alex Vallat"
	Category as string:
		get: return "South African"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		//Retrieve the search results json
		content = "{'queryString':'" + Base64("0|FreeText_Music_English|${artist} ${album} ||19738|0|1|25|||||||||||") + "', 'commandKey':'undefined','commandValue':'undefined','shopperId':'undefined','referrer':'/','sessionId':'undefined'}";
		request = System.Net.HttpWebRequest.Create("http://www.kalahari.com/common/searchservice.asmx/ExecuteSearch");
		request.Method = "POST";
		request.ContentType = "application/json; charset=UTF-8";
		request.Headers.Add("Accept-Encoding","")
		bytes = System.Text.Encoding.UTF8.GetBytes(content);
		request.ContentLength = bytes.Length;
		stream = request.GetRequestStream();
		stream.Write(bytes, 0, bytes.Length);
		stream.Close();
		response = request.GetResponse()
		streamresponse = response.GetResponseStream();
		searchResults = System.IO.StreamReader(streamresponse).ReadToEnd();

		//Very basic de-jsoning
		searchResults = Regex.Match(searchResults, "\"ProductHtml\":\"(.*?(?<!\\\\))\"").Groups[1].Value;
		searchResults = searchResults.Replace("\\u003e", ">").Replace("\\u003c", "<").Replace("\\\"", "\"");

		//Find album info
		matches = Regex("<a\\s[^>]*?href=\"(?<info>[^\"]+)\"[^>]+?title=\"(?<title>[^\"]+)\"[^>]*>\\s*<img\\s[^>]*?src=\"(?<image>[^\"]+?)0\\.jpg\"", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResults)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			image = match.Groups["image"].Value;

			results.Add(image + "1.jpg", System.Web.HttpUtility.HtmlDecode(match.Groups["title"].Value), "http://www.kalahari.com" + match.Groups["info"].Value, -1, -1, image + "2.jpg", CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;
		
	def Base64(value):
		return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value))