#include <WiFi.h>       // Use <ESP8266WiFi.h> if you're on ESP8266
#include <WiFiUdp.h>

const char* ssid     = "NCAIR IOT";        // 🔁 Replace with your WiFi SSID
const char* password = "Asim@123Tewari";    // 🔁 Replace with your WiFi password

const char* udpAddress = "192.168.0.118";  // 🔁 Replace with your PC's IP
const int udpPort = 8080;

WiFiUDP udp;

void setup() {
  Serial.begin(115200);
  delay(1000);

  // Connect to WiFi
  Serial.println("Connecting to WiFi...");
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("\nConnected to WiFi!");
  Serial.print("IP address: ");
  Serial.println(WiFi.localIP());

  udp.begin(udpPort); // Optional for ESP to receive data
}

void loop() {
  // Generate random CSV data (e.g. temperature, humidity, pressure)


  float Ax = (random(-100, 100)) / 100.0;
  float Ay = (random(-100, 100)) / 100.0;
  float Az = (random(-100, 100)) / 100.0;
  String csvData = String(Ax) + "," + String(Ay) + "," + String(Az);
  Serial.println("Sending: " + csvData);

  // Send data as UDP packet
  udp.beginPacket(udpAddress, udpPort);
  udp.print(csvData);
  udp.endPacket();

  delay(10); // Send every second
}
