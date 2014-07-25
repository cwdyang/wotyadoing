#include "Timer.h"
// Acceleromter
const byte pinAccelX = A0;
const byte pinAccelY = A1;
const byte pinAccelZ = A2;
const byte pinAccelZG = 7;
const byte pinAccelSG = 6;
// Tilt
const byte pinTilt = 4;
// Gas
const byte pinGasDetect = 5;
const byte pinGasLevel = A3;
// Panic Button
const byte pinPanic = 2;
//  Cancel Button
const byte pinCancel = 3;
// LEDs
const byte pinLEDOn = 12;
const byte pinLEDWarning = 13;
const byte pinLEDAlert = 11;
// Buzzer
const byte pinBuzzer = 10;
// BlueTooth
const byte pinBlueToothTX = 9;
const byte pinBlueToothRX = 8;
// Misc
Timer timerCountdown;
Timer timerPulse;
int idTimerCountdown;
int idTimerPulse;
int warningGracePeriod = 15000;
// Messages
String DEVICE_ID = "1234";
String messageON = "O";
String messageWarning = "W";
String messageAlert = "A";
String messageCanceled = "C";
String messageReasonGas = "G";
String messageReasonFall = "F";
String messageReasonPanic = "P";
String messageDelimiter = "|";
// States
volatile byte conditionAlert;
volatile byte conditionWarning;
volatile byte reasonGas;
volatile byte reasonFall;
volatile byte reasonPanic;
volatile byte stateButtonPanic;  // Use?
volatile byte stateButtonCancel; // Use?

void setup() {
  
  Serial.begin(9600);
  
  pinMode(pinAccelZG,INPUT);
  pinMode(pinAccelSG,INPUT);
  pinMode(pinTilt,INPUT);
  pinMode(pinGasDetect,INPUT);
  pinMode(pinPanic,INPUT);
  pinMode(pinLEDAlert,OUTPUT);
  pinMode(pinLEDWarning,OUTPUT);
  pinMode(pinLEDOn,OUTPUT);
  pinMode(pinBuzzer,OUTPUT);
  pinMode(pinBlueToothTX,OUTPUT);
  pinMode(pinBlueToothRX,INPUT);
  
  attachInterrupt(0, PanicButtonPressed, HIGH);
  attachInterrupt(1, CancelButtonPressed, HIGH);
  ResetState();
} 
void loop() {

  if (!conditionWarning && !conditionAlert) {
//   if (!digitalRead(pinTilt)) RaiseWarning();
   if (SampleSensors()) {
     if (reasonGas) {
       conditionAlert = true;
       RaiseAlert();
     }
     else {
       RaiseWarning();
     }
   }
  }
  timerCountdown.update();
  timerPulse.update();  
}
void ResetState() {
  conditionAlert = false;
  conditionWarning = false;
  reasonGas = false;
  reasonFall = false;
  reasonPanic = false;
  stateButtonPanic = false;
  stateButtonCancel = false;
  digitalWrite(pinBuzzer, LOW);
  digitalWrite(pinLEDAlert, LOW);
  digitalWrite(pinLEDWarning, LOW);
  digitalWrite(pinLEDOn, HIGH);
}



