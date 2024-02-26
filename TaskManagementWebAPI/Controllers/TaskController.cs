using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using AutoMapper;
using TaskBusinessLayer;
using TaskManagementWebAPI.Models;

namespace TaskManagementWebAPI.Controllers
{
    public class TaskController : ApiController
    {
        private readonly TaskBusiness tb;
        private readonly IMapper mapper;
        public TaskController()
        {
            var dbcontext = new TaskDataAccessLayer.TaskManagementDatabaseEntities();
            this.tb = new TaskBusiness(dbcontext) ?? throw new ArgumentNullException(nameof(tb));

            // AutoMapper Configuration
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TaskDTO, TaskModel>();
                cfg.CreateMap<TaskModel, TaskDTO>();
            });
            mapper = config.CreateMapper();
        }

        // GET api/task
        [HttpGet]
        [Route("api/task")]
        public IHttpActionResult GetTasks()
        {
            var tasks = tb.GetAllTasks().Select(task => mapper.Map<TaskModel>(task)).ToList();
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

            var taskModel = mapper.Map<TaskModel>(task);
            return Ok(taskModel);
        }

        // POST api/task
        [HttpPost]
        [Route("api/task")]
        public IHttpActionResult AddTask(TaskModel taskModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var taskDTO = mapper.Map<TaskDTO>(taskModel);
            bool status = tb.AddTask(taskDTO);

            if (status)
                return Content(HttpStatusCode.OK, "Task added successfully!");
            else
                return BadRequest("Failed to add the task. Please check your input or try again later.");
        }

        // PUT api/task/1
        [HttpPut]
        [Route("api/task/{id}")]
        public IHttpActionResult UpdateTask(int id, TaskModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingTask = tb.GetTaskById(id);
            if (existingTask == null)
            {
                return BadRequest("Given ID data not exist to update");
            }
            var updatedtask = mapper.Map<TaskDTO>(model);
            updatedtask.Id = id;

            bool status = tb.UpdateTask(updatedtask);

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
            var existingTask = tb.GetTaskById(id);

            if (existingTask == null)
            {
                return BadRequest("Given ID data not exist to update");
            }

            bool status = tb.DeleteTask(id);

            if (status)
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
