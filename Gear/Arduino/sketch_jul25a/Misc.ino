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
       return messageNormal;
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

