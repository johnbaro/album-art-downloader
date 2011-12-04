# refs: System.Web.Extensions

import System
import System.Net
import System.Text.RegularExpressions
import System.Web.Script.Serialization

import util

class Discogs(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "Discogs"
	Author as string:
		get: return "Alex Vallat"
	Version as string:
		get: return "0.11"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		obidResults = GetDiscogsPage("http://www.discogs.com/advanced_search?artist=${EncodeUrl(artist)}&release_title=${EncodeUrl(album)}")
			
		//Get obids
		obidRegex = Regex("<div class=\"thumb\">\\s*<a href=\"(?<url>/[^/]+/release/(?<obid>\\d+))\">", RegexOptions.Singleline | RegexOptions.IgnoreCase)
		obidMatches = obidRegex.Matches(obidResults)
		results.EstimatedCount = obidMatches.Count //Probably more than this, as some releases might have multiple images

		json = JavaScriptSerializer()
		
		for obidMatch as Match in obidMatches:
			// Get the release info from api
			url = "http://www.discogs.com" + obidMatch.Groups["url"].Value
			obid = obidMatch.Groups["obid"].Value
			releaseInfoJson = GetDiscogsPage("http://api.discogs.com/release/" + obid)
			releaseInfo = json.Deserialize[of ReleaseInfo](releaseInfoJson).resp.release
			
			title = releaseInfo.artists[0].name + " - " + releaseInfo.title
			
			results.EstimatedCount += releaseInfo.images.Length - 1
			for image in releaseInfo.images:
				coverType =  CoverType.Unknown
				if image.type == "primary":
					coverType = CoverType.Front

				results.Add(GetDiscogsStream(image.uri150), title, url, image.width, image.height, image.uri, coverType)

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return GetDiscogsStream(fullSizeCallbackParameter);

	def GetDiscogsPage(url):
		stream = GetDiscogsStream(url)
		try:
			return GetPage(stream)
		ensure:
			stream.Close()

	def GetDiscogsStream(url):
		request = WebRequest.Create(url) as HttpWebRequest
		request.UserAgent = "AAD:Discogs/" + Version
		request.AutomaticDecompression = DecompressionMethods.GZip
		return request.GetResponse().GetResponseStream()
		
	class ReleaseInfo:
		public resp as Resp
		class Resp:
			public release as Release

			class Release:
				public artists as (Artist)
				public images as (Image)
				public title as String

				class Artist:
					public name as String

				class Image:
					public height as int
					public type as string
					public uri as string
					public uri150 as string
					public width as int