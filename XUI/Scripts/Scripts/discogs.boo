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
		get: return "0.12"
	def Search(artist as string, album as string, results as IScriptResults):
		//artist = StripCharacters("&.'\";:?!", artist)
		//album = StripCharacters("&.'\";:?!", album)

		json = JavaScriptSerializer()

		resultsInfoJson = GetDiscogsPage("http://api.discogs.com/database/search?type=release&artist=${EncodeUrl(artist)}&release_title=${EncodeUrl(album)}")
		resultsInfo = json.Deserialize[of ResultsInfo](resultsInfoJson).results
		
		results.EstimatedCount = resultsInfo.Length;
		
		for result in resultsInfo:
			// Get the release info from api
			releaseInfoJson = GetDiscogsPage("http://api.discogs.com/release/" + result.id)
			releaseInfo = json.Deserialize[of ReleaseInfo](releaseInfoJson).resp.release
			
			results.EstimatedCount += releaseInfo.images.Length - 1
			for image in releaseInfo.images:
				coverType =  CoverType.Unknown
				if image.type == "primary":
					coverType = CoverType.Front

				results.Add(GetDiscogsStream(image.uri150), result.title, releaseInfo.uri, image.width, image.height, image.uri, coverType)

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
				public uri as String

				class Artist:
					public name as String

				class Image:
					public height as int
					public type as string
					public uri as string
					public uri150 as string
					public width as int

	class ResultsInfo:
		public results as (Result)
		class Result:
			public title as String
			public id as string