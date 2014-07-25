byte SampleSensors() {
  
  delay(1);
  if (SampleAccelerometer()) {
   reasonFall = true;
   return true;
  }
  if (SampleTiltSensor()) {
   reasonFall = true;
   return true;
  }  
  if (SampleGasSensor()) {
  reasonGas = true;
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
  return !digitalRead(pinTilt);
}
byte SampleGasSensor() {
  return !digitalRead(pinGasDetect);
}

