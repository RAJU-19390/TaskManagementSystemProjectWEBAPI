using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TaskDataAccessLayer;
using AutoMapper;

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

        public List<UserInfoDTO> GetAllUserInfoByName(string name)
        {
            var userList = dbcontext.UserInfoes.Where(u => u.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();

            return mapper.Map<List<UserInfoDTO>>(userList);
        }

        public IEnumerable<UserTaskDTO> GetUserTasks(int userId)
        {
            var userTasks = from t in dbcontext.Tasks
                            join u in dbcontext.UserInfoes on t.UserId equals u.Id
                            join s in dbcontext.Status on t.StatusId equals s.Id
                            where u.Id == userId
                            select new UserTaskDTO
                            {
                                UserId = u.Id,
                                UserName = u.Name,
                                TaskId = t.Id,
                                Description = t.Description,
                                DueDate = t.DueDate,
                                StatusMode = s.Mode
                            };
            return mapper.Map<List<UserTaskDTO>>(userTasks.ToList());
        }

        public static string HashPassword(string password)
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
            bool status = false;
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            string encryptedpwd = HashPassword(user.Password);
            try
            {
                dbcontext.sp_InsertUserInfo( user.Name,user.Email,encryptedpwd,user.Is_Admin);
                dbcontext.SaveChanges();
                status = true;
                return status;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to add the user. Error: {ex.Message}", ex);
            }
        }
        public bool UpdateUserInfo(UserInfoDTO user)
        {
            bool status = false;
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
               
                if (existinguser.Password == encryptedpwd|| existinguser.Password!=encryptedpwd)
                {
                    dbcontext.sp_UpdateUserInfo(user.Id, user.Name, user.Email, encryptedpwd,user.Is_Admin);
                }
                dbcontext.SaveChanges();
                status = true;
                return status;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to update user with ID {user.Id}. Error: {ex.Message}", ex);
            }
        }
        public bool DeleteUserInfo(int userid)
        {
            bool status = false;
            var user = dbcontext.UserInfoes.Find(userid);
            if (user != null)
            {
                dbcontext.sp_DeleteUserInfoById(userid);
                dbcontext.SaveChanges();
                status = true;
            }
            return status;
        }
    }
}
