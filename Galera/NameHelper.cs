using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Galera
{
	public static class NameHelper
	{
		public static Command WithHandler(this Command command, ICommandHandler handler)
		{
			command.Handler = handler;
			return command;
		}

		public static string Combine(string folderPath, string fileName, string extension)
		{
			Span<char> correctFileName = stackalloc char[fileName.Length];
			fileName.CopyTo(correctFileName);
			for (int i = 0; i < correctFileName.Length; i++)
				if (correctFileName[i] == '\\' || correctFileName[i] == '|')
					correctFileName[i] = '_';

			if (string.IsNullOrEmpty(folderPath))
				folderPath = ".\\";

			var endsInSeparator = Path.EndsInDirectorySeparator(folderPath);
			var additionalChar = endsInSeparator ? 0 : 1;

			Span<char> result = stackalloc char[folderPath.Length + correctFileName.Length + 1 + extension.Length + additionalChar];
			
			folderPath.CopyTo(result);
			if (!endsInSeparator)
				result[folderPath.Length] = '\\';

			var fileNameSlice = result.Slice(folderPath.Length + additionalChar);
			correctFileName.CopyTo(fileNameSlice);

			var extensionSlice = fileNameSlice.Slice(correctFileName.Length + 1);
			fileNameSlice[correctFileName.Length] = '.';
			extension.CopyTo(extensionSlice);

			return new string(result);
		}
	}
}
