﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models
{
    public class OrderHeader
    {

        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        [ValidateNever][ForeignKey("ApplicationUserId")] public ApplicationUser ApplicationUser { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ShippingOrder { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime PaymentDueDate { get; set; }

        public double OrderTotal { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }
        public string? SessionId {  get; set; }
        public string? PaymentIntetId { get; set; }

        [Required] public string? PhoneNumber { get; set; }
        [Required] public string? StreetAddress { get; set; }
        [Required] public string? City { get; set; }
        [Required] public string? State { get; set; }
        [Required] public string? PostalCode { get; set; }
        [Required] public string Name { get; set; }

    }
}
