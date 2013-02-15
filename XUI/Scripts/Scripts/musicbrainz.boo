# refs: System.Web.Extensions
import System.Collections.Generic
import System.Web.Script.Serialization
import AlbumArtDownloader.Scripts
import util

class Musicbrainz(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "Musicbrainz"
	Author as string:
		get: return "Sebastian Hauser"
	Version as string:
		get: return "0.3"
	
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)
		
		if(artist!= null and album!=null):			
			mbidBaseUrl = "http://search.musicbrainz.org/ws/2/release/"
			
			#mbidUrl = "${mbidBaseUrl}?fmt=json&query=release:" + EncodeUrl("\"" + dummy + "\" AND artist:\"" + portishead + "\"")
			mbidUrl = "${mbidBaseUrl}?fmt=json&query=release:" + EncodeUrl("\"" + album + "\" AND artist:\"" + artist + "\"")
						
			picBaseUrl = "http://coverartarchive.org/release"
			
			scoreThreshold = 70
			
			json = JavaScriptSerializer()
			
			try:
				mbidDoc = GetPage(mbidUrl)
				mbidResult = json.DeserializeObject(mbidDoc) as Dictionary[of string, object]

				results.EstimatedCount = mbidResult["release-list"]["count"]
				
				for release as Dictionary[of string, object] in mbidResult["release-list"]["release"]:
					mbid = release["id"]
					mbidArtist = release["artist-credit"]["name-credit"][0]["artist"]["name"]
					mbidTitle = release["title"]
					mbidScore = System.Convert.ToInt32(release["score"])
					
					if mbidScore > scoreThreshold:
						try:
							#picUrl = "${picBaseUrl}/76df3287-6cda-33eb-8e9a-044b5e15ffdd"
							picUrl = "${picBaseUrl}/${mbid}"
														
							picDoc = GetPage(picUrl)
							picResult = json.DeserializeObject(picDoc) as Dictionary[of string, object]
							
							infoUrl = picResult["release"]

							for image as Dictionary[of string, object] in picResult["images"]:
								thumbnailUrl = image["thumbnails"]["small"]
								name = mbidArtist + " - " + mbidTitle
								pictureUrl = image["image"]
								if image["front"] == true:
									coverType = CoverType.Front
								elif image["back"] == true:
									coverType = CoverType.Back
								else:
									coverType = CoverType.Unknown
								results.Add(thumbnailUrl, name, infoUrl, -1, -1, pictureUrl, coverType)
						
						except e as System.Net.WebException:
							results.EstimatedCount--
					
			except e:
				return
		else:
			#both Parameter album and artist are necessary
			results.EstimatedCount = 0;
		
	
	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;