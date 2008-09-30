import System
import System.Text
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

/**
 * Searches for covers on darktown using Regex to 
 * navigate througth the sites.
 */
class darktown(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "Darktown"
	Version as string:
		get: return "0.1"
	Author as string:
		get: return "daju"
	
	def Search(artist as string, album as string, results as IScriptResults):
		toSearchFor = "${artist} ${album}"
		toSearchFor = toSearchFor.Trim() #delete unnessary whitespaces
		toSearchFor = EncodeUrlIsoLatin1(toSearchFor)# iso-latin-1 encoding is nessecary for searching  for "Die Ärzte"
		myQuery = "http://www.darktown.to/search.php?action=search&what=${toSearchFor}&category=audio"
		firstResultPage  = GetPageIsoLatin1(myQuery)
		
		resultRegex = Regex("'/coverdownload.php[^']*'", RegexOptions.Multiline)
		resultMatches = resultRegex.Matches(firstResultPage)
		results.EstimatedCount = resultMatches.Count
		for resultMatch as Match in resultMatches:
			currentRes = resultMatch.ToString().Replace('\'',' ').Trim()
			nextQuery = "http://www.darktown.to${currentRes}"
			secondResultPage  = GetPageIsoLatin1(nextQuery)
			
			imgRegex = Regex("\"http://img.darktown.to/getcover.php[^\"]*\"", RegexOptions.Multiline)
			imgMatches = imgRegex.Matches(secondResultPage)
			if (imgMatches.Count==1):
				imgUrl = imgMatches[0].ToString().Replace('"',' ').Trim()
						
				thumbRegex = Regex("\"http://img.darktown.to/thumbnail.php[^\"]*\"", RegexOptions.Multiline)
				thumbMatches = thumbRegex.Matches(secondResultPage)
				if (thumbMatches.Count==1):
					thumbUrl = 	thumbMatches[0].ToString().Replace('"',' ').Trim()
				else:
					thumbUrl = imgUrl
				
				nameRegex = Regex("<font size=4>[^<]*</font></b><br><b>[^<]*</b>", RegexOptions.Multiline)
				nameMatches = nameRegex.Matches(secondResultPage)
				if (nameMatches.Count==1):
					name = nameMatches[0].ToString().Remove(0,13).Replace("</font></b><br><b>"," - ").Replace("</b>","").Trim()
				
				typeRegex = Regex("<b>Typ des Covers:</b>[^<]*", RegexOptions.Multiline)
				typeMatches = typeRegex.Matches(secondResultPage)
				if (typeMatches.Count==1):
					type = typeMatches[0].ToString().Remove(0,22).Trim()
				
				kbRegex = Regex("<b>Dateigr[^:]*:</b>[^<]*", RegexOptions.Multiline)
				kbMatches = kbRegex.Matches(secondResultPage)
				if (kbMatches.Count==1):
					kb = kbMatches[0].ToString()
					kb = kb.Remove(0,kb.LastIndexOf('>')+1).Trim()
					
				results.Add(thumbUrl, "${name} - ${type} - ${kb}", nextQuery, -1, -1, imgUrl);
				
		
		
	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;
	
	/**
	 * Does the same like util.UtilModul.GetPage,
	 * but uses ISO/IEC 8859-1 (called iso-latin-1)
	 * as encoding.
	 * Many Pages uses this encoding for umlauts (äöüß),
	 * when searching for "Die Ärzte" on such a site, using
	 * this method is nessesacry
	 */
	def GetPageIsoLatin1(url as string):
		encoding as Encoding = Encoding.GetEncoding("iso-8859-1")
		s=System.IO.StreamReader(GetPageStream(url),encoding)
		return s.ReadToEnd()
	
	/**
	 * Does the same like util.UtilModul.EncodeUrl,
	 * but uses ISO/IEC 8859-1 (called iso-latin-1)
	 * as encoding.
	 * EncodeUrl uses utf-8 per default. So encoding
	 * "Die Ärzte" will result in "Die%20%c3%84rzte".
	 * Some websites have a problem with that encoding.
	 * These sites uses iso-latin-1 as encoding.
	 * Using this methode to encode "Die Ärzte" will 
	 * result in "Die%20%c4rzte"
	 */
	def EncodeUrlIsoLatin1(url as string):
		encoding as Encoding = Encoding.GetEncoding("iso-8859-1")#iso-latin-1
		return System.Web.HttpUtility.UrlEncode(url,encoding)
