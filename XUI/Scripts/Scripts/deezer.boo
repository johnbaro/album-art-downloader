# refs: System.Web.Extensions

import System
import System.Text.RegularExpressions
import System.Web.Script.Serialization

import util

class Deezer(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "Deezer"
	Author as string:
		get: return "Alex Vallat"
	Version as string:
		get: return "0.2"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		jsonSearchResults = GetPage("http://api-v3.deezer.com/1.0/search/album/?q=" + EncodeUrl(artist + " " + album))
			
		json = JavaScriptSerializer()
		searchResults = json.Deserialize[of SearchResults](jsonSearchResults).search

		if searchResults.total_results > 0:
			results.EstimatedCount = searchResults.total_results

			imageIdRegex = Regex("cover/(?<id>[^/]+)/", RegexOptions.Singleline | RegexOptions.IgnoreCase)
		
			for album in searchResults.albums.album:
				title = album.artist.name + " - " + album.name
				match = imageIdRegex.Match(album.image)
				results.Add(album.image, title, album.url, -1, -1, match.Groups["id"].Value, CoverType.Front)

	def RetrieveFullSizeImage(id):
		return "http://cdn-images.deezer.com/images/cover/${id}/0x0-000000-100-0-0.png";

	class SearchResults:
		public search as Search
		class Search:
			public total_results as int
			public albums as Albums

			class Albums:
				public album as (Album)

				class Album:
					public artist as Artist
					public image as String
					public name as String
					public url as String

					class Artist:
						public name as String