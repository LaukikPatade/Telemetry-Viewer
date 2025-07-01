#include <WiFi.h>       // Use <ESP8266WiFi.h> if you're on ESP8266
#include <WiFiUdp.h>

const char* ssid     = "NCAIR IOT";        // 🔁 Replace with your WiFi SSID
const char* password = "Asim@123Tewari";   // 🔁 Replace with your WiFi password

const char* udpAddress = "192.168.0.131";  // 🔁 Replace with your PC's IP
const int udpPort = 8080;

WiFiUDP udp;
const uint8_t syncByte = 0xAA;             // Sync byte to mark the start of each packet

void setup() {
  Serial.begin(115200);
  delay(1000);

  Serial.println("Connecting to WiFi...");
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("\nConnected to WiFi!");
  Serial.print("IP address: ");
  Serial.println(WiFi.localIP());

  udp.begin(udpPort); // Optional for receiving data
}

void loop() {
  // Simulated accelerometer data
  float Ax = (random(-100, 100)) / 100.0;
  float Ay = (random(-100, 100)) / 100.0;
  float Az = (random(-100, 100)) / 100.0;

  // Send binary UDP packet with sync byte
  udp.beginPacket(udpAddress, udpPort);
  udp.write(syncByte);              // Sync byte

  udp.write((uint8_t*)&Ax, sizeof(float)); // Ax
  udp.write((uint8_t*)&Ay, sizeof(float)); // Ay
  udp.write((uint8_t*)&Az, sizeof(float)); // Az

  udp.endPacket();

  Serial.print("Sent binary packet: Ax=");
  Serial.print(Ax, 2);
  Serial.print(", Ay=");
  Serial.print(Ay, 2);
  Serial.print(", Az=");
  Serial.println(Az, 2);

  delay(10); // Send every 10ms
}
