void setup() {
    Serial.begin(115200);  // Start serial communication
    randomSeed(analogRead(0));  // Seed random generator
}

void loop() {
    // Generate random float values between -1.0 and 1.0

      float Ax = (random(-100, 100)) / 100.0;
      float Ay = (random(-100, 100)) / 100.0;
//    int16_t Az = (random(-100, 100)) / 100.0;
//    int16_t Gx = (random(-100, 100)) / 100.0;
//    int16_t Gy = (random(-100, 100)) / 100.0;
//    int16_t Gz = (random(-100, 100)) / 100.0;

//    // Print values in CSV format
    Serial.print(Ax, 2);
    Serial.print(",");
      Serial.println(Ay, 2);
//    Serial.print(",");
//    Serial.print(Az, 2); 
//    Serial.print(",");
//    Serial.print(Gx, 2);
//    Serial.print(",");
//    Serial.print(Gy, 2);
//    Serial.print(",");
//    Serial.println(Gz, 2);// Newline at end
// Print values in CSV format

    delay(10); // Send new data every 100ms
}
