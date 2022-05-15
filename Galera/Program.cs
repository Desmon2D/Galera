using System.Diagnostics;
using System.Reflection;
using System.CommandLine;
using Galera;
using Galera.Enums;

var cmd = new RootCommand($"Galera v0.2")
{
	BuildVideoCommand(),
	BuildPlaylistCommand()
};

return await cmd.InvokeAsync(args);

//galera video
static Command BuildVideoCommand()
{
	var linkArgument = new Argument<string>("link", "Link to video");
	var outputOption = new Option<string>(new[] { "-o", "--output" }, "Output folder");
	var bitrateOption = new Option<BitrateSort>(new[] { "-b", "--bitrate" }, "Download video with max or min bitrate");
	var extensionOption = new Option<Extension>(new[] { "-e", "--extension" }, "Download video with specific extension");
	var streamTypeOption = new Option<StreamType>(new[] { "-s", "--stream-type" }, "Download specific stream");
	var command = new Command("video", "Download video")
	{
		linkArgument,
		outputOption,
		bitrateOption,
		extensionOption,
		streamTypeOption
	};

	command.SetHandler(async (string link, string output, BitrateSort bitrate, Extension extension, StreamType streamType) =>
	{
		try
		{
			Console.Write("Downloading ... ");
			var watch = new Stopwatch();
			watch.Start();

			var fileName = await Downloader.DownloadVideoAsync(link, output, bitrate, extension, streamType);

			watch.Stop();
			Console.WriteLine($"done! | {fileName} - {watch.Elapsed.TotalSeconds} sec");
		}
		catch (Exception e)
		{
			Console.WriteLine($"error | {e.Message}");
		}
	}, linkArgument, outputOption, bitrateOption, extensionOption, streamTypeOption);
	return command;
}

//galera playlist
static Command BuildPlaylistCommand()
{
	var linkArgument = new Argument<string>("link", "Link to playlist");
	var outputOption = new Option<string>(new[] { "-o", "--output" }, "Output folder");
	var bitrateOption = new Option<BitrateSort>(new[] { "-b", "--bitrate" }, "Download videos with max or min bitrate");
	var extensionOption = new Option<Extension>(new[] { "-e", "--extension" }, "Download videos with specific extension");
	var streamTypeOption = new Option<StreamType>(new[] { "-s", "--stream-type" }, "Download specific streams");
	var command = new Command("playlist", "Download playlist")
	{
		linkArgument,
		outputOption,
		bitrateOption,
		extensionOption,
		streamTypeOption
	};

	command.SetHandler(async (string link, string output, BitrateSort bitrate, Extension extension, StreamType streamType) =>
	{
		try
		{
			var watch = new Stopwatch();
			watch.Start();
			var (downloaded, errors) = await Downloader.DownloadPalylistAsync(link, output, bitrate, extension, streamType);
			watch.Stop();
			Console.WriteLine($"Done. Downloaded: {downloaded}. Errors: {errors}");
		}
		catch (Exception e)
		{
			Console.WriteLine($"error | {e.Message}");
		}
	}, linkArgument, outputOption, bitrateOption, extensionOption, streamTypeOption);

	return command;
}