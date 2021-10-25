using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Keyfactor.AnyGateway.DigiCertSym.Client.Models;
using Newtonsoft.Json.Linq;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            JObject jsonTemplate = JObject.Parse(File.ReadAllText(@"C:\Users\bhill\source\repos\digicertsym-cagateway\Templates\FAA-StandardRequest.json"));

            var request = BuildEnrollmentRequest(jsonTemplate);
        }


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
    }
}
