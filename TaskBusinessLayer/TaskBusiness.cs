using System;
using System.Collections.Generic;
using System.Linq;
using TaskDataAccessLayer;
using AutoMapper;

namespace TaskBusinessLayer
{
    public class TaskBusiness : IDisposable
    {
        private readonly TaskManagementDatabaseEntities dbcontext;
        private readonly IMapper mapper;

        public TaskBusiness(TaskManagementDatabaseEntities dbcontext)
        {
            this.dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));

            // AutoMapper Configuration
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Task, TaskDTO>();
                cfg.CreateMap<Status, StatusDTO>();
                cfg.CreateMap<UserInfo, UserInfoDTO>();
                cfg.CreateMap<UserTaskDTO, TaskDTO>();
                cfg.CreateMap<TaskDTO, Task>();

            });

            mapper = config.CreateMapper();
        }

        public List<TaskDTO> GetAllTasks()
        {
            var tasks = dbcontext.Tasks.ToList();
            return mapper.Map<List<TaskDTO>>(tasks);
        }
        public TaskDTO GetTaskById(int taskid)
        {
            var task = dbcontext.Tasks.SingleOrDefault(t => t.Id == taskid);
            return mapper.Map<TaskDTO>(task);
        }

        public bool AddTask(TaskDTO task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            try
            {
                dbcontext.sp_InsertTask(task.Title,task.Description,task.DueDate,task.StatusId,task.UserId);
                dbcontext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public bool UpdateTask(TaskDTO task)
        {
            try
            {
                dbcontext.sp_UpdateTask(task.Id, task.Title, task.Description, task.DueDate, task.StatusId, task.UserId);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



        public bool DeleteTask(int taskid)
        {
            try
            {
                var task = dbcontext.Tasks.SingleOrDefault(t => t.Id == taskid);
                dbcontext.Tasks.Remove(task);
                dbcontext.SaveChanges();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public void Dispose()
        {
            dbcontext.Dispose();
        }
    }
}
