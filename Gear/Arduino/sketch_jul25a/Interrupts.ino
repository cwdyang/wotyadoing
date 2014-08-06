void PanicButtonPressed() {
    reasonCode = Panic;
    RaiseAlert();
}
//---------------------------------------------------------------------------------------------------------------------------------------
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
