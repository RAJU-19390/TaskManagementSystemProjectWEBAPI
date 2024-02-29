using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TaskDataAccessLayer;

namespace TaskBusinessLayer
{
    public class UserBusiness
    {
        private readonly TaskManagementDatabaseEntities dbcontext;
        private readonly IMapper mapper;

        public UserBusiness(TaskManagementDatabaseEntities dbcontext)
        {
            this.dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            // AutoMapper Configuration
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserInfo, UserInfoDTO>();
                cfg.CreateMap<UserInfoDTO, UserInfo>();
                cfg.CreateMap<UserTaskDTO, TaskDTO>();
            });

            mapper = config.CreateMapper();
        }

        public List<UserInfoDTO> GetAllUserInfos()
        {
            var users = dbcontext.UserInfoes.ToList();
            return mapper.Map<List<UserInfoDTO>>(users);
        }

        public UserInfoDTO GetUserById(int userid)
        {
            var user = dbcontext.UserInfoes.Find(userid);
            return mapper.Map<UserInfoDTO>(user);
        }
        public UserInfoDTO GetUserByEmail(string email)
        {
            var user = dbcontext.UserInfoes.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            return mapper.Map<UserInfoDTO>(user);
        }


        public List<UserInfoDTO> GetAllUserInfoByName(string name)
        {
            var userList = dbcontext.UserInfoes.Where(u => u.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();

            return mapper.Map<List<UserInfoDTO>>(userList);
        }

        public IEnumerable<UserTaskDTO> GetUserTasks(int userId)
        {
            var userTasksGrouped = from task in dbcontext.Tasks
                                   join user in dbcontext.UserInfoes on task.UserId equals user.Id
                                   join status in dbcontext.Status on task.StatusId equals status.Id
                                   where user.Id == userId
                                   group new { task, user, status } by new { user.Id, user.Name } into groupbytask
                                   select new UserTaskDTO
                                   {
                                       UserId = groupbytask.Key.Id,
                                       UserName = groupbytask.Key.Name,
                                       TaskData = groupbytask.Select(required => new RequiredUserTaskDTO
                                       {
                                           Id = required.task.Id,
                                           Title = required.task.Title,
                                           Description = required.task.Description,
                                           DueDate = required.task.DueDate,
                                           Status = new List<StatusDTO>
                                             { new StatusDTO
                                                {  Mode = required.status.Mode }
                                             }
                                       }).ToList()
                                   };

            return mapper.Map<List<UserTaskDTO>>(userTasksGrouped.ToList());
        }

        public string HashPassword(string password)
        {
            using (SHA512 sha512Hash = SHA512.Create())
            {
                byte[] bytes = sha512Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        public bool AddUserInfo(UserInfoDTO user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            string encryptedpwd = HashPassword(user.Password);
            try
            {
                dbcontext.sp_InsertUserInfo(user.Name, user.Email, encryptedpwd, user.Is_Admin);
                dbcontext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool UpdateUserInfo(UserInfoDTO user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            try
            {
                var existinguser = dbcontext.UserInfoes.Find(user.Id);
                if (existinguser == null)
                {
                    throw new ArgumentException($"User with ID {user.Id} not found");
                }
                string encryptedpwd = HashPassword(user.Password);

                dbcontext.sp_UpdateUserInfo(user.Id, user.Name, user.Email, encryptedpwd, user.Is_Admin);
                dbcontext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdatePassword(UpdatePwdDTO updatedpwd)
        {
            var user = dbcontext.UserInfoes.FirstOrDefault(u => u.Email.Equals(updatedpwd.Email, StringComparison.OrdinalIgnoreCase));
            string hashedpwd = HashPassword(updatedpwd.OldPassword);//Existing password is hashed to check
            try
            {
                if (user.Password == hashedpwd.Substring(0, 10))//checking Password stored pwd and entered pwd
                {
                    user.Password = HashPassword(updatedpwd.NewPassword);  //new password is hashed here
                    dbcontext.sp_UpdateUserInfo(user.Id, user.Name, user.Email, user.Password, user.Is_Admin);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool DeleteUserInfo(int userid)
        {
            try
            {
                var user = dbcontext.UserInfoes.Find(userid);
                dbcontext.sp_DeleteUserInfoById(userid);
                dbcontext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
