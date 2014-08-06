#include "Timer.h"
#include "SoftwareSerial.h"
#include <Wire.h>
#include <math.h>
#define TEST_BOARD true
#define DEBUG true
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
#define pwmLEDBrightness 1
#define warningGracePeriod 15000
#define cycletimeBuzzer 500
#define defaultBaudRate 9600
#define seeedBTBaudRate 38400
#define seeedBTInitDelay 2000
//Seeed 6-Axis Accelerometer
#define ACCELE_SCALE 4  // accelerometer full-scale, should be 2, 4, or 8
/* LSM303 Address definitions */
#define LSM303_MAG  0x1E  // assuming SA0 grounded
#define LSM303_ACC  0x18  // assuming SA0 grounded
#define XAxis 0
#define YAxis 1
#define ZAxis 2
/* LSM303 Register definitions */
#define CTRL_REG1_A 0x20
#define CTRL_REG2_A 0x21
#define CTRL_REG3_A 0x22
#define CTRL_REG4_A 0x23
#define CTRL_REG5_A 0x24
#define HP_FILTER_RESET_A 0x25
#define REFERENCE_A 0x26
#define STATUS_REG_A 0x27
#define OUT_X_L_A 0x28
#define OUT_X_H_A 0x29
#define OUT_Y_L_A 0x2A
#define OUT_Y_H_A 0x2B
#define OUT_Z_L_A 0x2C
#define OUT_Z_H_A 0x2D
#define INT1_CFG_A 0x30
#define INT1_SOURCE_A 0x31
#define INT1_THS_A 0x32
#define INT1_DURATION_A 0x33
#define CRA_REG_M 0x00
#define CRB_REG_M 0x01//refer to the Table 58 of the datasheet of LSM303DLM
#define MAG_SCALE_1_3 0x20//full-scale is +/-1.3Gauss
#define MAG_SCALE_1_9 0x40//+/-1.9Gauss
#define MAG_SCALE_2_5 0x60//+/-2.5Gauss
#define MAG_SCALE_4_0 0x80//+/-4.0Gauss
#define MAG_SCALE_4_7 0xa0//+/-4.7Gauss
#define MAG_SCALE_5_6 0xc0//+/-5.6Gauss
#define MAG_SCALE_8_1 0xe0//+/-8.1Gauss
#define MR_REG_M 0x02
#define OUT_X_H_M 0x03
#define OUT_X_L_M 0x04
#define OUT_Y_H_M 0x07
#define OUT_Y_L_M 0x08
#define OUT_Z_H_M 0x05
#define OUT_Z_L_M 0x06
#define SR_REG_M 0x09
#define IRA_REG_M 0x0A
#define IRB_REG_M 0x0B
#define IRC_REG_M 0x0C
// Messages
const String deviceName = "Sentinal-";
const String deviceID = "1234";
const String softwareVersion = ". Firmware Version 0.3.0";
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
const String messageContentDelimiter = ",";
const String messageTerminator = "\n";
// States
enum condition {Normal, Warning, Alert, Canceled};
volatile condition conditionCode;
enum reason {Gas, Fall, Panic};
volatile reason reasonCode;
// Misc
SoftwareSerial serialBT(pinBlueToothRX, pinBlueToothTX);
Timer timerCountdown;
Timer timerPulse;
int idTimerCountdown;
int idTimerPulse;
int accel[3];  // we'll store the raw acceleration values here
int mag[3];  // raw magnetometer values stored here
float realAccel[3];  // calculated acceleration values here
//---------------------------------------------------------------------------------------------------------------------------------------
void setup() {
  Serial.begin(defaultBaudRate);
  if (TEST_BOARD) {
    setupBlueToothConnection();
    //serialBT.begin(defaultBaudRate);
  }
  else
  {
    delay(2000);
  }
  SendCustomMessage(messageDelimiter + messageON + messageDelimiter +
  messageOK + messageDelimiter + deviceName + deviceID + softwareVersion);  
  pinMode(pinPanic,INPUT);
  pinMode(pinLEDAlert,OUTPUT);
  pinMode(pinLEDWarning,OUTPUT);
  pinMode(pinLEDOn,OUTPUT);
  pinMode(pinBuzzer,OUTPUT);
  pinMode(pinBlueToothTX,OUTPUT);
  pinMode(pinBlueToothRX,INPUT);
  if (!TEST_BOARD) {
    pinMode(pinAccelSG,OUTPUT); 
    pinMode(pinTilt,INPUT);
    pinMode(pinGasDetect,INPUT);
    attachInterrupt(0, PanicButtonPressed, HIGH);
    attachInterrupt(1, CancelButtonPressed, HIGH);
  }  
  Wire.begin();  // Start up I2C, required for LSM303 communication
  initLSM303(ACCELE_SCALE);  // Initialize the LSM303, using a SCALE full-scale range
  ResetState();
}
//---------------------------------------------------------------------------------------------------------------------------------------
void loop() {
  if (conditionCode == Normal) {
   if (SampleSensors()) {
     if (reasonCode == Gas) {
       RaiseAlert();
     }
     else RaiseWarning();
   }
  }
  if (conditionCode != Normal) {
    timerCountdown.update();
    timerPulse.update();
  }
}


