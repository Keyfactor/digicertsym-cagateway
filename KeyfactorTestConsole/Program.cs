using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CAProxy.AnyGateway.Models;
using Keyfactor.AnyGateway.DigiCertSym.Client.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;

namespace KeyfactorTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            EnrollmentProductInfo productInfo= new EnrollmentProductInfo();

            productInfo.ProductParameters.Add("Seat","KeyfactorPortal");
            productInfo.ProductParameters.Add("Validity (Years)","1");

            string csr = "MIIExjCCAq4CAQAwgYAxCzAJBgNVBAYTAlVTMR4wHAYDVQQDDBV3d3cuZmx5aW5nc2F1Y2Vycy5jb20xEjAQBgNVBAcMCUNsZXZlbGFuZDEMMAoGA1UECgwDRkFBMQ0wCwYDVQQIDARPaGlvMSAwHgYDVQQLDBdJVCBEZXBhcnRtZW50L01hcmtldGluZzCCAiIwDQYJKoZIhvcNAQEBBQADggIPADCCAgoCggIBAMsCyAxwkM+rczVSZjmr6UoQ518ImGtcluIwTczIuCOCx7K1JPo3otJzZ7vzFcxXfXV+M7Zdhobqm9U+kprKW/QL9m7oY9j/+ZUoFq/Q4MgT3ukoKylFu5XxIDs5gwg3/2aMDUv84moWOReiSCjjOXG3qVfjOYn6RUfNW89a7H2z/A2Wd0CJHq2vyk3LDwSa3GyuBTttPuLbv3WIm82/DthJ0Mn9Hw9+PCLQ6TOefkNTubcS4qjiJ75hkIcn7y7uH6pfD3Bt7LGellvwwjUM/2F5tbtkYlOxVkixMcjFRQ6w/5YlGM5KTBvqUDxQdhxHeM6PiW1vVZUMqv1aUvhT1tDBG7JJnxbL7fpW++IODzHqAiy2mcyaJ/IBFMN4i6ngKP4nvYGn3fMatbdoQDDp9oUXSIuqPY/BdYZAQoAcMul9Sme4MzBAosfAWoYCM8czLpTWgrnhCWft4nVGwmbfKX353gp9irToh02kObqwvNrHp1LIL4FgOBOa6kkxpmnb7/i7TLF/CHhVeoKkZpguUqqJeQlEKj0Y/kFeMapiHPfgdvhXJRSoTCOOYoSamr85kxsXLJ4emIrOyECSwssVadVvznWvfkBj9ESDHr0aFghJpT7kbpm4ezeH627+d41NmqVksIl1e9vV30RsjmCmCqhfAKMGUSb40ne17k3S0V4tAgMBAAGgADANBgkqhkiG9w0BAQsFAAOCAgEABm7fyj6tjSk2N+X9Fz+FLNB23bzwkeTclNS8sZiHLi9mHhyjpLmbGDoVwvxpt3v0ySkaEhUnO15fMQkF8JIBcl4DomzaT/tNA3AyKMQVZ4lZRjBAG8zln/HSU9NMUtrtgoIx1SCApmeFEZ8hq3KMDalt99lHTLWmENgU3keHfIpsBPKdLmUffCl7ZDbTPtqcZfpJ/hhG5Dc8OyLXI5wobvgiSPAPG48Qta6kSAXE7HqSLuHkDwC0uM0RcM7Xj0lBLGJz39q8I+rcQXw53qjSguCO65//e1xo1JwwVj1CerZ4TE3WXK/N26FO5QV72Zt4FgwPcJeI7JHYBEayDq/wLXnQ3nWilDX+SR6PFtP2Ll9eGgkKP7Lb1BY/mGxvFi0OW9aSG7zWrfHKSmgDozYA9+uDBVypMAAHwTh5I3MBAN1HqUxVSOBjaGPNfekmlSJU58rvYycy/frYMSr/hV+Qy6qQoHLV7pmP3Sv+2jfByFlR9g4OmberIGJNrW1dsTA2jkd37ypTe0QQwKjTEXgW7Sk3cecC3XcwL+mWOEqXciTpMWxte33gO4wNZD1b8mSXo75xWAp6SP5MV4fNJ+rWLxZrIvv5X3bmrD03Q9vQezmebImF13gxG+Edi0KkGa3vpcIcExj4viTcHlmS4j4LLYu+dNd5MXVGoHoHOjoFC9M=";

            var pemCert = Pemify(csr);
            pemCert = "-----BEGIN CERTIFICATE REQUEST-----\n" + pemCert;
            pemCert += "\n-----END CERTIFICATE REQUEST-----";

            string[] dnsNames = { "www.cnn.com", "www.yahoo.com"};
            Dictionary<string, string[]> san = new Dictionary<string, string[]> {{"DNS Name", dnsNames}};

            var req = new EnrollmentRequest();
            var sn = new San();

            CertificationRequestInfo csrParsed;

            using (TextReader sr = new StringReader(pemCert))
            {
                var reader = new PemReader(sr);
                var cReq = reader.ReadObject() as Pkcs10CertificationRequest;
                csrParsed = cReq?.GetCertificationRequestInfo();
            }

            JObject jsonTemplate = JObject.Parse(File.ReadAllText(@"C:\Users\bhill\source\repos\digicertsym-cagateway\Templates\FAA-StandardRequest.json"));

            var jsonResult = jsonTemplate.ToString();

            //1. Loop through list of Product Parameters and replace in JSON
            foreach (var productParam in productInfo.ProductParameters)
            {
                jsonResult= ReplaceProductParam(productParam,jsonResult);
            }
            //Clean up the Numeric values remove double quotes
            jsonResult = jsonResult.Replace("\"Numeric|","");
            jsonResult = jsonResult.Replace("|Numeric\"", "");


            //2. Loop though list of Parsed CSR Elements and replace in JSON
            var csrValues = csrParsed?.Subject.ToString().Split(',');
            if (csrValues != null)
                foreach (var csrValue in csrValues)
                {
                    var nmValPair = csrValue.Split('=');
                    jsonResult = ReplaceCsrEntry(nmValPair, jsonResult);
                }

            //3. Replace the RAW CSR content
            jsonResult=jsonResult.Replace("CSR|RAW",csr);

            //4. Deserialize Back to EnrollmentRequest
            var enrollmentRequest = JsonConvert.DeserializeObject<EnrollmentRequest>(jsonResult);

            //5. Loop through SANS and replace in Object
            var dnsList = new List<DnsName>();
            var dnsKp = san["DNS Name"];
            foreach (var item in dnsKp)
            {
                DnsName dns = new DnsName {Value = item};
                dnsList.Add(dns);
            }

            //6. Loop through OUs and replace in Object
            var organizationalUnits = GetValueFromCsr("OU", csrParsed).Split('/');
            
            var orgUnits=new List<OrganizationUnit>();
            var i = 1;
            foreach (var ou in organizationalUnits)
            {
                var organizationUnit = new OrganizationUnit {Id = "cert_org_unit" + i,Value=ou};
                orgUnits.Add(organizationUnit);
                i++;
            }

            var attributes = enrollmentRequest.Attributes;
            attributes.OrganizationUnit = orgUnits;
            sn.DnsName = dnsList;
            attributes.San = sn;
            enrollmentRequest.Attributes = attributes;

        }

        private static string ReplaceProductParam(KeyValuePair<string,string> productParam,string jsonResult)
        {
            return jsonResult.Replace("EnrollmentParam|" + productParam.Key, productParam.Value);
        }

        private static string ReplaceCsrEntry(string[] nameValuePair,string jsonResult)
        {
            string pattern = @"\b"+ "CSR\\|" + nameValuePair[0] + @"\b";
            string replace = nameValuePair[1];
            return Regex.Replace(jsonResult, pattern, replace);
            //return jsonResult.Replace("CSR|" + nameValuePair[0], nameValuePair[1]);
        }

        public static Func<string, string> Pemify = ss =>
            ss.Length <= 64 ? ss : ss.Substring(0, 64) + "\n" + Pemify(ss.Substring(64));

        static EnrollmentRequest BuildEnrollmentRequest(JObject jsonToParse)
        {
            foreach (var jToken in (JToken)jsonToParse)
            {
                var x = (JProperty)jToken; // if 'obj' is a JObject
                string name = x.Name;
                JToken value = x.Value;
            }


            return new EnrollmentRequest();
        }

        public static string GetValueFromCsr(string subjectItem, CertificationRequestInfo csr)
        {
            var csrValues = csr.Subject.ToString().Split(',');
            foreach (var val in csrValues)
            {
                var nmValPair = val.Split('=');

                if (subjectItem == nmValPair[0]) return nmValPair[1];
            }

            return "";
        }
    }
}
