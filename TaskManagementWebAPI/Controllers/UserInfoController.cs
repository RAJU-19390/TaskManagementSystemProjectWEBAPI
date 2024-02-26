using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using TaskBusinessLayer;
using TaskManagementWebAPI.Models;

namespace TaskMgm.Controllers
{
    public class UserInfoController : ApiController
    {
        private readonly UserBusiness ub;

        public UserInfoController()
        {
             var dbcontext = new TaskDataAccessLayer.TaskManagementDatabaseEntities();
            this.ub = new UserBusiness(dbcontext) ?? throw new ArgumentNullException(nameof(ub));
        }

        // GET api/UserInfo
        [HttpGet]
        [Route("api/UserInfo")]
        public IHttpActionResult GetAllUsers()
        {
            var allusers = ub.GetAllUserInfos();
            var usermodels = allusers.Select(u => new UserInfoModel
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Password = u.Password,
                Is_Admin = u.Is_Admin
            }).ToList();

            return Ok(usermodels);
        }

        // GET api/UserInfo/5
        [HttpGet]
        [Route("api/UserInfo/{id}")]
        public IHttpActionResult GetUserById(int id)
        {
            var user = ub.GetUserById(id);

            if (user == null)
            {
                return NotFound();
            }

            var userModel = new UserInfoModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
                Is_Admin = user.Is_Admin
            };

            return Ok(userModel);
        }

        [HttpGet]
        [Route("api/UserInfo/ByName/{name}")]
        public IHttpActionResult GetUserByName(string name)
        {
            var allusers = ub.GetAllUserInfoByName(name);

            if (allusers.Count == 0)
            {
                return BadRequest("Given Name data not exist");
            }

            var usermodels = allusers.Select(u => new UserInfoModel
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Password = u.Password,
                Is_Admin = u.Is_Admin
            }).ToList();

            return Ok(usermodels);
        }

        [HttpGet]
        [Route("api/UserInfo/UserTasks/{userId}")]
        public IHttpActionResult GetUserTasks(int userId)
        {
            var userTasks = ub.GetUserTasks(userId);

            if (userTasks == null)
            {
                return BadRequest($"No user tasks found for userId: {userId}");
            }

            return Ok(userTasks);
        }

        [HttpPost]
        [Route("api/UserInfo")]
        // POST api/UserInfo
        public IHttpActionResult AddUserData(UserInfoModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Encrypt the password using SHA-256
            var user = new UserInfoDTO
            {
                Name = model.Name,
                Email = model.Email,
                Password = model.Password,
                Is_Admin = model.Is_Admin
            };
            bool status =ub.AddUserInfo(user);
            if (status)
                return Content(HttpStatusCode.OK, "User added successfully!");
            else
                return BadRequest("Failed to add the userx. Please check your input or try again later.");
        }

        // PUT api/UserInfo/5
        [HttpPut]
        [Route("api/UserInfo/{id}")]
        public IHttpActionResult UpdateUserData(int id, UserInfoModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = new UserInfoDTO
            {
                Id = id,
                Name = model.Name,
                Email = model.Email,
                Password = model.Password,
                Is_Admin = model.Is_Admin
            };
            bool status =ub.UpdateUserInfo(user);
            if (status)
                return Content(HttpStatusCode.OK, "User Updated successfully!");
            else
                return BadRequest("Failed to update the user. Please check your input or try again later.");
        }

        // DELETE api/UserInfo/5
        [HttpDelete]
        [Route("api/UserInfo/{id}")]
        public IHttpActionResult DeleteUserInfo(int id)
        {
            var user = ub.GetUserById(id);

            if (user == null)
            {
                return BadRequest("Given ID data not exist");
            }

            bool status=ub.DeleteUserInfo(id);
            if (status)
                return Content(HttpStatusCode.OK, "User Deleted successfully!");
            else
                return BadRequest("Failed to delete the user. Please check your input or try again later.");
        }
    }
}
