# refs: System.Web
namespace util

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
