using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcIdentityServerCenter.Models
{
    public class ProcessConsentResult
    {
        public string RedirctUrl { get; set; }
        public bool IsRedirct => RedirctUrl != null;
        public ConsentViewModel ConsentViewModel { get; set; }
        public string ValidationError { get; set; }
    }
}
