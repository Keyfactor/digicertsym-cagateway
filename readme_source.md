# Getting Started
## Standard Gateway Installation
To begin, you must have the CA Gateway Service installed and operational before attempting to configure the DigiCertSym mPKI plugin. This integration was tested with Keyfactor 9.1.0.0.
To install the gateway follow these instructions.

1) **Gateway Server** - Get the latest gateway .msi installer from Keyfactor and run the installation on the gateway server.

2) **Gateway Server** - If you have the rights to install the database (usually in a Non SQL PAAS Environment) Using Powershell, run the following command to create the gateway database.

   **SQL Server Windows Auth**
    ```
    %InstallLocation%\DatabaseManagementConsole.exe create -s [database server name] -d [database name]
    ```
   Note if you are using SQL Authentication, then you need to run
   
   **SQL Server SQL Authentication**

   ```
   %InstallLocation%\DatabaseManagementConsole.exe create -s [database server name] -d [database name] -u [sql user] -p [sql password]
   ```

   ## Populate commands below

   **Windows Authentication**

   ```
   %InstallLocation%\DatabaseManagementConsole.exe populate -s [database server name] -d [database name]
   ```

   **SQL Server SQL Authentication** 

   ```
   %InstallLocation%\DatabaseManagementConsole.exe populate -s [database server name] -d [database name] -u [sql user] -p [sql password]
   ```

3) **Gateway Server** - run the following Powershell to import the Cmdlets

   C:\Program Files\Keyfactor\Keyfactor AnyGateway\ConfigurationCmdlets.dll (must be imported into Powershell)
   ```ps
   Import-Module C:\Program Files\Keyfactor\Keyfactor AnyGateway\ConfigurationCmdlets.dll
   ```

4) **Gateway Server** - Run the Following Powershell script to set the gateway encryption cert

   ### Set-KeyfactorGatewayEncryptionCert
   This cmdlet will generate a self-signed certificate used to encrypt the database connection string. It populates a registry value with the serial number of the certificate to be used. The certificate is stored in the LocalMachine Personal Store and the registry key populated is:

   HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\CertSvcProxy\Parameters\EncryptSerialNumber
   No parameters are required to run this cmdlet.

5) **Gateway Server** - Run the following Powershell Script to Set the Database Connection

   ### Set-KeyfactorGatewayDatabaseConnection
   This cmdlet will set and encrypt the database connection string used by the AnyGateway service. 

   **Windows Authentication**
   ```ps
   Set-KeyfactorGatewayDatabaseConnection -Server [db server name] -Database [database name]
   ```

   **SQL Authentication**
   ```ps
   $KeyfactorCredentials = Get-Credentials
   Set-KeyfactorGatewayDatabaseConnection -Server [db server name] -Database [database name] -Account [$KeyfactorCredentials]
   ```
## Standard Gateway Configuration Finished
---


## DigiCertSym mPKI Global AnyGateway Specific Configuration
It is important to note that importing the DigiCertSym mPKI configuration into the CA Gateway prior to installing the binaries must be completed. Additionally, the CA Gateway service
must be running in order to succesfully import the configuation. When the CA Gateway service starts it will attempt to validate the connection information to 
the CA.  Without the imported configuration, the service will fail to start.

### Binary Installation

1) Get the Latest Zip File from [Here](https://github.com/Keyfactor/digicertsym-cagateway/releases)
2) **Gateway Server** - Copy the DigiCertSymProxy.dll and DigiCertSymProxy.dll.config to the location where the Gateway Framework was installed (usually C:\Program Files\Keyfactor\Keyfactor AnyGateway)

### Configuration Changes
1) **Gateway Server** - Edit the CAProxyServer.exe.config file and replace the line that says "NoOp" with the line below:
   ```
	<alias alias="CAConnector" type="Keyfactor.AnyGateway.DigiCertSym.DigiCertSymProxy, DigiCertSymProxy"/>
   ```
2) **Gateway Server** - Install the Root Digicert Certificate that was found in the "Manage CAs" Settings Menu in the Digicert mPKI Portal

3) **Gateway Server** - Install the Intermediate Digicert Certificate that was found in the "Manage CAs" Settings Menu in the Digicert mPKI Portal

4) **Gateway Server** - Take the sample Config.json located [Here](https://github.com/Keyfactor/digicertsym-cagateway/raw/main/SampleConfig.json) and make the following modifications

- *Security Settings Modifications* (Swap this out for the typical Gateway Security Settings for Test or Prod)

```
  "Security": {
    "KEYFACTOR\\administrator": {
      "READ": "Allow",
      "ENROLL": "Allow",
      "OFFICER": "Allow",
      "ADMINISTRATOR": "Allow"
    },
    "KEYFACTOR\\SVC_AppPool": {
      "READ": "Allow",
      "ENROLL": "Allow",
      "OFFICER": "Allow",
      "ADMINISTRATOR": "Allow"
    },
    "KEYFACTOR\\SVC_TimerService": {
      "READ": "Allow",
      "ENROLL": "Allow",
      "OFFICER": "Allow",
      "ADMINISTRATOR": "Allow"
    }
```
- **Digicert mPKI Environment Settings** (Modify these with the production keys and Urls obtained from your private mPKI portal) 

1) **DigiCertSymUrl** - Prod or rest Url to the DigiCertSym mPKI Api
2) **ApiKey** - Key generated from the DigiCertSym mPKI API Settings section
3) **KeyfactorApiUserId** - User in Keyfactor with access to Keyfactor API for REST API Calls to Keyfactor
4) **KeyfactorApiPassword** - Password for user in Keyfactor with access to Keyfactor API for REST API Calls to Keyfactor
5) **KeyfactorApiUrl** - URL for Keyfactor API for REST API Calls to Keyfactor
6) **SeatList** - Comma Separated list of Seats to inventory for the Gateway inventory process
7) **ConstantNames** - These were made configurable because the digicert API expects these to be named differently depending on the setup.  Try the default values first and they can be adjusted if errors occur.
8) **ClientCertificateLocation** - This is for the SOAP Inventory as explained in the SOAP Inventory Setup section.  This is the location of the pfx to use for the client certificate.
9) **ClientCertificatePassword** - This is for the SOAP Inventory as explained in the SOAP Inventory Setup section.  This is the password for the PFX file to use for the client certificate.
8) **EndpointAddress** - This is for the SOAP Inventory as explained in the SOAP Inventory Setup section.  This is endpoint address for the SOAP API.  You will want a differnt value than the test version in production.
9) **OuStartPoint** - This was made configurable for clients that have more than one Organizational Units.  If there are 2 existing, this should be set to 2.

```
	"CAConnection": {
		"DigiCertSymUrl": "https://pki-ws-rest.symauth.com/mpki/api/v1",
		"ApiKey": "01cb64eba8173b53a9_E2FEF2DB64730C9332B964104E2248CEA05C7D8A6F2BBE02CD535DD51FA78B2E",
		"KeyfactorApiUserId": "Keyfactor\\Administrator",
		"KeyfactorApiPassword": "Password1",
		"KeyfactorApiUrl": "https://kftrain.keyfactor.lab/KeyfactorAPI",
		"DnsConstantName": "dnsName",
		"UpnConstantName": "otherNameUPN",
		"IpConstantName": "san_ipAddress",
		"EmailConstantName": "mail_email",
		"ClientCertificateLocation": "C:\\Program Files\\Keyfactor\\Keyfactor AnyGateway\\KeyfactorMPki.pfx",
		"ClientCertificatePassword": "SomePassword!",
		"EndpointAddress": "https://pki-ws.symauth.com/pki-ws/certificateManagementService",
		"OuStartPoint":2
	}
```

- **Template Settings**
1) **ProductID** - OID for profile generated in Digicert mPKI
2) **EnrollmentTemplate** - Template JSON used to generate a enrollment request explained later in this document
```
	"Templates": {
		"Microsoft Wi-Fi (Test Drive)": {
			"ProductID": "2.16.840.1.113733.1.16.1.2.3.6.1.1266772938",
			"Parameters": {
				"EnrollmentTemplate": "Wifi-StandardRequest.json"
			}
		},
		"Private Server certificates (Test Drive)": {
			"ProductID": "2.16.840.1.113733.1.16.1.5.2.5.1.1266771486",
			"Parameters": {
				"EnrollmentTemplate": "PriverServer-StandardRequest.json"
			}
		},
		"FAA-TEST-WS-INTERNAL": {
			"ProductID": "2.16.840.1.113733.1.16.1.5.2.5.1.1280209757",
			"Parameters": {
				"EnrollmentTemplate": "FAA-StandardRequest.json"
			}
		}
	}
```

- **Enrollment Templates**
Since there are infinate number of profile configurations in DigiCertSym mPKI, these tempates are used to shell out the request for each profile and during the enrollment process will be replaced with data from the Enrollment request in Keyfactor.

These tempates files must be copied into the same directory as the Gateway binaries and saved as a JSON file with the same name outlined in the tempates section above.

Sample Enrollment Template is [here](https://github.com/Keyfactor/digicertsym-cagateway/raw/main/FAA-StandardRequest.json)

Enrollment Format Specifications Located [here](https://pki-ws-rest.symauth.com/mpki/docs/index.html) and [here](https://pki-ws-rest.symauth.com/mpki/docs/index.html)

1) **EnrollmentParam** - Below is a sample Enrollment Template where anything Prefixed with "EnrollmentParam|FieldName" will be replaced with an enrollment field value from the Keyfactor portal during enrollment. 
2) **CSR|RAW** - Below is a sample Enrollment Template where anything Prefixed with "CSR|RAW" will be replaced with the raw CSR content from the enrollment request from Keyfactor Portal. 
3) **CSR|CSRContent** - Below is a sample Enrollment Template where anything Prefixed with "CSR|CSRContent" will be replaced with the CSR content from the enrollment request from Keyfactor Portal. 

```
{
	"profile": {
		"id": "2.16.840.1.113733.1.16.1.5.2.5.1.1280209757"
	},
	"seat": {
		"seat_id": "EnrollmentParam|Seat"
	},
	"csr": "CSR|RAW",
	"validity": {
		"unit": "years",
		"duration": "Numeric|EnrollmentParam|Validity (Years)|Numeric"
	},
	"attributes": {
		"common_name": "CSR|CN",
		"country": "CSR|C",
		"organization_name": "CSR|O"
	}
}
```

4) **Sample Mapping Below**
![](Images/SampleMapping.gif)


- **Gateway Settings**
```
  "CertificateManagers": null,
  "GatewayRegistration": {
    "LogicalName": "DigiCertSym",
    "GatewayCertificate": {
      "StoreName": "CA",
      "StoreLocation": "LocalMachine",
      "Thumbprint": "7c0d887f94559bb151d399e978aa89e9179cf1ad"
    }
  }
```

- **Service Settings** (Modify these to be in accordance with Keyfactor Standard Gateway Production Settings)
```
  "ServiceSettings": {
    "ViewIdleMinutes": 1,
    "FullScanPeriodHours": 1,
    "PartialScanPeriodMinutes": 1
  }
```

5) **Gateway Server** - Save the newly modified config.json to the following location "C:\Program Files\Keyfactor\Keyfactor AnyGateway"

### Template Installation

1) **Command Server** - Install a tempate into Active Directory to match each profile that you want to integrate with in DigiCertSym mPKI

### SOAP Inventory Setup

The Digicert mPKI REST API does not support inventory so the SOAP API is required to inventory all of the certs for the profiles listed in config.json file.
In order to use the SOAP API, you need a client certificate from the Digicert mPKI Portal.  The steps to obtain a certfificate are outlined in the documentation
listed [here](https://knowledge.digicert.com/content/dam/digicertknowledgebase/attachments/pki-platform/soap-api-client-package/pki-web-services-developers-guide.pdf).

1) Follow the instructions in section 2.6.1 of the above document.
2) Export the keystore to a PFX file with a similar command that is listed below:
```keytool -importkeystore -srckeystore KeyfactorMPki.jks -srcstoretype JKS -destkeystore KeyfactorMPki3.pfx -deststoretype PKCS12```
3) Import the PFX Certificate to the computer it was generated on.
4) Export the PFX to a file from that same machine's certificate store and copy it to the same directory where the config.json is located.

Sample Commands for a Test Envrionment are below:
```
keytool -genkey -alias pki_ra -keyalg RSA -keysize 2048 -sigalg SHA256withRSA -dname "CN=pki_ra" -keypass SomePassword -keystore KeyfactorMPki3 -storepass SomePassword

keytool -certreq -alias pki_ra -sigalg SHA256withRSA -file pki_raCSR.req -keypass SomePassword -keystore KeyfactorMPki2 -storepass SomePassword

keytool -import -alias pki_ra -file cert.p7b -noprompt -keypass SomePassword -keystore KeyfactorMPki2 -storepass SomePassword

keytool -import -trustcacerts -alias pki_ca -file SYMC_Test_Drive_RA_Intermediate_CA.cer -keystore KeyfactorMPki2 -storepass SomePassword

keytool -import -trustcacerts -alias root -file SYMC_Managed_PKI_Infrastructure_Test_Drive_Root.cer -keystore KeyfactorMPki2 -storepass SomePassword

keytool -importkeystore -srckeystore KeyfactorMPki.jks -srcstoretype JKS -destkeystore KeyfactorMPki2.pfx -deststoretype PKCS12
```

### Certificate Authority Installation
1) **Gateway Server** - Start the Keyfactor Gateway Service
2) Run the set Gateway command similar to below
```ps
Set-KeyfactorGatewayConfig -LogicalName "DigiCertSym" -FilePath [path to json file] -PublishAd
```
3) **Command Server** - Import the certificate authority in Keyfactor Portal 


***

### License
[Apache](https://apache.org/licenses/LICENSE-2.0)
