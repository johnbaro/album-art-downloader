namespace CoverSources
import System
import System.Xml
import System.Drawing
import System.Text.RegularExpressions
import util

class GoogleImage:
	static ThumbSize = Size(150, 150)
	static def GetPageSecret(url as string):
		request as System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(url)
		request.UserAgent="Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 2.0.50727)"
		response = request.GetResponse()
		return response.GetResponseStream()
	static SourceName as string:
		get: return "GoogleImage"
	static SourceVersion as string:
		get: return "0.2"
	static def GetThumbs(coverart,artist,album):
		query = artist+" "+album
		params = EncodeUrl(query)
		params.Replace('%20','+')
		textstream = GetPageSecret("http://images.google.com/images?q="+params)
		text = System.IO.StreamReader(textstream).ReadToEnd()
		r = Regex("""dyn\.Img\("([^"]*)","([^"]*)","([^"]*)","([^"]*)","([^"]*)","([^"]*)","([^"]*)","([^"]*)","([^"]*)","([^"]*)","([^"]*)","([^"]*)","([^"]*)","([^"]*)","([^"]*)"\)""")
		iterator = r.Matches(text)
		coverart.SetCountEstimate(iterator.Count)
		for result as Match in iterator:
			size = result.Groups[10].Value
			size = size.Remove(size.IndexOf("pixels"))

			large = System.Drawing.Bitmap.FromStream(GetPageStream("http://images.google.com/images?q=tbn:"+result.Groups[3].Value+result.Groups[4].Value))
			thumb = ResizeBitmap(large, ThumbSize.Width, ThumbSize.Height)
			large.Dispose()

			name = (result.Groups[7].Value.Replace("<b>","").Replace("</b>",""))

			Width = Int32.Parse(size.Remove(size.IndexOf(" x ")))
			Hight = Int32.Parse(size.Remove(1, size.IndexOf(" x ") + 3))
			if (Width >= 300):
				coverart.AddThumb(thumb,name,Width,Hight,result.Groups[4].Value)
	static def GetResult(param):
		return param

