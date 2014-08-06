boolean SampleSensors() {
  delay(50); // Required amount TBD
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
//---------------------------------------------------------------------------------------------------------------------------------------
boolean SampleAccelerometer() {
  PrintXYZValues();
  return false;
}
//---------------------------------------------------------------------------------------------------------------------------------------
boolean SampleTiltSensor() {
  return (TEST_BOARD) ? false : !digitalRead(pinTilt);
}
//---------------------------------------------------------------------------------------------------------------------------------------
boolean SampleGasSensor() {
  return (TEST_BOARD) ? false : !digitalRead(pinGasDetect);
}


