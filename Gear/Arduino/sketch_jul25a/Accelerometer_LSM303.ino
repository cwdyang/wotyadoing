void PrintXYZValues() {
 
  char buff[6];
  
  //if (!digitalRead(pinPanic)) return
  getLSM303_accel(accel);  // get the acceleration values and store them in the accel array
  
  SendRawData(dtostrf((accel[XAxis] / pow(2, 15) * ACCELE_SCALE),1,2,buff));
  SendRawData(messageContentDelimiter);
  SendRawData(dtostrf((accel[YAxis] / pow(2, 15) * ACCELE_SCALE),1,2,buff));
  SendRawData(messageContentDelimiter);
  SendRawData(dtostrf((accel[ZAxis] / pow(2, 15) * ACCELE_SCALE),1,2,buff));
  SendRawData(messageTerminator);
}
//---------------------------------------------------------------------------------------------------------------------------------------
void initLSM303(int fs)
{
  LSM303_write(0x27, CTRL_REG1_A);  // 0x27 = normal power mode, all accel axes on
  if ((fs==8)||(fs==4))
    LSM303_write((0x00 | (fs-fs/2-1)<<4), CTRL_REG4_A);  // set full-scale
  else
    LSM303_write(0x00, CTRL_REG4_A);
  LSM303_write(0x14, CRA_REG_M);  // 0x14 = mag 30Hz output rate
  LSM303_write(MAG_SCALE_1_3, CRB_REG_M); //magnetic scale = +/-1.3Gauss
  LSM303_write(0x00, MR_REG_M);  // 0x00 = continouous conversion mode
}
//---------------------------------------------------------------------------------------------------------------------------------------

//---------------------------------------------------------------------------------------------------------------------------------------

//---------------------------------------------------------------------------------------------------------------------------------------
void getLSM303_accel(int * rawValues)
{
  rawValues[ZAxis] = ((int)LSM303_read(OUT_X_L_A) << 8) | (LSM303_read(OUT_X_H_A));
  rawValues[XAxis] = ((int)LSM303_read(OUT_Y_L_A) << 8) | (LSM303_read(OUT_Y_H_A));
  rawValues[YAxis] = ((int)LSM303_read(OUT_Z_L_A) << 8) | (LSM303_read(OUT_Z_H_A));
  // had to swap those to right the data with the proper axis
}
//---------------------------------------------------------------------------------------------------------------------------------------
byte LSM303_read(byte address)
{
  byte temp;
  
  if (address >= 0x20)
    Wire.beginTransmission(LSM303_ACC);
  else
    Wire.beginTransmission(LSM303_MAG);
    
  Wire.write(address);
  
  if (address >= 0x20)
    Wire.requestFrom(LSM303_ACC, 1);
  else
    Wire.requestFrom(LSM303_MAG, 1);
  while(!Wire.available())
    ;
  temp = Wire.read();
  Wire.endTransmission();
  
  return temp;
}
//---------------------------------------------------------------------------------------------------------------------------------------
void LSM303_write(byte data, byte address)
{
  if (address >= 0x20)
    Wire.beginTransmission(LSM303_ACC);
  else
    Wire.beginTransmission(LSM303_MAG);
    
  Wire.write(address);
  Wire.write(data);
  Wire.endTransmission();
}
