
using System;

namespace TaskBusinessLayer
{
    internal class Task
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public int? StatusId { get; set; }
        public int UserId { get; set; }
    }
}