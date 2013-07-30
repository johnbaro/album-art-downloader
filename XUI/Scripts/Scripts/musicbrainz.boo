# refs: System.Web.Extensions
import System.Collections.Generic
import System.Web.Script.Serialization
import AlbumArtDownloader.Scripts
import util

class Musicbrainz(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "MusicBrainz"
	Author as string:
		get: return "Sebastian Hauser"
	Version as string:
		get: return "0.6"
	
	
	def Search(artist as string, album as string, results as IScriptResults):
		if(artist!= null and album!=null):			
			//striping isn't really necessary here, because musicbrainz handles those characters quite well
			//artist = StripCharacters("&.'\";:?!", artist)
			//album = StripCharacters("&.'\";:?!", album)
			
			//mbidUrl = GetMbidUrl("Portishead", "Dummy") //Test for album Portishead - Dummy
			mbidUrl = GetMbidUrl(artist, album)
			
			json = JavaScriptSerializer()
			
			try:
				mbidDoc = GetPage(mbidUrl)
				mbidResult = json.DeserializeObject(mbidDoc) as Dictionary[of string, object]

				results.EstimatedCount = mbidResult["release-list"]["count"]
				
				//results are sorted by score. get first score and discard all results where the score is less then 60% of that score.
				scoreThreshold = System.Convert.ToInt32(mbidResult["release-list"]["release"][0]["score"]) * 0.6
				
				for release as Dictionary[of string, object] in mbidResult["release-list"]["release"]:
					mbid = release["id"]
					mbidArtist = release["artist-credit"]["name-credit"][0]["artist"]["name"]
					mbidTitle = release["title"]
					mbidScore = System.Convert.ToInt32(release["score"])
					
					if mbidScore > scoreThreshold:

						try:
							#picUrl = GetCaaUrl(76df3287-6cda-33eb-8e9a-044b5e15ffdd) //Test for album Portishead - Dummy
							picUrl = GetCaaUrl(mbid)
							
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
					else:
						results.EstimatedCount--
			except e:
				return
		else:
			#both Parameter album and artist are necessary
			results.EstimatedCount = 0;
	
	
	def GetCaaUrl(mbid as string):
		caaBaseUrl = "http://coverartarchive.org/release/"
		return caaBaseUrl + mbid
	
	
	def GetMbidUrl(artist as string, album as string):
		encodedArtist = EncodeUrl(artist)
		encodedAlbum = EncodeUrl(album)
		mbidBaseUrl = "http://search.musicbrainz.org/ws/2/release/"
		if artist == "" and album == "":
			return "${mbidBaseUrl}?fmt=json&query="
		elif artist == "":
			return "${mbidBaseUrl}?fmt=json&query=release:${encodedAlbum}"
		elif album == "":
			return "${mbidBaseUrl}?fmt=json&query=artist:${encodedArtist}"
		else:
			return "${mbidBaseUrl}?fmt=json&query=release:${encodedAlbum} AND artist:${encodedArtist}"
	
	
	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;
