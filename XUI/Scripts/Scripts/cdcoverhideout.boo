import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class CDCoverHideout(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "CDCoverHideout"
	Version as string:
		get: return "0.1"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
	
		imagesHtml = Post("http://cdcoverhideout.com/covers/thumbnails.php", "search=" + EncodeUrl(artist + " " + album) + "&category=2&album=search&title=1&caption=1&keywords=1&owner_name=1&filename=1&type=AND")

		matches = Regex("<img src=\"(?<thumbURI>[^\"]+?thumb_[^\"]+)\"[^>]+alt=\"(?<name>[^.(\"]+(?:\\((?<type>[^)]+)\\))?)[^>]+Dimensions=(?<width>\\d+)x(?<height>\\d+)", RegexOptions.IgnoreCase).Matches(imagesHtml)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			name = match.Groups["name"].Value
			thumbURI = "http://www.cdcoverhideout.com/covers/" + match.Groups["thumbURI"].Value
			fullSize = thumbURI.Replace("thumb_", "")
			type = GetCoverType(match.Groups["type"].Value);
			width = System.Int32.Parse(match.Groups["width"].Value)
			height = System.Int32.Parse(match.Groups["height"].Value)
			
			results.Add(thumbURI, name, null, width, height, fullSize, type);


	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter
		
	static def GetCoverType(typeName as string):
		if(typeName == "Front"):
			return CoverType.Front;
		elif(typeName == "Back"):
			return CoverType.Back;
		elif(typeName == "Inside"):
			return CoverType.Inlay;
		elif(typeName.StartsWith("Cd")):
			return CoverType.CD;
		else:
			return CoverType.Unknown;
		
	static def Post(url as string, content as string):
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