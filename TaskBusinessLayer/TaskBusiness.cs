using System;
using System.Collections.Generic;
using System.Linq;
using TaskDataAccessLayer;

namespace TaskBusinessLayer
{
    public class TaskBusiness : IDisposable
    {
        private readonly TaskManagementDatabaseEntities dbcontext;
        public bool status;
        public TaskBusiness(TaskManagementDatabaseEntities dbcontext)
        {
            this.dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            status = false;
        }

        public List<Task> GetAllTasks()
        {
            return dbcontext.Tasks.ToList();
        }

        public TaskDTO GetTaskById(int taskid)
        {
            try
            {
                var task = dbcontext.Tasks.Where(t => t.Id == taskid).SingleOrDefault();

                if (task == null)
                {
                    return null;
                }

                var taskdata = new TaskDTO
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    DueDate = task.DueDate,
                    StatusId = task.StatusId,
                    UserId = task.UserId
                };
                return taskdata;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool AddTask(TaskDTO task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            dbcontext.sp_InsertTask(
                task.Title,
                task.Description,
                task.DueDate,
                task.StatusId,
                task.UserId
            );
            dbcontext.SaveChanges();
            status = true;
            return status;
        }


        public bool UpdateTask(TaskDTO task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            try
            {
                dbcontext.sp_UpdateTask(
                    task.Id,
                    task.Title,
                    task.Description,
                    task.DueDate,
                    task.StatusId,
                    task.UserId
                );
                status = true;
                return status;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to update Task with ID {task.Id}. Error: {ex.Message}", ex);
            }
        }
        public bool DeleteTask(int taskid)
        {
            var task = dbcontext.Tasks.Where(t => t.Id == taskid).SingleOrDefault();

            if (task != null)
            {
                dbcontext.sp_DeleteTaskById(taskid);
                dbcontext.SaveChanges();
                status = true;
                return status;
            }
            return status;
        }
        public List<Status> GetAllStatuses()
        {
            return dbcontext.Status.ToList();
        }
        public void Dispose()
        {
            dbcontext.Dispose();
        }
    }
}
