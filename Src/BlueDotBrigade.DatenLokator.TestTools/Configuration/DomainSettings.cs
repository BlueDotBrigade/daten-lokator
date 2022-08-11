namespace BlueDotBrigade.DatenLokator.TestsTools.Configuration
{
	using System;
	using BlueDotBrigade.DatenLokator.TestsTools.IO;

	internal class DomainSettings
    {
	    private FileManager _fileManager;
	    internal bool IsSetup { get; set; } = false;

		internal string DefaultFile { get; set; }

		internal FileManager FileManager
		{
			get
			{
				if (this.IsSetup)
				{
					return _fileManager;
				}
				else
				{
					throw new InvalidOperationException("The DatenLokator environment has not been initialized. Hint: Call Domain.Setup()");
				}
			}
			set => _fileManager = value;
		}
    }
}
