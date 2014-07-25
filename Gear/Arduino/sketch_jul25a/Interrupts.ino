void PanicButtonPressed() {
    reasonPanic = true;
    conditionAlert = true;
  // Send Message()  
    RaiseAlert();
}
void CancelButtonPressed() {
  // Send Message()  
  if (conditionWarning) CancelWarning();
  if (conditionAlert) CancelAlert();  
}
