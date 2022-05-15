# Galera

Galera is a console util for downloading videos/playlists from YouTube. 

## Usage
### Examples
```
Download video:
> galera video https://www.youtube.com/watch?v=dQw4w9WgXcQ

Download only audio from playlist:
> galera playlist https://www.youtube.com/playlist?list=PLKIxB9vhdS_3x0A5za3mmu1wdoolgRQ65 -s audio_only -o .\PlaylistAudios -b max
```

### root
```
Usage:
  galera <command> [options]

Options:
  --version       Show version information
  -?, -h, --help  Show help and usage information

Commands:
  video <link>     Download video
  playlist <link>  Download playlist
```
### video
```
Description:
  Download video

Usage:
  galera video <link> [options]

Arguments:
  <link>  Link to video

Options:
  -o, --output <output>                            Output folder
  -b, --bitrate <max|min>                          Download video with max or min bitrate
  -e, --extension <any|mp3|mp4|tgpp|webm>          Download video with specific extension
  -s, --stream-type <audio_only|muxed|video_only>  Download specific stream
  -?, -h, --help                                   Show help and usage information
```
### playlist
```
Description:
  Download playlist

Usage:
  galera playlist <link> [options]

Arguments:
  <link>  Link to playlist

Options:
  -o, --output <output>                            Output folder
  -b, --bitrate <max|min>                          Download videos with max or min bitrate
  -e, --extension <any|mp3|mp4|tgpp|webm>          Download videos with specific extension
  -s, --stream-type <audio_only|muxed|video_only>  Download specific streams
  -?, -h, --help                                   Show help and usage information
```
