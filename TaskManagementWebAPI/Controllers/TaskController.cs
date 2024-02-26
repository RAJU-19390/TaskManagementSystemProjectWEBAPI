using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using TaskBusinessLayer;
using TaskManagementWebAPI.Models;

namespace TaskManagementWebAPI.Controllers
{
    public class TaskController : ApiController
    {
        private readonly TaskBusiness tb;
        public TaskController()
        { 
            var dbcontext = new TaskDataAccessLayer.TaskManagementDatabaseEntities();
            this.tb = new TaskBusiness(dbcontext) ?? throw new ArgumentNullException(nameof(tb));
        }

        // GET api/task
        [HttpGet]
        [Route("api/task")]
        public IHttpActionResult GetTasks()
        {
            var tasks = tb.GetAllTasks().Select(task => new TaskModel
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                StatusId = task.StatusId,
                UserId = task.UserId
            })
            .ToList();

            return Ok(tasks);
        }

        // GET api/task/1
        [HttpGet]
        [Route("api/task/{id}")]
        public IHttpActionResult GetTaskById(int id)
        {
            var task = tb.GetTaskById(id);
            if (task == null)
            {
                return BadRequest("Given ID data not exist");
            }
            var taskModel = new TaskModel
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                StatusId = task.StatusId,
                UserId = task.UserId
            };
            return Ok(taskModel);
        }

        // POST api/task
        [HttpPost]
        [Route("api/task")]
        public IHttpActionResult AddTask(TaskModel taskModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var task = new TaskDTO
            {
                Title = taskModel.Title,
                Description = taskModel.Description,
                DueDate = taskModel.DueDate,
                StatusId = taskModel.StatusId,
                UserId = taskModel.UserId
            };
            bool status = tb.AddTask(task);
            if (status)
                return Content(HttpStatusCode.OK, "Task added successfully!");
            else
                return BadRequest("Failed to add the task. Please check your input or try again later.");
        }


        // PUT api/task/1
        [HttpPut]
        [Route("api/task/{id}")]
        public IHttpActionResult UpdateTask(int id, TaskModel taskmodel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var task = tb.GetTaskById(id);
            if (task == null)
            {
                return BadRequest("Given ID data not exist to update");
            }
            task.Id = id;
            task.Title = taskmodel.Title;
            task.Description = taskmodel.Description;
            task.DueDate = taskmodel.DueDate;
            task.StatusId = taskmodel.StatusId;
            task.UserId = taskmodel.UserId;

           bool status= tb.UpdateTask(task);
            if (status)
                return Content(HttpStatusCode.OK, "Task Updated successfully!");
            else
                return BadRequest("Failed to update the task. Please check your input or try again later.");
        }

        // DELETE api/task/1
        [HttpDelete]
        [Route("api/task/{id}")]
        public IHttpActionResult DeleteTask(int id)
        {
            var task = tb.GetTaskById(id);

            if (task == null)
            {
                return BadRequest("Given ID data not exist to update");
            }

            bool status=tb.DeleteTask(id);
            if(status)
                return Content(HttpStatusCode.OK, "Task Deleted successfully!");
            else
                return BadRequest("Failed to delete the task. Please check your input or try again later.");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                tb.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
