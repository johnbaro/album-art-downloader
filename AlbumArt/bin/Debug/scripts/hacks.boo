//This class contains hacks that are not cover download source scripts, but are used by them

namespace Hacks
import System
import System.IO

class HackStream(Stream):
	mInternalStream as Stream

	def constructor(stream as Stream):
		mInternalStream = stream
	
//This is the hack.	
	override Length as Int64:
		get:
			if not CanSeek: //Avoid throwing an exception if the stream can't seek
				return 0
			return mInternalStream.Length
			 
			
//Everything past here is just pass-through to the original stream.
	
	override CanRead as bool:
		get:
			return mInternalStream.CanRead

	override CanSeek as bool:
		get:
			return mInternalStream.CanSeek

	override CanWrite as bool:
		get:
			return mInternalStream.CanWrite

	override def Flush():
		mInternalStream.Flush()

	override Position as Int64:
		get:
			return mInternalStream.Position
		set:
			mInternalStream.Position = value

	override def Read(buffer as (byte), offset as Int32, count as Int32):
		return mInternalStream.Read(buffer, offset, count)

	override def Seek(offset as Int64, origin as SeekOrigin):
		return mInternalStream.Seek(offset, origin)

	override def SetLength(value as Int64):
		mInternalStream.SetLength(value)

	override def Write(buffer as (byte), offset as Int32, count as Int32):
		mInternalStream.Write(buffer, offset, count)
