﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keyfactor.AnyGateway.DigiCertSym.Client.Models
{
    public static class DigiCertRevokeReasons
    {
        public const string KeyCompromise = "key_compromise";
        public const string AffiliationChanged = "affiliation_changed";
        public const string CessationOfOperation = "cessation_of_operation";
        public const string Superseded = "superseded";
    }
}
