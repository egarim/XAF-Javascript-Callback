alert('Hello XAFERS');
//in this case the name of the callback handler is Question which is the name of the file, in the CallbackController im registering the scripts call back with the name of the script file
RaiseXafCallback(globalCallbackControl, 'ShowAlert', 'false', '', false);