# refs: System.Web.Extensions
import System.Collections.Generic
import System.Web.Script.Serialization
import AlbumArtDownloader.Scripts
import util

class iTunes(AlbumArtDownloader.Scripts.IScript):
	virtual Name as string:
		get: 
			name = "iTunes"
			if not System.String.IsNullOrEmpty(CountryName):
				name += " (${CountryName})"
			return name
	Version as string:
		get: return "0.2"
	Author as string:
		get: return "Alex Vallat"
	virtual protected CountryName as string:
		get: return null
	virtual protected CountryCode as string:
		get: return "US"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)
		
		searchResultsJson as string = GetPage("http://itunes.apple.com/search?entity=album&country=${CountryCode}&term=" + EncodeUrl("\"" + artist + "\" \"" + album + "\""))

		json = JavaScriptSerializer()
		searchResults = json.DeserializeObject(searchResultsJson) as Dictionary[of string, object]
		
		results.EstimatedCount = searchResults["resultCount"]
		
		for result as Dictionary[of string, object] in searchResults["results"]:
			title = result["artistName"] + " - " + result["collectionName"]
			url = result["collectionViewUrl"]
			imageUrlBase = result["artworkUrl100"]
			// Remove size from image to get base
			imageUrlBase = imageUrlBase.Substring(0, imageUrlBase.Length - "100x100-75.jpg".Length)

			results.Add(imageUrlBase + "170x170-75.jpg", title, url, -1, -1, imageUrlBase, CoverType.Front);

	def RetrieveFullSizeImage(imageUrlBase):
		imageStream = TryGetImageStream(imageUrlBase + "jpg")

		if imageStream != null:
			return imageStream
		else:
			// Couldn't find full size .jpg, try .tif
			imageStream = TryGetImageStream(imageUrlBase + "tif")

			if imageStream != null:
				return imageStream
			else:
				// Couldn't find full size .jpg or .tif, fall back on 600x600
				return TryGetImageStream(imageUrlBase + "600x600-75.jpg")

	def TryGetImageStream(url):
		request as System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(url)
		try:
			response = request.GetResponse()
			return response.GetResponseStream()
		except e as System.Net.WebException:
			return null