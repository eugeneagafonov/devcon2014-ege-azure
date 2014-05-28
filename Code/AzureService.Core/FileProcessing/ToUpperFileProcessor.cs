using System.IO;
using System.Threading.Tasks;

namespace AzureService.Core.FileProcessing
{
	public class ToUpperFileProcessor : IFileProcessor
	{
		public string Operation
		{
			get { return "ToUpper"; }
		}

		public async Task ProcessFileAsync(Stream sourceFileStream, Stream destinationFileStream)
		{
			using (var reader = new StreamReader(sourceFileStream))
			{
				var writer = new StreamWriter(destinationFileStream);
				while (!reader.EndOfStream)
				{
					string line = await reader.ReadLineAsync();
					await writer.WriteLineAsync(line.ToUpper());
				}
				await writer.FlushAsync();
			}
		}
	}
}