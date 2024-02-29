using System.Collections.Generic;
using TaskBusinessLayer;

public class UserTaskDTO
{
    public int UserId { get; set; }
    public string UserName { get; set; }
    public List<RequiredUserTaskDTO> TaskData { get; set; }
}
    