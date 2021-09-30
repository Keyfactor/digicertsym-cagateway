Stop-Service -Name "CertSvcProxy"
Start-Sleep 5
Copy-Item "C:\Users\administrator.KEYFACTOR\source\repos\digicertsym-cagateway\DigiCertSymCaProxy\bin\Debug\DigiCertSymProxy.dll" -Destination "C:\Program Files\Keyfactor\Keyfactor AnyGateway"
Copy-Item "C:\Users\administrator.KEYFACTOR\source\repos\digicertsym-cagateway\DigiCertSymCaProxy\bin\Debug\DigiCertSymProxy.dll.config" -Destination "C:\Program Files\Keyfactor\Keyfactor AnyGateway"
Copy-Item "C:\Users\administrator.KEYFACTOR\source\repos\digicertsym-cagateway\DigiCertSymCaProxy\bin\Debug\DigiCertSymProxy.pdb" -Destination "C:\Program Files\Keyfactor\Keyfactor AnyGateway"
Start-Service -Name "CertSvcProxy"