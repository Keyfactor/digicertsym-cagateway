using System.Collections.Generic;
using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public class Attributes : IAttributes
    {
        [JsonProperty("common_name")] public string CommonName { get; set; }

        [JsonProperty("content_type")] public string ContentType { get; set; }

        [JsonProperty("counter_signature")] public string CounterSignature { get; set; }

        [JsonProperty("country")] public string Country { get; set; }

        [JsonProperty("custom_attributes")] public CustomAttributes CustomAttributes { get; set; }

        [JsonProperty("dn_qualifier")] public string DnQualifier { get; set; }

        [JsonProperty("domain_component")] public List<DomainComponent> DomainComponent { get; set; }

        [JsonProperty("domain_name")] public string DomainName { get; set; }

        [JsonProperty("email")] public string Email { get; set; }


        [JsonProperty("given_name")] public string GivenName { get; set; }

        [JsonProperty("ip_address")] public string IpAddress { get; set; }

        [JsonProperty("job_title")] public string JobTitle { get; set; }

        [JsonProperty("locality")] public string Locality { get; set; }

        [JsonProperty("message_digest")] public string MessageDigest { get; set; }

        [JsonProperty("organization_name")] public string OrganizationName { get; set; }

        [JsonProperty("organization_unit")] public List<OrganizationUnit> OrganizationUnit { get; set; }

        [JsonProperty("postal_code")] public string PostalCode { get; set; }

        [JsonProperty("pseudonym")] public string Pseudonym { get; set; }

        [JsonProperty("san")] public San San { get; set; }

        [JsonProperty("serial_number")] public string SerialNumber { get; set; }

        [JsonProperty("signing_time")] public string SigningTime { get; set; }

        [JsonProperty("state")] public string State { get; set; }

        [JsonProperty("street_address")] public List<StreetAddress> StreetAddress { get; set; }

        [JsonProperty("surname")] public string Surname { get; set; }

        [JsonProperty("unique_identifier")] public string UniqueIdentifier { get; set; }

        [JsonProperty("unstructured_address")] public string UnstructuredAddress { get; set; }

        [JsonProperty("unstructured_name")] public string UnstructuredName { get; set; }

        [JsonProperty("user_id")] public string UserId { get; set; }
    }
    
}