//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TaskDataAccessLayer
{
    using System;
    
    public partial class sp_GetTaskById_Result
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public Nullable<int> StatusId { get; set; }
        public int UserId { get; set; }
    }
}