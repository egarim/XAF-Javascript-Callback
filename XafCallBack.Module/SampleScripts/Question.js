if (confirm('Some message')) {
    alert('Thanks for confirming');
    //in this case the name of the callback handler is Question which is the name of the file, in the CallbackController im registering the scripts call back with the name of the script file
    RaiseXafCallback(globalCallbackControl, 'Question', 'true', '', false);
} else {
    alert('Why did you press cancel? You should have confirmed');
    RaiseXafCallback(globalCallbackControl, 'Question', 'false', '', false);
}