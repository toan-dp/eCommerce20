using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.DomainModels
{
    [Table("AspNetUsers")]
    public class Customer: IdentityUser
    {
        public override string Id { get; set; }
        public string FullName { get; set; }
    }
}
