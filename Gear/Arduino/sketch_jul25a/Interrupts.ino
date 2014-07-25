void PanicButtonPressed() {
    reasonCode = Panic;
    conditionCode = Alert;
    RaiseAlert();
}
void CancelButtonPressed() {
  if (conditionCode == Warning) {
    conditionCode = Canceled;
    CancelWarning();
  }
  if (conditionCode == Alert) {
    conditionCode = Canceled;
    CancelAlert();
  }  
}
