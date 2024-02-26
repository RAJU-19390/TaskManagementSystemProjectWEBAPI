using System;
namespace TaskBusinessLayer
{
    public class UserTaskDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int TaskId { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public string StatusMode { get; set; }
    }
}
