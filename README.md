# Telemetry Viewer

A cross-platform desktop application for real-time visualization and logging of multi-channel sensor data from embedded systems. Built with C# and Avalonia UI, it supports both UART serial communication and UDP network protocols for flexible data acquisition.

![.NET](https://img.shields.io/badge/.NET-9.0-blue)
![Avalonia](https://img.shields.io/badge/Avalonia-11.3.2-purple)
![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20macOS%20%7C%20Linux-lightgrey)

## Features

### 🔌 Multi-Protocol Data Acquisition
- **UART Serial Communication**: Direct connection to embedded devices via serial ports
- **UDP Network Protocol**: Wireless data reception over network connections
- **Configurable Baud Rates**: Support for various serial communication speeds
- **Port Management**: Easy configuration of UDP ports and COM ports

### 📊 Real-Time Data Visualization
- **Live Charts**: Real-time plotting using LiveCharts library
- **Multi-Channel Support**: Visualize multiple sensor channels simultaneously
- **Customizable Colors**: Individual color coding for each data channel
- **Interactive Controls**: Pause, resume, and clear data during collection

### 📝 Data Logging & Management
- **Automatic Logging**: All received data is automatically saved to CSV files
- **Timestamped Records**: Each data point includes precise timestamps
- **Organized Storage**: Data organized by connection type and timestamp
- **Persistent Logging**: Data continues to be logged even when visualization is paused

### ⚙️ Flexible Configuration
- **JSON-Based Configs**: Easy-to-edit configuration files
- **Multiple Data Types**: Support for int8, int16, int32, uint8, uint16, uint32
- **Binary & CSV Modes**: Handle both structured binary data and comma-separated values
- **Sync Byte Support**: Reliable packet synchronization for binary data streams

## Screenshots

*Screenshots would go here showing the main interface, plotting page, and configuration screens*

## Installation

### Prerequisites
- .NET 9.0 Runtime
- Windows 10/11, macOS 10.15+, or Linux (Ubuntu 18.04+)

### Download
1. Download the latest release from the [Releases](../../releases) page
2. Extract the archive to your desired location
3. Run `Telemetry_demo_Avalonia` executable

### Building from Source
```bash
git clone https://github.com/yourusername/telemetry-viewer.git
cd telemetry-viewer/Telemetry_demo_Avalonia
dotnet restore
dotnet build
dotnet run
```

## Quick Start

### 1. Configure Your Connection
1. Open the **Input** tab
2. Create a new configuration:
   - **UART**: Select COM port and baud rate
   - **UDP**: Specify port number and data format
3. Define your sensor channels with names, data types, and colors
4. Save the configuration

### 2. Start Data Collection
1. Switch to the **Plotting** tab
2. Select your configuration from the dropdown
3. Click **Start** to begin data collection
4. View real-time charts and monitor incoming data

### 3. Monitor and Log
- Data is automatically logged to CSV files in the `logs/` directory
- Use **Pause** to stop visualization while continuing to log
- Use **Clear** to reset the current data display

## Configuration Examples

### UART Configuration
```json
{
  "ConnectionName": "ESP32_IMU",
  "ConnectionType": "UART",
  "ComPort": "/dev/cu.usbserial-0001",
  "BaudRate": "115200",
  "Mode": "Binary Mode",
  "SyncByte": "AA",
  "Channels": [
    {
      "Name": "Accel_X",
      "Length": "int16",
      "Color": {"A": 255, "R": 255, "G": 0, "B": 0}
    },
    {
      "Name": "Accel_Y", 
      "Length": "int16",
      "Color": {"A": 255, "R": 0, "G": 255, "B": 0}
    }
  ]
}
```

### UDP Configuration
```json
{
  "ConnectionName": "Sensor_Network",
  "ConnectionType": "UDP",
  "UdpPort": "8080",
  "Mode": "CSV Mode",
  "Channels": [
    {
      "Name": "Temperature",
      "Length": "int16",
      "Color": {"A": 255, "R": 0, "G": 0, "B": 255}
    }
  ]
}
```

## Testing

### UDP Test Script
Use the included Python test script to verify UDP functionality:

```bash
# CSV Mode Testing
python udp_test_sender.py csv localhost 8080 0.1

# Binary Mode Testing  
python udp_test_sender.py binary localhost 9999 0.1
```

### UART Testing
Connect your embedded device to the specified COM port and ensure it's sending data in the configured format.

## Data Formats

### Binary Mode
- **Sync Byte**: Optional synchronization byte (e.g., 0xAA)
- **Data Types**: int8, int16, int32, uint8, uint16, uint32
- **Endianness**: Little-endian format
- **Packet Structure**: `[SyncByte][Channel1][Channel2]...[ChannelN]`

### CSV Mode
- **Format**: Comma-separated values
- **Example**: `23.45,1013.25,55.67`
- **Sync String**: Optional first value for synchronization
- **Example with sync**: `START,23.45,1013.25,55.67`

## Architecture

### Technology Stack
- **Framework**: .NET 9.0 with Avalonia UI
- **UI Pattern**: MVVM with ReactiveUI
- **Charts**: LiveCharts for real-time visualization
- **Serial**: System.IO.Ports for UART communication
- **Networking**: Built-in UDP socket support

### Project Structure
```
Telemetry_demo_Avalonia/
├── ViewModels/          # MVVM ViewModels
├── Views/              # Avalonia UI Views
├── Converters/         # UI Value Converters
├── Utils/              # Utility classes
└── Models/             # Data models
```

## Performance Optimizations

- **Efficient Data Parsing**: Optimized binary and CSV parsing for high-frequency data
- **Memory Management**: Proper disposal and cleanup of resources
- **UI Responsiveness**: Asynchronous data processing to prevent UI blocking
- **Configurable Update Rates**: Adjustable refresh rates for different data frequencies

## Troubleshooting

### Common Issues

**Port Already in Use**
- Ensure no other application is using the specified COM port or UDP port
- Check Windows Device Manager for COM port conflicts

**No Data Received (UART)**
- Verify COM port selection and baud rate
- Check cable connections
- Ensure device is sending data in the correct format

**No Data Received (UDP)**
- Check firewall settings
- Verify port number and host address
- Ensure sender is targeting the correct port

**Performance Issues**
- Reduce update frequency for high-rate data
- Limit the number of displayed data points
- Close unnecessary applications

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- [Avalonia UI](https://avaloniaui.net/) for the cross-platform UI framework
- [LiveCharts](https://livecharts.dev/) for real-time charting capabilities
- [ReactiveUI](https://reactiveui.net/) for MVVM pattern implementation

## Support

For support, please open an issue on GitHub or contact the development team.

---

**Made with ❤️ for embedded systems developers and data enthusiasts**
