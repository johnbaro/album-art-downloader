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
	return System.Web.HttpUtility.UrlEncode(url.Replace("&","%26").Replace("?","%3F"));
	
/**
 * Does the same like util.UtilModule.GetPage,
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
	return System.Web.HttpUtility.UrlEncode(url,encoding)
