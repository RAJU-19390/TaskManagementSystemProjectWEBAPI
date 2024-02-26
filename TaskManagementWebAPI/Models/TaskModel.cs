using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementWebAPI.Models
{
    public class TaskModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public Nullable<int> StatusId { get; set; }
        public int UserId { get; set; }

       // public virtual Status Status { get; set; }
       //public virtual UserInfo UserInfo{  get; set; }
    }
}
