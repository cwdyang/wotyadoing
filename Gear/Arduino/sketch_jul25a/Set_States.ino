void ResetState() {
  conditionCode = Normal;
  digitalWrite(pinBuzzer, LOW);
  digitalWrite(pinLEDAlert, LOW);
  analogWrite(pinLEDWarning, LOW);
  analogWrite(pinLEDOn, pwmLEDBrightness);
}
void SetWarningState() {
  conditionCode = Warning;
  digitalWrite(pinBuzzer, LOW);
  digitalWrite(pinLEDAlert, LOW);
  analogWrite(pinLEDOn, LOW);
  analogWrite(pinLEDWarning, HIGH);
}
void SetAlertState() {
  conditionCode = Alert;
  digitalWrite(pinBuzzer, HIGH);
  analogWrite(pinLEDWarning, LOW);
  analogWrite(pinLEDOn, LOW);
  digitalWrite(pinLEDAlert, HIGH);
}
