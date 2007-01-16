namespace CoverSources
import System
import System.Drawing
import System.Text
import System.Text.RegularExpressions
import util

class CoverIsland:
	static SourceName as string:
		get: return "CoverIsland"
	static SourceCreator as string:
		get: return "Alex Vallat"
	static SourceVersion as decimal:
		get: return 0.2
	static def GetThumbs(coverart,artist,album,size):
		query as string = artist + " " + album
		query.Replace(' ','+')
		searchResults = GetPage(String.Format("http://www.coverforum.net/modules.php?name=CoverSearch&tipo=audio&keywords={0}", EncodeUrl(query)))
		System.Diagnostics.Debug.WriteLine(searchResults)
		
		//Get results
		resultsRegex = Regex("<a href=\"http\\://www\\.coverisland\\.net/copertine/Audio/.\\.asp\">(?<title>.+?)(?<type>-(?:front|back|cd|cd2|inlay|inside))", RegexOptions.Multiline)
		resultMatches = resultsRegex.Matches(searchResults)
		coverart.SetCountEstimate(resultMatches.Count)
		
		for resultMatch as Match in resultMatches:
			//Get the image result page (try segno 0 and 1, it could be either. Quicker to try both than to get another page and parse it to find out which
			for segno in range(2):
				imageResult = Post("http://www.coverisland.net/copertine/down.asp", String.Format("tipologia=Audio&title={0}&type={1}&segno={2}", resultMatch.Groups["title"].Value, resultMatch.Groups["type"], segno))
				
				imageRegex = Regex("'(?<image>http\\://www\\.coverforum\\.net/view\\.php\\?[^']+)'", RegexOptions.Multiline)
				imageMatches = imageRegex.Matches(imageResult)
				
				for imageMatch as Match in imageMatches: //Only expecting one, really.
					url = imageMatch.Groups["image"].Value
					request = System.Net.HttpWebRequest.Create(url)
					response = request.GetResponse()
					if response.ContentType.StartsWith("image/"):
						coverart.AddThumb(response.GetResponseStream(), resultMatch.Groups["title"].Value, -1, -1, null)

	static def Post(url as String, content as String):
		request = System.Net.HttpWebRequest.Create(url)
		request.Method="POST"
		request.ContentType = "application/x-www-form-urlencoded"
		bytes = System.Text.UTF8Encoding().GetBytes(content)
		request.ContentLength = bytes.Length
		stream = request.GetRequestStream()
		stream.Write(bytes,0,bytes.Length)
		stream.Close()
		streamresponse = request.GetResponse().GetResponseStream()
		return System.IO.StreamReader(streamresponse).ReadToEnd()
			
	static def GetResult(param):
		return param