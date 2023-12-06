﻿using System.ComponentModel.DataAnnotations;

namespace RoomReservations.Models
{
    public class Room
    {
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public int Capacity { get; set; }
        [Required]
        public decimal PricePerNight { get; set; }
        public string? Location { get; set; }
        
        public string? ImageUrl { get; set; }


    }
}