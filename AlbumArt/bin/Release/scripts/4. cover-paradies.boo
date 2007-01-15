namespace CoverSources
import System
import System.Drawing
import System.Text
import System.Text.RegularExpressions
import util
import Hacks

class CoverParadies:
	static ThumbSize = Size(150, 150)
	static DescriptiveImageTitles = true //Set this to False to prevent the type of image (Front, Back, CD, etc.) from being appended to the image titles

	static SourceName as string:
		get: return "Cover-Paradies"
	static SourceVersion as decimal:
		get: return 0.1
	static def GetThumbs(coverart,artist,album):
		query as string = artist + " " + album
		query.Replace(' ','+')
		
		searchResults = Post("http://www.cover-paradies.to/?Module=ExtendedSearch", String.Format("StartSearch=true&PagePos=0&SearchString={0}&StringMode=Wild&DisplayStyle=Text&HideDetails=Yes&PageLimit=1000&SektionID-2=Yes", EncodeUrl(query)))
		
		//Get results
		resultsRegex = Regex(";ID=(?<ID>\\d+)\">(?!\\s*<img)", RegexOptions.Singleline)
		resultMatches = resultsRegex.Matches(searchResults)
		coverart.SetCountEstimate(resultMatches.Count * 3) //Estimate 3 covers per result. Results may vary.
		
		for resultMatch as Match in resultMatches:
			//Get the album page
			albumPage = GetPage(String.Format("http://www.cover-paradies.to/?Module=ViewEntry&ID={0}", resultMatch.Groups["ID"].Value))
			
			//Get the title for that album
			titleRegex = Regex("<div class=\"BoxHeadline\"><img[^>]+>\\s*(?<title>[^<]+)<", RegexOptions.Singleline)
			title = titleRegex.Matches(albumPage)[0].Groups["title"].Value //Expecting only one match
			
			//Get all the images for the album
			imagesRegex = Regex("ID=(?<fullSizeID>\\d+)\"><img [^>]+? src=\"\\.(?<thumb>[^\"]+)\" [^>]+? class=\"EntryThumb\".+?<a [^>]+>(?<imageName>[^<]+)</a>.+?: (?<width>\\d+) x (?<height>\\d+) px", RegexOptions.Singleline)
			imageMatches = imagesRegex.Matches(albumPage)
			
			for imageMatch as Match in imageMatches:
				if(DescriptiveImageTitles and imageMatches.Count > 1):
					imageTitle = String.Format("{0} - {1}", title, imageMatch.Groups["imageName"].Value)
				else:
					imageTitle = title
					
				large = System.Drawing.Bitmap.FromStream(GetPageStream(String.Format("http://www.cover-paradies.to{0}", imageMatch.Groups["thumb"].Value)))
				thumb = ResizeBitmap(large, ThumbSize.Width, ThumbSize.Height)
				large.Dispose()

				coverart.AddThumb(thumb, imageTitle, Int32.Parse(imageMatch.Groups["width"].Value), Int32.Parse(imageMatch.Groups["height"].Value), imageMatch.Groups["fullSizeID"].Value)		

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
		url = String.Format("http://www.cover-paradies.to/res/exe/DLElement.exe.php?ID={0}", param)
		//If the cover downloader app is fixed to download unknown-length streams, then just return the url string from here directly.
		//For the moment, use a slimey hack to prevent an exception, and return that stream.
		//stream = HackStream(GetPageStream(url))
		return url

