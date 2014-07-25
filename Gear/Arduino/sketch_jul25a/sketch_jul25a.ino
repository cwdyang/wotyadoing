#include "Timer.h"
#include "SoftwareSerial.h"
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
SoftwareSerial serialBT(pinBlueToothRX, pinBlueToothTX); // RX, TX
Timer timerCountdown;
Timer timerPulse;
int idTimerCountdown;
int idTimerPulse;
int warningGracePeriod = 15000;
int cycletimeBuzzer = 500;
// Messages
String deviceID = "1234";
String messageON = "O";
String messageWarning = "W";
String messageAlert = "A";
String messageCanceled = "C";
String messageReasonGas = "G";
String messageReasonFall = "F";
String messageReasonPanic = "P";
String messageDelimiter = "|";
// States
typedef enum {Gas, Fall, Panic} reason;
volatile reason reasonCode;
typedef enum {Normal, Warning, Alert, Canceled} condition;
volatile condition conditionCode;

void setup() {
  
  Serial.begin(9600);
  serialBT.begin(9600);
  SendCustomMessage(messageDelimiter + messageON + messageDelimiter + 
  "SentriCare Client - Ver 0.1 BETA -> Hello World!");
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

  if (conditionCode == Normal) {
   if (SampleSensors()) {
     if (reasonCode == Gas) {
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
  conditionCode = Normal;
  digitalWrite(pinBuzzer, LOW);
  digitalWrite(pinLEDAlert, LOW);
  digitalWrite(pinLEDWarning, LOW);
  digitalWrite(pinLEDOn, HIGH);
}
String ConstructReason(byte theReason) {
   switch (theReason) {
     case Gas:
       return messageReasonGas;
       break;
     case Fall:
       return messageReasonFall;
       break;
     case Panic:
       return messageReasonPanic;
    }
}
String ConstructCondition(byte theCondition) {
   switch (theCondition) {
     case Normal:
       return "N";
       break;
     case Warning:
       return messageWarning;
       break;
     case Alert:
       return messageAlert;
     case Canceled:
       return messageCanceled;

    }
}


