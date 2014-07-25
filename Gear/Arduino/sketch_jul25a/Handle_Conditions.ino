void RaiseWarning() {
  if (conditionAlert) return;
  // Send Message()
  conditionWarning = true;  
  StartAlertCountdown();
  digitalWrite(pinLEDOn, LOW);
  digitalWrite(pinLEDAlert, LOW);
  digitalWrite(pinLEDWarning, HIGH);
}
 void RaiseAlert() {
  if (conditionWarning) conditionWarning = false;
  StopTimers();
  // Send Message()  
  digitalWrite(pinLEDWarning, LOW);
  digitalWrite(pinLEDOn, LOW);
  digitalWrite(pinLEDAlert, HIGH);
  digitalWrite(pinBuzzer, HIGH);
}
void CancelWarning() {
  StopTimers();
  // Send Message()  
  ResetState();
}
 void CancelAlert() {
  StopTimers();
  // Send Message()  
  ResetState();
}

