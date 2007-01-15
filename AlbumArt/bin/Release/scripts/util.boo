# refs: System.Web

namespace util

import System
import System.Math
import System.Drawing
import System.Drawing.Imaging
import System.Drawing.Drawing2D

def GetPageStream(url as string):
						request = System.Net.HttpWebRequest.Create(url)
						response = request.GetResponse()
						return response.GetResponseStream()
def GetPage(url as string):
						s=System.IO.StreamReader(GetPageStream(url))
						return s.ReadToEnd()
def EncodeUrl(url as string):
	return System.Web.HttpUtility.UrlEncode(url)
		
static def ResizeBitmap(source as Bitmap, MaxWidth as Int32, MaxHeight as Int32):
	sourceWidth = source.Width
	sourceHeight = source.Height
	sourceX = 0
	sourceY = 0
	destX = 0
	destY = 0 

	nPercent as Decimal = 0
	nPercentW as Decimal = 0
	nPercentH as Decimal = 0

	nPercentW = Decimal.Parse(MaxWidth.ToString()) / Decimal.Parse(sourceWidth.ToString())
	nPercentH = Decimal.Parse(MaxHeight.ToString()) / Decimal.Parse(sourceHeight.ToString())
	if(nPercentH < nPercentW):
		nPercent = nPercentH
		destX = System.Convert.ToInt16((MaxWidth - (sourceWidth * nPercent))/2)
	else:
		nPercent = nPercentW
		destY = System.Convert.ToInt16((MaxHeight - (sourceHeight * nPercent))/2)

	destWidth  = (sourceWidth * nPercent)
	destHeight = (sourceHeight * nPercent)

	bmPhoto = Bitmap(MaxWidth, MaxHeight, PixelFormat.Format24bppRgb)
	bmPhoto.SetResolution(96.0, 96.0)

	grPhoto = Graphics.FromImage(bmPhoto)
	grPhoto.Clear(Color.White)
	grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic

	grPhoto.DrawImage(source, Rectangle(destX,destY,destWidth,destHeight), Rectangle(sourceX,sourceY,sourceWidth,sourceHeight), GraphicsUnit.Pixel)

	grPhoto.Dispose()

	return bmPhoto
		
