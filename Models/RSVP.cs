using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace WeddingPlanner.Models
{
    public class RSVP
    {
        [Key]
        public int RSVPId {get;set;}
        public int UserId {get;set;}
        public int WeddingId {get;set;}
        public User User {get;set;}
        public Wedding Wedding {get;set;}
        public RSVP (){}
        public RSVP (int SessionId, int WedId)
        {
            UserId = SessionId;
            WeddingId = WedId;
        }
    }
}