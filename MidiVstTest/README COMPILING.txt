UPDATED: For VSTi's except sylenth it looks like the latest version of Vst.Net and Naudio works well!


The only way I got this to work properly is to make sure the compiler is set to 
.NET Framework 3.5

Note!
Don't use the latest VST.NET library from https://vstnet.codeplex.com/
I only get lots of errors. 
(e.g. using the Sylenth plugin crashes when calling MainsChanged=True with
"System.AccessViolationException: Attempted to read or write protected memory. 
This is often an indication that other memory is corrupt.)"
Using the libraries from microDRUM instead.
https://github.com/microDRUM/md-config-tool


In theory a higher versioned framework and an .app file with the following should work,
but the app is unstable.
<?xml version="1.0"?>
<configuration>
  <startup useLegacyV2RuntimeActivationPolicy="true">
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.0"/>
  </startup>
</configuration>

