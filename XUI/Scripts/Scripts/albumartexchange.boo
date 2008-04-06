import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class AlbumArtExchange(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "Album Art Exchange"
	Version as string:
		get: return "0.1"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
		albumResults = GetPage("http://www.albumartexchange.com/covers.php?q=" + EncodeUrl(artist + " " + album))

		albumMatches = @/(?i)scale_image\.php\?size=\d+&amp;src=(?<image>[^"]+)".+?alt="(?<title>.+?) \\ .+?(?<width>\d+)x(?<height>\d+)/.Matches(albumResults)
		results.EstimatedCount = albumMatches.Count
		
		for albumMatch as Match in albumMatches:
			imageUrl as string = albumMatch.Groups["image"].Value
			results.AddThumb("http://www.albumartexchange.com/phputil/scale_image.php?size=150&src=" + imageUrl, albumMatch.Groups["title"].Value, System.Int32.Parse(albumMatch.Groups["width"].Value), System.Int32.Parse(albumMatch.Groups["height"].Value), imageUrl);


	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return "http://www.albumartexchange.com" + System.Web.HttpUtility.UrlDecode(fullSizeCallbackParameter)

