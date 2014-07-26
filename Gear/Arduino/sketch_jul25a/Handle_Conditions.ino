void RaiseWarning() {
  if (conditionCode == Alert) return;
  conditionCode = Warning;
  SendServerMessage();   
  StartAlertCountdown();
  digitalWrite(pinLEDOn, LOW);
  digitalWrite(pinLEDAlert, LOW);
  digitalWrite(pinLEDWarning, HIGH);
}
 void RaiseAlert() {
  if (conditionCode == Alert) return;
  conditionCode = Alert;
  StopTimers();
  SendServerMessage();
  digitalWrite(pinLEDWarning, LOW);
  digitalWrite(pinLEDOn, LOW);
  digitalWrite(pinLEDAlert, HIGH);
  digitalWrite(pinBuzzer, HIGH);
}
void CancelWarning() {
  StopTimers();
  SendServerMessage();   
  ResetState();
}
 void CancelAlert() {
  StopTimers();
  SendServerMessage();   
  ResetState();
}

