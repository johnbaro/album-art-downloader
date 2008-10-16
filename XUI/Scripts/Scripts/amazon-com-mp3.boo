class AmazonComMp3(Amazon):
	override Name as string:
		get: return "Amazon mp3 (.com)"
	override protected Suffix as string:
		get: return "com"
	override protected SearchIndex as string:
		get: return "MP3Downloads"