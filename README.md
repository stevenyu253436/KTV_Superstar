# DualScreenKTVPlayStation

DualScreenKTVPlayStation is a multimedia application designed to manage video and audio playback across multiple monitors. This application leverages DirectShow to handle various media types and provides synchronized playback functionality for dual-screen karaoke systems.

## Prerequisites

Before you can build and run this application, ensure that you have the following prerequisites installed:

- **Windows OS**: This application is designed to run on Windows.
- **.NET Framework**: Make sure you have the appropriate version of the .NET Framework installed.
- **DirectShow**: Ensure DirectShow components are properly installed and registered.
- **Dll Files**: Ensure all required .dll files are placed in the correct directories.

## Building the Application

To build the application, follow these steps:

1. **Clone the repository**: 
    ```sh
    git clone https://github.com/stevenyu253436/KTV_Superstar.git
    cd KTV_Superstar
    ```

2. **Run the build script**:
    Execute the `build.bat` script to compile the application. This script will handle the compilation process and generate the necessary executable files.
    ```sh
    build.bat
    ```

## Running the Application

Once the application is built, you can run it using the following steps:

1. **Navigate to the output directory**:
    ```sh
    cd bin/Release
    ```

2. **Execute the application**:
    ```sh
    DualScreenKTVPlayStation.exe
    ```

## Configuration

Before running the application, ensure that all necessary .dll files are in place. The required .dll files should be located in the same directory as the executable or in a directory included in your system's PATH environment variable.

## Features

- **Dual Screen Playback**: Play video and audio content across two screens, synchronized for karaoke use.
- **Media Management**: Handle different media types, including video and audio, with support for various codecs.
- **DirectShow Integration**: Leverage DirectShow for advanced media playback capabilities.

## License

This project is licensed under the GNU Affero General Public License v3.0 (AGPL-3.0). See the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please read the [CONTRIBUTING.md](CONTRIBUTING.md) file for guidelines on how to contribute to this project.

## Support

For support, please open an issue in the GitHub repository or contact the maintainers directly.

## Authors

- **Steven Yu** - *Initial work* - [Your GitHub Profile](https://github.com/stevenyu253436)

## Acknowledgments

- Special thanks to the contributors and the open-source community for their support and tools.

