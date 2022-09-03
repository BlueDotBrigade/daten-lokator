namespace BlueDotBrigade.DatenLokator.TestsTools.Configuration
{
	using System;
	using BlueDotBrigade.DatenLokator.TestsTools.IO;

	internal class LokatorConfiguration
    {
	    private FileManager _fileManager;

		internal string DefaultFile { get; set; }

		internal FileManager FileManager 
		{
			get
			{
				return _fileManager;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(this.FileManager));
				}
				else
				{
					_fileManager = value;
				}
			}
		}
    }
}
