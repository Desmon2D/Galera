using Galera.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace Galera
{
	public static class Downloader
	{
		private static readonly YoutubeClient _client = new();

		public static async Task<string> DownloadVideoAsync(string link, string folderPath, BitrateSort bitrate, Extension extension, StreamType streamType)
		{
			var videoTask = _client.Videos.GetAsync(link);

			if (string.IsNullOrEmpty(folderPath))
				folderPath = ".\\";
			Directory.CreateDirectory(folderPath);

			var manifest = await _client.Videos.Streams.GetManifestAsync(link);
			IEnumerable<IStreamInfo> streams = streamType switch
			{
				StreamType.muxed => manifest.GetMuxedStreams().OrderBy(a => a.VideoResolution.Area).ThenBy(a => a.Bitrate).AsEnumerable(),
				StreamType.video_only => manifest.GetVideoOnlyStreams().OrderBy(a => a.VideoResolution.Area).ThenBy(a => a.Bitrate).AsEnumerable(),
				StreamType.audio_only => manifest.GetAudioOnlyStreams(),
				_ => throw new NotImplementedException()
			};

			if (extension != Extension.any)
				streams = streams.Where(a => a.Container.Name == extension.ToString()).AsEnumerable();

			streams = streams.ToArray();

			if (!streams.Any())
				throw new ArgumentException("Specific streams not found");

			var streamInfo = bitrate == BitrateSort.min
				? streams.First()
				: streams.Last();

			var video = await videoTask;
			var path = NameHelper.Combine(folderPath, video.Title, streamInfo.Container.Name);

			await _client.Videos.Streams.DownloadAsync(streamInfo, path);

			return Path.GetFileName(path);
		}

		public static async Task<(int downloaded, int errors)> DownloadPalylistAsync(string link, string folderPath, BitrateSort bitrate, Extension extension, StreamType streamType)
		{
			var downloaded = 0;
			var errors = 0;
			var videos = _client.Playlists.GetVideosAsync(link);

			if (string.IsNullOrEmpty(folderPath))
				folderPath = ".\\";
			Directory.CreateDirectory(folderPath);

			await foreach (var video in videos)
			{
				try
				{
					Console.WriteLine($@"Downloading ""{video.Title}""");

					var manifest = await _client.Videos.Streams.GetManifestAsync(video.Id);
					IEnumerable<IStreamInfo> streams = streamType switch
					{
						StreamType.muxed => manifest.GetMuxedStreams().OrderBy(a => a.VideoResolution.Area).ThenBy(a => a.Bitrate).AsEnumerable(),
						StreamType.video_only => manifest.GetVideoOnlyStreams().OrderBy(a => a.VideoResolution.Area).ThenBy(a => a.Bitrate).AsEnumerable(),
						StreamType.audio_only => manifest.GetAudioOnlyStreams(),
						_ => throw new NotImplementedException()
					};

					if (extension != Extension.any)
						streams = streams.Where(a => a.Container.Name == extension.ToString()).AsEnumerable();

					streams = streams.ToArray();

					if (!streams.Any())
						throw new ArgumentException("Specific streams not found");

					var streamInfo = bitrate == BitrateSort.min
						? streams.First()
						: streams.Last();

					var path = NameHelper.Combine(folderPath, video.Title, streamInfo.Container.Name);
					if (File.Exists(path))
						throw new ArgumentException("File already exists");

					await _client.Videos.Streams.DownloadAsync(streamInfo, path);
					Console.WriteLine($@"Done ""{video.Title}""");
					downloaded++;
				}
				catch(Exception e)
				{
					Console.WriteLine($@"Error ""{video.Title}"" - {e.Message}");
					errors++;
				}
			}
			
			return new(downloaded, errors);
		}
	}
}
