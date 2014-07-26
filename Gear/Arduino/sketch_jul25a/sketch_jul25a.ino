#include "Timer.h"
#include "SoftwareSerial.h"
byte TEST_BOARD = true;
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
// Messages
String deviceID = "1234";
String messageON = "O";
String messageOK = "K";
String messageNormal = "N";
String messageWarning = "W";
String messageAlert = "A";
String messageCanceled = "C";
String messageReasonGas = "G";
String messageReasonFall = "F";
String messageReasonPanic = "P";
String messageDebug = "#DEBUG: ";
String messageDelimiter = "|";
String messageTerminator = ";";
// States
typedef enum {Normal, Warning, Alert, Canceled} condition;
volatile condition conditionCode;
typedef enum {Gas, Fall, Panic} reason;
volatile reason reasonCode;
// Misc
SoftwareSerial serialBT(pinBlueToothRX, pinBlueToothTX); // RX, TX
Timer timerCountdown;
Timer timerPulse;
int idTimerCountdown;
int idTimerPulse;
int warningGracePeriod = 15000;
int cycletimeBuzzer = 500;
//------------------------------------------------------------------
void setup() {
  Serial.begin(9600);
  if (TEST_BOARD) {
    setupBlueToothConnection();
  }
  else
  {
    serialBT.begin(9600);
    delay(2000);
  }
  SendCustomMessage(messageDelimiter + messageON + messageDelimiter +
  messageOK + messageDelimiter + "SentriCare Client - Ver 0.1 BETA -> Hello World!");
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
void ResetState() {
  conditionCode = Normal;
  digitalWrite(pinBuzzer, LOW);
  digitalWrite(pinLEDAlert, LOW);
  digitalWrite(pinLEDWarning, LOW);
  digitalWrite(pinLEDOn, HIGH);
}
void setupBlueToothConnection()
{
    serialBT.begin(38400);                           // Set BluetoothBee BaudRate to default baud rate 38400
    serialBT.print("\r\n+STWMOD=0\r\n");             // set the bluetooth work in slave mode
    serialBT.print("\r\n+STNA=SFX\r\n");    // set the bluetooth name as "SeeedBTSlave"
    serialBT.print("\r\n+STOAUT=1\r\n");             // Permit Paired device to connect me
    serialBT.print("\r\n+STAUTO=0\r\n");             // Auto-connection should be forbidden here
    delay(2000);                                            // This delay is required.
    serialBT.print("\r\n+INQ=1\r\n");                // make the slave bluetooth inquirable
    delay(2000);                                            // This delay is required.
    serialBT.flush();
}
