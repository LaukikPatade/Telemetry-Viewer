//// Basic demo for accelerometer readings from Adafruit MPU6050
//
//// ESP32 Guide: https://RandomNerdTutorials.com/esp32-mpu-6050-accelerometer-gyroscope-arduino/
//// ESP8266 Guide: https://RandomNerdTutorials.com/esp8266-nodemcu-mpu-6050-accelerometer-gyroscope-arduino/
//// Arduino Guide: https://RandomNerdTutorials.com/arduino-mpu-6050-accelerometer-gyroscope/
//
//#include <Adafruit_MPU6050.h>
//#include <Adafruit_Sensor.h>
//#include <Wire.h>
//
//Adafruit_MPU6050 mpu;
//
//void setup(void) {
//  Serial.begin(1500000);
//  // Serial.begin(115200);
//  // while (!Serial)
//    // delay(10);  // will pause Zero, Leonardo, etc until serial console opens
//
//  // Serial.println("Adafruit MPU6050 test!");
//
//  // Try to initialize!
//  if (!mpu.begin()) {
//    Serial.println("Failed to find MPU6050 chip");
//    while (1) {
//      delay(10);
//    }
//  }
//  // Serial.println("MPU6050 Found!");
//
//  mpu.setAccelerometerRange(MPU6050_RANGE_8_G);
//  // Serial.print("Accelerometer range set to: ");
//  // switch (mpu.getAccelerometerRange()) {
//  //   case MPU6050_RANGE_2_G:
//  //     Serial.println("+-2G");
//  //     break;
//  //   case MPU6050_RANGE_4_G:
//  //     Serial.println("+-4G");
//  //     break;
//  //   case MPU6050_RANGE_8_G:
//  //     Serial.println("+-8G");
//  //     break;
//  //   case MPU6050_RANGE_16_G:
//  //     Serial.println("+-16G");
//  //     break;
//  // }
//  mpu.setGyroRange(MPU6050_RANGE_2000_DEG);
//  // Serial.print("Gyro range set to: ");
//  // switch (mpu.getGyroRange()) {
//  //   case MPU6050_RANGE_250_DEG:
//  //     Serial.println("+- 250 deg/s");
//  //     break;
//  //   case MPU6050_RANGE_500_DEG:
//  //     Serial.println("+- 500 deg/s");
//  //     break;
//  //   case MPU6050_RANGE_1000_DEG:
//  //     Serial.println("+- 1000 deg/s");
//  //     break;
//  //   case MPU6050_RANGE_2000_DEG:
//  //     Serial.println("+- 2000 deg/s");
//  //     break;
//  // }
//
//  mpu.setFilterBandwidth(MPU6050_BAND_260_HZ);
//  // Serial.print("Filter bandwidth set to: ");
//  // switch (mpu.getFilterBandwidth()) {
//  //   case MPU6050_BAND_260_HZ:
//  //     Serial.println("260 Hz");
//  //     break;
//  //   case MPU6050_BAND_184_HZ:
//  //     Serial.println("184 Hz");
//  //     break;
//  //   case MPU6050_BAND_94_HZ:
//  //     Serial.println("94 Hz");
//  //     break;
//  //   case MPU6050_BAND_44_HZ:
//  //     Serial.println("44 Hz");
//  //     break;
//  //   case MPU6050_BAND_21_HZ:
//  //     Serial.println("21 Hz");
//  //     break;
//  //   case MPU6050_BAND_10_HZ:
//  //     Serial.println("10 Hz");
//  //     break;
//  //   case MPU6050_BAND_5_HZ:
//  //     Serial.println("5 Hz");
//  //     break;
//  // }
//
//  Serial.println("");
//  delay(100);
//}
//
//void loop() {
//  /* Get new sensor events with the readings */
//  sensors_event_t a, g, temp;
//  mpu.getEvent(&a, &g, &temp);
//
//  //Sending Data
//  int16_t Ax = a.acceleration.x;
//  int16_t Ay = a.acceleration.y;
//  int16_t Az = a.acceleration.z;
//  int16_t Gx = g.gyro.x;
//  int16_t Gy = g.gyro.y;
//  int16_t Gz = g.gyro.z;
//  // Serial.println("A");
//  // delay(100);
//  Serial.write(0xAA);  // Send the start/sync byte
//  //Accl Data
//  Serial.write((uint8_t*)&(Ax), sizeof(Ax));
//  Serial.write((uint8_t*)&(Ay), sizeof(Ay));
//  Serial.write((uint8_t*)&(Az), sizeof(Az));
//  //Gyro Data
//  Serial.write((uint8_t*)&(Gx), sizeof(Gx));
//  Serial.write((uint8_t*)&(Gy), sizeof(Gy));
//  Serial.write((uint8_t*)&(Gz), sizeof(Gz));
//
//
//  // /* Print out the values */
//  // // Serial.print("X: ");
//  // Serial.print(a.acceleration.x);
//  // Serial.print(", ");
//  // Serial.print(a.acceleration.y);
//  // Serial.print(", ");
//  // Serial.print(a.acceleration.z);
//  // // Serial.println(" m/s^2");
//
//  // Serial.print(", ");
//  // Serial.print(g.gyro.x);
//  // Serial.print(", ");
//  // Serial.print(g.gyro.y);
//  // Serial.print(", ");
//  // Serial.println(g.gyro.z);
//  // Serial.println(" rad/s");
//
//  // Serial.print("Temperature: ");
//  // Serial.print(temp.temperature);
//  // Serial.println(" degC");
//
//  // Serial.println("");
//  // delay(5);
//}



#include <Arduino.h>

void setup() {
  Serial.begin(1500000); // High baud rate for faster transmission
  delay(100);            // Give time for Serial to initialize
}

void loop() {
  // Generate fake int16_t data (simulating ±8g and ±2000 deg/s)
  int16_t Ax = random(-8000, 8000); // Accelerometer X
  int16_t Ay = random(-8000, 8000); // Accelerometer Y
  int16_t Az = random(-8000, 8000); // Accelerometer Z
  int16_t Gx = random(-2000, 2000); // Gyro X
  int16_t Gy = random(-2000, 2000); // Gyro Y
  int16_t Gz = random(-2000, 2000); // Gyro Z

  // Send sync byte
  Serial.write(0xAA);  // Frame start indicator

  // Send all sensor data as binary (12 bytes total)
  Serial.write((uint8_t*)&Ax, sizeof(Ax));
  Serial.write((uint8_t*)&Ay, sizeof(Ay));
  Serial.write((uint8_t*)&Az, sizeof(Az));
  Serial.write((uint8_t*)&Gx, sizeof(Gx));
  Serial.write((uint8_t*)&Gy, sizeof(Gy));
  Serial.write((uint8_t*)&Gz, sizeof(Gz));

  // Wait a little to simulate sensor sampling rate (~200 Hz)
  delay(5);
}
