#include "Timer.h"
#include "SoftwareSerial.h"
#define TEST_BOARD false
// Accelerometer
#define pinAccelX A0
#define pinAccelY A1
#define pinAccelZ A2
#define pinAccelZG 10
#define pinAccelSG 4
// Tilt
#define pinTilt 11
// Gas
#define pinGasDetect 11
#define pinGasLevel A3
// Panic Button
#define pinPanic 2
//  Cancel Button
#define pinCancel 3
// LEDs
#define pinLEDOn 6
#define pinLEDWarning 5
#define pinLEDAlert 7
// Buzzer
#define pinBuzzer 12
// BlueTooth
#define pinBlueToothTX 9
#define pinBlueToothRX 8
// Misc
#define pwmLEDBrightness 5
#define warningGracePeriod 15000
#define cycletimeBuzzer 500
#define defaultBaudRate 9600
#define seeedBTBaudRate 38400
#define seeedBTInitDelay 2000
// Messages
const String deviceName = "Sentinal-";
const String deviceID = "1234";
const String softwareVersion = ". - Software Version 0.3.0";
const String messageON = "O";
const String messageOK = "K";
const String messageNormal = "N";
const String messageWarning = "W";
const String messageAlert = "A";
const String messageCanceled = "C";
const String messageReasonGas = "G";
const String messageReasonFall = "F";
const String messageReasonPanic = "P";
const String messageDebug = "#DEBUG: ";
const String messageDelimiter = "|";
const String messageTerminator = ";";
// States
typedef enum {Normal, Warning, Alert, Canceled} condition;
volatile condition conditionCode;
typedef enum {Gas, Fall, Panic} reason;
volatile reason reasonCode;
// Misc
SoftwareSerial serialBT(pinBlueToothRX, pinBlueToothTX);
Timer timerCountdown;
Timer timerPulse;
int idTimerCountdown;
int idTimerPulse;
//------------------------------------------------------------------
void setup() {
  Serial.begin(defaultBaudRate);
  if (TEST_BOARD) {
    setupBlueToothConnection();
  }
  else
  {
    serialBT.begin(defaultBaudRate);
    delay(2000);
  }
  SendCustomMessage(messageDelimiter + messageON + messageDelimiter +
  messageOK + messageDelimiter + deviceName + deviceID + softwareVersion);
  pinMode(pinAccelSG,OUTPUT);
  //pinMode(pinTilt,INPUT);
  //pinMode(pinGasDetect,INPUT);
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
//------------------------------------------------------------------
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


