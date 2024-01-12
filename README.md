# DownTube - Open-Source YouTube Video Downloader

## Introduction

DownTube is a cross-platform open-source application developed by EnterACE using .NET MAUI. This application allows users to search for YouTube videos, view essential information such as title, duration, and thumbnail, and download the videos to their local devices, subject to YouTube's terms of service.

## Features

- Retrieve video information using the YouTubeExplode NuGet package.
- Save downloaded videos to the Downloads folder on Android devices and the Documents folder on Windows.

## Requirements

- Internet connection for searching and fetching video information.
- Proper storage permissions on the device for saving downloaded videos.


## Getting Started

To get started with DownTube, download the latest release from the [Releases](https://github.com/enterace/DownTube/releases) page. Choose the appropriate version for your platform (Windows or Android) and download the file.

- ***On Windows 10 or above:***
	1. **Download the Latest Release:**
	   - Visit the [Releases](https://github.com/enterace/DownTube/releases) page.
	   - Download the executable file with the following format:
		   `Windows10-x86_MSIX_DownTube_vX.X.X.X.zip`

	2. **Unzip the downloaded file:**
	   - Right-click and then unzip the downloaded file.

	3. **Open the unzipped folder and run the installer:**
	   - In the folder run the file with the following format:
		    `DownTube_X.X.X.X_x86.msix`
		- On the installer window click on the install button.
	
- ***On Android:***
	1. **Download the Latest Release:**
	   - Visit the [Releases](https://github.com/enterace/DownTube/releases) page.
	   - Download the executable file with the following format:
		   `Android_universal_DownTube_vX.X.X.X.apk`

	2. **Run the installer:**
	   - Tap on the downloaded file and install it. 
		**Note:** This may need you to disable some security settings on your phone.
		

## Usage

1. Launch the DownTube application.
2. Paste the youtube video URL in the URL section.
3. Tap on the `Load Video` button.
4. You now can see the video title, duration and thumbnail.
5. Click the `Download Video` button to download and save the video on your device.

## Contributing

All contributions to this project are welcome!

## License

DownTube is licensed under the [MIT License](LICENSE). Feel free to use, modify, and distribute the code.

## Disclaimer

DownTube is not affiliated with YouTube. Please respect YouTube's terms of service and use this application responsibly.
