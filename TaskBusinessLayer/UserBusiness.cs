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
        public bool status;
        public UserBusiness(TaskManagementDatabaseEntities dbcontext)
        {
            this.dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            status = false;
        }

        public List<UserInfo> GetAllUserInfos()
        {
            return dbcontext.UserInfoes.ToList();
        }

        public UserInfo GetUserById(int userid)
        {
            return dbcontext.UserInfoes.Find(userid);
        }
        public List<UserInfo> GetAllUserInfoByName(string name)
        {
            var userList = dbcontext.UserInfoes.Where(u => u.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();

            return userList;
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
            return userTasks.ToList();
        }

        public static string HashPassword(string password)
        {
            using (SHA512 sha256Hash = SHA512.Create()) // using Create() method of SHA256 class to create an instance of the class.
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password)); // Computing hash using UTF8 encoding 
                // Convert byte array to a string
                StringBuilder builder = new StringBuilder(); // using "StringBuilder" class, to build strings from the bytes obtained above.
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2")); // Converting byte to String using "x2" fomrat specifier.
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
            string encryptedpassword = HashPassword(user.Password);
            dbcontext.sp_InsertUserInfo(
                user.Name,
                user.Email,
                encryptedpassword,
                user.Is_Admin
            );
            dbcontext.SaveChanges();
            status = true;
            return status;
        }

        public bool UpdateUserInfo(UserInfoDTO user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var existinguser = dbcontext.UserInfoes.Where(t => t.Id == user.Id).SingleOrDefault();

            if (existinguser == null)
            {
                throw new ArgumentException($"User with ID {user.Id} not found");
            }
            existinguser.Id = user.Id;
            existinguser.Name = user.Name;
            existinguser.Email = user.Email;
            existinguser.Is_Admin = user.Is_Admin;
            string encryptedpwd = HashPassword(user.Password);
            if (existinguser.Password != encryptedpwd)
            {
                dbcontext.sp_UpdateUserInfo(existinguser.Id, existinguser.Name, existinguser.Email, encryptedpwd, existinguser.Is_Admin);
            }
            dbcontext.SaveChanges();
            return true;
        }


        public bool DeleteUserInfo(int userid)
        {
            var user = dbcontext.UserInfoes.Find(userid);

            if (user != null)
            {
                dbcontext.sp_DeleteUserInfoById(userid);
                dbcontext.SaveChanges();
                status = true;
                return status;
            }
            return status;
        }
    }
}
