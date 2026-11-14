namespace BlueDotBrigade.DatenLokator.TestTools.IO
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.IO.Compression;

	/// <summary>
	/// Extracts the content of the supported archive formats.
	/// </summary>
	internal static class ArchiveExtractor
	{
		private static readonly IReadOnlyDictionary<string, Action<string, string>> Extractors =
			new Dictionary<string, Action<string, string>>(StringComparer.OrdinalIgnoreCase)
			{
				[".zip"] = (archivePath, targetDirectory) => ZipFile.ExtractToDirectory(archivePath, targetDirectory),
			};

		/// <summary>
		/// The file extensions, including the leading period, of the supported archive formats.
		/// </summary>
		public static IEnumerable<string> SupportedExtensions => Extractors.Keys;

		public static bool IsArchive(string filePath)
		{
			return Extractors.ContainsKey(Path.GetExtension(filePath));
		}

		/// <summary>
		/// Extracts the content of <paramref name="archivePath"/> into <paramref name="targetDirectory"/>.
		/// </summary>
		/// <remarks>
		/// If the extraction fails, the partially extracted directory is removed,
		/// so that the next test run does not mistake it for a complete extraction.
		/// </remarks>
		public static void Extract(string archivePath, string targetDirectory)
		{
			if (!Extractors.TryGetValue(Path.GetExtension(archivePath), out var extract))
			{
				throw new NotSupportedException(
					$"The archive format is not supported. SupportedExtensions=`{string.Join(", ", SupportedExtensions)}`, Path=`{archivePath}`");
			}

			try
			{
				Directory.CreateDirectory(targetDirectory);
				extract(archivePath, targetDirectory);
			}
			catch
			{
				if (Directory.Exists(targetDirectory))
				{
					Directory.Delete(targetDirectory, recursive: true);
				}

				throw;
			}
		}
	}
}
