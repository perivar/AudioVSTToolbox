The only way I got this to work properly is to make sure the compiler is set to 
.NET Framework 3.5

- Allow unsafe code

Note!
Don't use the latest VST.NET library from https://vstnet.codeplex.com/
I only get lots of errors.
Using the libraries from microDRUM instead.
https://github.com/microDRUM/md-config-tool


In theory a higher versioned framework and an .app file with the following - but the app is unstable with lots of errors
<?xml version="1.0"?>
<configuration>
  <startup useLegacyV2RuntimeActivationPolicy="true">
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.0"/>
  </startup>
</configuration>
