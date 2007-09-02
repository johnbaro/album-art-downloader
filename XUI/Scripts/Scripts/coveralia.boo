namespace CoverSources
import System
import System.Text
import System.Text.RegularExpressions
import util

class Coveralia:
	static SourceName as string:
		get: return "Coveralia"
	static SourceCreator as string:
		get: return "Alex Vallat"
	static SourceVersion as string:
		get: return "0.5"
	static def GetThumbs(coverart,artist,album):
		query as string = artist + " " + album
		query.Replace(' ','+')
		resultResults = GetPage(String.Format("http://www.coveralia.com/mostrar.php?bus={0}&bust=2", EncodeUrl(query)))
		
		//Get results
		resultRegex = Regex("<a href=\"(?<url>discos/[^\"]+)\" class=\"texto9\">", RegexOptions.Multiline)
		resultMatches = resultRegex.Matches(resultResults)
		coverart.EstimatedCount = resultMatches.Count * 3 //Estimate each result has front, back and CD images

		for resultMatch as Match in resultMatches:
			//Get the result page
			resultPage = GetPage(String.Format("http://www.coveralia.com/{0}", resultMatch.Groups["url"].Value))
			
			labelRegex = Regex("<span class=\"disco1\"><a[^>]+>(?<artist>[^<]+)</a>\\s*</span>\\s*<br>\\s*<span class=\"disco2\">(?<album>[^<]+)", RegexOptions.Multiline)
			labelMatch = labelRegex.Match(resultPage) //Expecting one match
			
			imagePageRegex = Regex("<a href=\"/caratulas/(?<imageName>[^\"]+)\"><img src=\"/audio/thumbs/(?<thumbID>[^\"]+)\"")
			imagePageMatches = imagePageRegex.Matches(resultPage)
			
			coverart.EstimatedCount += imagePageMatches.Count - 3 //Adjust estimated count based on number of matches found here
			
			for imagePageMatch as Match in imagePageMatches:
				//Find Full Size image
				fullSizeImagePage = GetPage(String.Format("http://www.coveralia.com/caratulas/{0}", imagePageMatch.Groups["imageName"].Value))
				
				//Width and Height in the html are not the actual width and height of the image, they are always around 500, so ignore them.
				fullSizeImageRegex = Regex("src=\"(?<url>http://www\\.coveralia\\.com/audio/[^\"]+)\"")
				fullSizeImageMatch = fullSizeImageRegex.Match(fullSizeImagePage) //Expecting only one match
				
				if fullSizeImageMatch.Success:
					coverart.Add(String.Format("http://www.coveralia.com/audio/thumbs/{0}", imagePageMatch.Groups["thumbID"].Value), labelMatch.Groups["artist"].Value + " - " + labelMatch.Groups["album"].Value, fullSizeImageMatch.Groups["url"].Value)
		
	static def GetResult(param):
		return param
