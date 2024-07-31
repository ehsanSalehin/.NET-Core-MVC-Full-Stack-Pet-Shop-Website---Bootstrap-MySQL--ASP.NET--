using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models
{
    public class Product
    {
        [Key] public int Id { get; set; }
        [DisplayName("Category Name")][MaxLength(30)][Required] public string Title { get; set; }
        public string Description { get; set; }
        [Required]public string Brand { get; set; }
        [Required]public string Weight { get; set; }
        [Required]public string Type { get; set; }
        [Required][Display(Name ="List Price")][Range(1, 200)] public double ListPrice { get; set; }
        [Required][Display(Name ="1-50")][Range(1, 200)] public double Price { get; set; }
        [Required][Display(Name ="50-100")][Range(1, 200)] public double Price50 { get; set; }
        [Required][Display(Name ="100+")][Range(1, 200)] public double Price100 { get; set; }
        public int CategoryId { get; set; }
       
        [ValidateNever][ForeignKey("CategoryId")]public Category? Category { get; set; } //define CategoryId is a foreign key

        [ValidateNever] public string Image { get; set; }


    }
}
