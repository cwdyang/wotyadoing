void RaiseWarning() {
  if (conditionCode == Alert) return;
  SetWarningState();
  SendServerMessage();   
  StartAlertCountdown();
}
//---------------------------------------------------------------------------------------------------------------------------------------
 void RaiseAlert() {
  if (conditionCode == Alert) return;
  StopTimers();
  SetAlertState();
  SendServerMessage();
}
//---------------------------------------------------------------------------------------------------------------------------------------
void CancelWarning() {
  StopTimers();
  SendServerMessage();   
  ResetState();
}
//---------------------------------------------------------------------------------------------------------------------------------------
 void CancelAlert() {
  StopTimers();
  SendServerMessage();   
  ResetState();
}

