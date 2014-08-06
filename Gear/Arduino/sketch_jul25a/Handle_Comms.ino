void setupBlueToothConnection()
{
    /*if (!digitalRead(pinPanic)) {*/
      serialBT.begin(seeedBTBaudRate);                                // Set BluetoothBee BaudRate to default baud rate 38400
      serialBT.print("\r\n+STWMOD=0\r\n");                            // set the bluetooth work in slave mode
      serialBT.print("\r\n+STNA=" + deviceName + deviceID + "\r\n");  // set the bluetooth name
      serialBT.print("\r\n+STOAUT=1\r\n");                            // Permit Paired device to connect me
      serialBT.print("\r\n+STAUTO=0\r\n");                            // Auto-connection should be forbidden here
      delay(seeedBTInitDelay);                                        // This delay is required.
      serialBT.print("\r\n+INQ=1\r\n");                               // make the slave bluetooth inquirable
      delay(seeedBTInitDelay);                                        // This delay is required.
      serialBT.flush();
    //}
}
//---------------------------------------------------------------------------------------------------------------------------------------
void SendCustomMessage(String message) {
  message = (deviceID + message);   
  if (digitalRead(pinPanic)) {
    serialBT.print(message + messageTerminator);
    serialBT.flush();
  }
  if (DEBUG) Serial.println(messageDebug + message);
}
//---------------------------------------------------------------------------------------------------------------------------------------
void SendServerMessage() {
  String message;
  message = deviceID + messageDelimiter + ConstructCondition(conditionCode)
  + messageDelimiter + ConstructReason(reasonCode);   
  if (digitalRead(pinPanic)) {
    serialBT.print(message + messageTerminator);
    serialBT.flush();
  }
  if (DEBUG) Serial.println(messageDebug + message);
}
//---------------------------------------------------------------------------------------------------------------------------------------
void SendRawData(String message) {
  if (digitalRead(pinPanic)) {
    serialBT.print(message);
    analogWrite(pinLEDOn, LOW);
    analogWrite(pinLEDWarning, pwmLEDBrightness);
  }
  else {
    analogWrite(pinLEDWarning, LOW);
    analogWrite(pinLEDOn, pwmLEDBrightness);  
  }
  serialBT.flush();
  if (DEBUG) Serial.print(message);
}
