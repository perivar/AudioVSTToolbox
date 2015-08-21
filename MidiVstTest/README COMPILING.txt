The only way I got this to work properly is to make sure the compiler is set to 
.NET Framework 3.5

- Allow unsafe code


In theory a higher versioned framework and an .app file with the following - but the app is unstable with lots of errors
<?xml version="1.0"?>
<configuration>
  <startup useLegacyV2RuntimeActivationPolicy="true">
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.0"/>
  </startup>
</configuration>
