using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using AutoMapper;
using TaskBusinessLayer;
using TaskManagementWebAPI.Models;

namespace TaskMgm.Controllers
{
    public class UserInfoController : ApiController
    {
        private readonly UserBusiness ub;
        private readonly IMapper mapper;
        public UserInfoController()
        {
            var dbcontext = new TaskDataAccessLayer.TaskManagementDatabaseEntities();
            this.ub = new UserBusiness(dbcontext) ?? throw new ArgumentNullException(nameof(ub));

            // AutoMapper Configuration
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserInfoDTO, UserInfoModel>();
                cfg.CreateMap<UserInfoModel, UserInfoDTO>();
                cfg.CreateMap<UpdatePwdDTO, UpdatePwdModel>();
                cfg.CreateMap<UpdatePwdModel, UpdatePwdDTO>();
            });

            mapper = config.CreateMapper();
        }

        // GET api/UserInfo
        [HttpGet]
        [Route("api/UserInfo")]
        public IHttpActionResult GetAllUsers()
        {
            var allusers = ub.GetAllUserInfos().Select(user => mapper.Map<UserInfoModel>(user)).ToList();
            if (allusers == null)
                return BadRequest("Users data not exist");
            else
                return Ok(allusers);
        }

        // GET api/UserInfo/5
        [HttpGet]
        [Route("api/UserInfo/{id}")]
        public IHttpActionResult GetUserById(int id)
        {
            var user = ub.GetUserById(id);
            if (user == null)
            {
                return BadRequest("Given ID data not exist");
            }
            var userModel = mapper.Map<UserInfoModel>(user);
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

            var usermodels = allusers.Select(u => mapper.Map<UserInfoModel>(u)).ToList();

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
            var user = mapper.Map<UserInfoDTO>(model);
            bool status = ub.AddUserInfo(user);
            if (status)
                return Content(HttpStatusCode.OK, "User added successfully!");
            else
                return BadRequest("Failed to add the user. Please check your input or try again later.");
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
            var user = mapper.Map<UserInfoDTO>(model);
            bool status = ub.UpdateUserInfo(user);
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

            bool status = ub.DeleteUserInfo(id);
            if (status)
                return Content(HttpStatusCode.OK, "User Deleted successfully!");
            else
                return BadRequest("Getting Error While deleting user account");
        }

        [HttpPut]
        [Route("api/UserInfo/UpdatePassword")]
        public IHttpActionResult UpdatePassword([FromBody] UpdatePwdModel updatePwdModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedPwdDTO = mapper.Map<UpdatePwdDTO>(updatePwdModel);

            try
            {
                var user = ub.GetUserByEmail(updatedPwdDTO.Email);
                if (user == null)
                {
                    return BadRequest("User not found for the given email.");
                }
                bool status=ub.UpdatePassword(updatedPwdDTO);
                if (status)
                    return Ok("Password updated successfully!");
                else
                    return BadRequest("Current Password is not Correct.Recheck Once Again...");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}
