byte SampleSensors() {
  delay(1);
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
  return true;
  }  
  return false;
}
byte SampleAccelerometer() {
int x;
int y;
int z;
// To Do
  return false;
}
byte SampleTiltSensor() {
  if (TEST_BOARD) return digitalRead(pinTilt);
  return !digitalRead(pinTilt);
}
byte SampleGasSensor() {
    if (TEST_BOARD) return digitalRead(pinGasDetect);
    return !digitalRead(pinGasDetect);
}


