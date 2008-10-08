namespace CoverSources
import System
import System.Drawing
import System.Text
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class JunoRecords:

	static SourceName as string:
		get: return "Juno Records"
	static SourceVersion as string:
		get: return "0.2"
	static SourceCreator as string:
		get: return "Marc Landis"

	static def GetThumbs(coverart,artist,album):
		query as string = artist + " " + album
		query = EncodeUrl(query)
		
		searchResults = GetPage("http://classic.juno.co.uk/search/?q=${query}&precision=any&column=all&genre_id=0000&released=&sdate=&edate=")

		//Get obids
		resultsRegex = Regex("<a href=\"/products/(?<ID>.*?.htm)", RegexOptions.Singleline)
		resultMatches = resultsRegex.Matches(searchResults)
		coverart.SetCountEstimate(resultMatches.Count * 2) //Estimate 2 covers per result. Results may vary.

		for resultMatch as Match in resultMatches:
			//Get the album page
			albumPageUrl = String.Format("http://classic.juno.co.uk/products/{0}", resultMatch.Groups["ID"].Value)
			albumPage = GetPage(albumPageUrl)

			//Get the title for that album
			titleRegex = Regex("<h3>(?<title>.*?)</h3>", RegexOptions.Singleline)
			title = titleRegex.Matches(albumPage)[0].Groups["title"].Value //Expecting only one match

			//Get all the images for the album
			imagesRegex = Regex("150/CS(?<fullSizeID>.*?)\\.jpg.*?alt=\"(?<imageName>.*?)\"", RegexOptions.Singleline)
			imageMatches = imagesRegex.Matches(albumPage)
			
			for imageMatch as Match in imageMatches:
				if(imageMatches.Count > 1):
					altText = imageMatch.Groups["imageName"].Value
					coverType = string2coverType(altText);
					imageTitle = "${title} - ${altText}"
				else:
					imageTitle = title
				fullSizeID = imageMatch.Groups["fullSizeID"].Value
				coverart.Add(
					"http://images.juno.co.uk/150/CS${fullSizeID}.jpg", #thumbnail
					imageTitle, #name
					albumPageUrl, #infoUri
					-1, #fullSizeImageWidth
					-1, #fullSizeImageHeight
					"http://images.juno.co.uk/full/CS${fullSizeID}-BIG.jpg", #fullSizeImageCallback
					coverType #coverType
					)
				
	static def GetResult(param):
		return param
		
	static def string2coverType(typeString as string):
		if(typeString.ToLower().Contains("front")):
			return CoverType.Front;
		elif(typeString.ToLower().Contains("back")):
			return CoverType.Back;
		elif(typeString.ToLower().Contains("inlay")):
			return CoverType.Inlay;
		elif(typeString.ToLower().Contains("cd")):
			return CoverType.CD;
		elif(typeString.ToLower().Contains("inside")):
			return CoverType.Inlay;
		else:
			return CoverType.Unknown;

