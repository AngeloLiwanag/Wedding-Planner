using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace WeddingPlanner.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId {get;set;}

        [Required]
        public int UserId {get;set;}

        [Required]
        [Display(Name="bride")]
        [MinLength(2, ErrorMessage="Bride name must be 2 characters or longer!")]
        public string Bride {get;set;}

        [Required]
        [Display(Name="groom")]
        [MinLength(2, ErrorMessage="Groom name must be 2 characters or longer!")]
        public string Groom {get;set;}

        [Required]
        [Display(Name="date")]
        public DateTime? Date {get;set;}

        [Required]
        [Display(Name="address")]
        public string Address {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now; 
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

        // ---- Many to Many ----
        public List<RSVP> PeopleList {get;set;}

        // ---- One to Many ----   
        public User Creator {get;set;}
    }
}