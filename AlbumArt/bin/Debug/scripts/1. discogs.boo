namespace CoverSources
import System
import System.Xml
import System.Drawing
import System.Text
import System.Text.RegularExpressions
import util

class Discogs:

	static ThumbSize = Size(150, 150)

	static SourceName as string:
		get: return "Discogs"

	static SourceCreator as string:
		get: return "Someone"

	static SourceVersion as string:
		get: return "0.1"

	static def GetThumbs(coverart,artist,album,size):
		query as string = artist + " " + album
		query.Replace(' ','+')
		obidResults = GetPage(String.Format("http://www.discogs.com/search?type=all&q={0}", EncodeUrl(query)))

		//Get obids
		obidRegex = Regex("<a href=\"/release/(?<obid>\\d+)\"><span[^>]+>[^-]+- (?:</?em>|(?<name>[^<-]+))+</span>", RegexOptions.Multiline)
		obidMatches = obidRegex.Matches(obidResults)
		coverart.SetCountEstimate(obidMatches.Count) //Probably more than this, as some releases might have multiple images

		for obidMatch as Match in obidMatches:
			//Construct the release name by joining up all the captures of the "name" group
			releaseNameBuilder = StringBuilder()
			for namePart in obidMatch.Groups["name"].Captures:
				releaseNameBuilder.Append(namePart)

			releaseName = releaseNameBuilder.ToString()

			//Get the image results
			imageResults = GetPage(String.Format("http://www.discogs.com/viewimages?what=R&obid={0}&showpending=1", obidMatch.Groups["obid"].Value))
			imageRegex = Regex("<img src=\"(?<url>http://www\\.discogs\\.com/image/R-\\d+-\\d+.jpe?g)\" width=\"(?<width>\\d+)\" height=\"(?<height>\\d+)\"")
			imageMatches = imageRegex.Matches(imageResults)
			for imageMatch as Match in imageMatches:

				large = System.Drawing.Bitmap.FromStream(GetPageStream(imageMatch.Groups["url"].Value))
				thumb = ResizeBitmap(large, ThumbSize.Width, ThumbSize.Height)
				large.Dispose()

				coverart.AddThumb(thumb, releaseName, Int32.Parse(imageMatch.Groups["width"].Value), Int32.Parse(imageMatch.Groups["height"].Value), imageMatch.Groups["url"].Value)

	static def GetResult(param):
		return param
