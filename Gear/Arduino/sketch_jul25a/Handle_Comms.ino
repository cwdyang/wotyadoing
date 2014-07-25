void WaitForBTSetial() {
  // To Do
}
void SendCustomMessage(String message) {
  message = (deviceID + message);   
  serialBT.println(message);
  if (Serial.available())Serial.println(messageDebug + message);
}
void SendServerMessage() {
  String message;
  message = (deviceID + messageDelimiter + ConstructCondition(conditionCode)
  + messageDelimiter + ConstructReason(reasonCode));   
  serialBT.println(message);
  Serial.println(messageDebug + message);
}
