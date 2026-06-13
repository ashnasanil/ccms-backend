using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CCMS.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace CCMS.Application.DTOs.Court
{
    public class CreateCaseDto
    {
        [Required]
        public string ComplainantName { get; set; }
        
        [Required]
        public string DefendantName { get; set; }
        
        [Required]
        public string AadhaarNumber { get; set; }
        
        [Required]
        public string PanNumber { get; set; }
        
        [Required]
        public string AccountNumber { get; set; }
        
        [Required]
        public string BankName { get; set; }
        
        [Required]
        public OrderType OrderType { get; set; }
        
        public decimal? FreezeAmount { get; set; }
        
        [Required]
        public IFormFile CourtOrderFile { get; set; }
        
        [Required]
        public IFormFile AadhaarCopyFile { get; set; }
        
        [Required]
        public IFormFile PanCopyFile { get; set; }
    }
}
