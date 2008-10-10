# refs: System.Web
namespace util

import System.Text

def GetPageStream(url as string):
	return GetPageStream(url, null)
def GetPageStream(url as string, referer as string):
	request = System.Net.HttpWebRequest.Create(url) as System.Net.HttpWebRequest
	if not string.IsNullOrEmpty(referer):
		request.Referer = referer;
	response = request.GetResponse()
	return response.GetResponseStream()
def GetPage(url as string):
	s=System.IO.StreamReader(GetPageStream(url))
	return s.ReadToEnd()
def EncodeUrl(url as string):
	return EncodeUrl(url, null)

def  EncodeUrl(url as string, encoding as Encoding):
	url = url.Replace("&","%26").Replace("?","%3F")
	if not encoding == null:
		return System.Web.HttpUtility.UrlEncode(url, encoding)
	else:
		return System.Web.HttpUtility.UrlEncode(url)# Seems to be different from UrlEncode(url, Encoding.Default)

def GetPage(url as string, withFirefoxLikeHeader as bool, encodingName as string):
	if not string.IsNullOrEmpty(encodingName):
		encoding as Encoding = Encoding.GetEncoding(encodingName)
		s = System.IO.StreamReader(GetPageStream2(url, withFirefoxLikeHeader), encoding)
	else:
		s = System.IO.StreamReader(GetPageStream2(url, withFirefoxLikeHeader))
	return s.ReadToEnd()

def GetPageStream2(url as string, withFirefoxLikeHeader as bool):
	request as System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(url)
	if withFirefoxLikeHeader:
		request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8"
		request.Headers.Add("KEEP_ALIVE","300")
		request.Headers.Add("ACCEPT_CHARSET","ISO-8859-1,utf-8;q=0.7,*;q=0.7")
		#request.Headers.Add("ACCEPT_ENCODING","gzip,deflate")
		request.Headers.Add("ACCEPT_LANGUAGE","en-us,en;q=0.5")
		request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.9.0.3) Gecko/2008092417 Firefox/3.0.3 "
	response = request.GetResponse()
	return response.GetResponseStream()
	
/**
 * Does the same like util.UtilModule.GetPage,
 * but uses ISO/IEC 8859-1 (called iso-latin-1)
 * as encoding.
 * Many Pages uses this encoding for umlauts (äöüß),
 * when searching for "Die Ärzte" on such a site, using
 * this method is nessesacry
 */
def GetPageIsoLatin1(url as string):
	return GetPageIsoLatin1(url, false)

def GetPageIsoLatin1(url as string, withFirefoxLikeHeader as bool):
	return GetPage(url, withFirefoxLikeHeader, "iso-8859-1")

/**
 * Does the same like util.UtilModule.EncodeUrl,
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
	return EncodeUrl(url, encoding)

def GetPageStreamAsFirefox(url as string):
	return GetPageStream2(url, true)
	
def GetPageAsFirefox(url as string):
	return GetPage(url, true, null)