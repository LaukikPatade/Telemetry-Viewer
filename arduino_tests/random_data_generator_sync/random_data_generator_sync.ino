void setup() {
    Serial.begin(1500000);  // Start serial communication
    randomSeed(analogRead(0));  // Seed random generator
}
uint8_t sync_byte = 0xAA;
void loop() {
    // Generate random float values between -1.0 and 1.0
    int16_t Ax = (int)(random(100, 25000)) / 100;
//    int16_t Ay = (int)(random(25000, 50000)) / 100;
//    int16_t Ax = (random(-100, 100)) / 100.0;
//    int16_t Ay = (random(-100, 100)) / 100.0;
//    int16_t Az = (random(-100, 100)) / 100.0;
//    int16_t Gx = (random(-100, 100)) / 100.0;
//    int16_t Gy = (random(-100, 100)) / 100.0;
//    int16_t Gz = (random(-100, 100)) / 100.0;

//    // Print values in CSV format
//    Serial.print(Ax, 2);
//    Serial.print(",");
  //    Serial.print(Ay, 2);e e 
//    Serial.print(",");
//    Serial.print(Az, 2); 
//    Serial.print(",");
//    Serial.print(Gx, 2);
//    Serial.print(",");
//    Serial.print(Gy, 2);
//    Serial.print(",");
//    Serial.println(Gz, 2);// Newline at end
// Print values in CSV format
    Serial.write(sync_byte);
    Serial.write((uint8_t *)&(Ax), sizeof(Ax));
//    Serial.write((uint8_t *)&(Ay), sizeof(Ay));
//    delay(); // Send new data every 100ms
}
