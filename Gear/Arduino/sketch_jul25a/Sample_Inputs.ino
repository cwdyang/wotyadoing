byte SampleSensors() {
  delay(1); // Required amount TBD
  if (SampleAccelerometer()) {
   reasonCode = Fall;
   return true;
  }
  if (SampleTiltSensor()) {
   reasonCode = Fall;
   return true;
  }  
  if (SampleGasSensor()) {
  reasonCode = Gas;
  //return true;
  }  
  return false;
}
byte SampleAccelerometer() {
int x;
int y;
int z;
int g;

String value;
String message;
/* 9:45AM Sunday Morning. 6 Hours sleep since Friday 7am and I have to write 
this last minute crap code becuase we had to diagnose BlutTooth problems for 5 hours last night!
No time for my Opus Magnum...the FALL ALGO.
Oh God forgive me for my transcendance into darkness...
*/
  x = analogRead(pinAccelX);
  y = analogRead(pinAccelY);
  z = analogRead(pinAccelZ);
  g = analogRead(pinAccelZG);
  
//z = analogRead(pinAccelZ)
  value = "X=" + x; 
  message += value;
  value = "|Y=" + y;
  message += value;
  value = "|Z=" + z;
  message += value;
  //z = analogRead(pinAccelZ)
  //Serial.println(x);
  delay(100);
  if (z >= 750) return true;
  if (y >=750 || y<=150) return true;
  if (x >=600 || x<=100) return true;
  return false;
}
byte SampleTiltSensor() {
  //if (TEST_BOARD) return digitalRead(pinTilt);
  //return !digitalRead(pinTilt);
}
byte SampleGasSensor() {
  //if (TEST_BOARD) return digitalRead(pinGasDetect);
  //return !digitalRead(pinGasDetect);
}


