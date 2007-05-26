using System;
using System.Collections.Generic;
using System.Text;

namespace AlbumArtDownloader
{
	/// <summary>
	/// Options available for when to auto download full size images
	/// </summary>
	public enum AutoDownloadFullSizeImages
	{
		Never,
		WhenSizeUnknown,
		Always
	}

	/// <summary>
	/// States that the art file search can be in for the browsers
	/// </summary>
	public enum ArtFileStatus
	{
		Unknown,
		Queued,
		Searching,
		Present,
		Missing
	}
}
