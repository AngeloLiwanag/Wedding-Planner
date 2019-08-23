using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace WeddingPlanner.Models
{
    public class User
    {
        [Key]
        public int UserId {get;set;}

        [Required]
        [Display(Name="first name")]
        [MinLength(2, ErrorMessage="First name must be 2 characters or longer!")]
        public string FirstName {get;set;}

        [Required]
        [Display(Name="last name")]
        [MinLength(2, ErrorMessage="Last name must be 2 characters or longer!")]
        public string LastName {get;set;}

        [Required]
        [EmailAddress]
        [Display(Name="email")]
        public string Email {get;set;}

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage="Password must be 8 charaters or longer!")]
        [Display(Name="password")]
        public string Password {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now; 
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

        // ---- Many to Many ----
        public List<RSVP> WeddingList {get;set;}

        // ---- One to Many ----
        public List<Wedding> CreatedWeddings {get;set;}

        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string Confirm {get;set;}
    }
}