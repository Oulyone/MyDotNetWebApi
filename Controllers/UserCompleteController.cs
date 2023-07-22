using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helper;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[Controller]")]
    public class UserCompleteController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly ReusableSql _reusableSql;

        public UserCompleteController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _reusableSql = new ReusableSql(config);
        }

        [HttpGet("TestConnection")]
        public DateTime TestConnection()
        {
            return _dapper.LoadDataSingle<DateTime>
                ("SELECT GETDATE();");
        }

        [HttpGet("GetUsers/{userId}/{onlyActive}")]
        public IEnumerable<UserComplete> GetUsers(int userId = 0, bool onlyActive = false)
        {
            string sql = @"EXEC TutorialAppSchema.spUsers_Get";
            string parameters = "";
            DynamicParameters sqlParameters = new DynamicParameters();


            if(userId != 0) 
            {
                parameters += ", @UserId=@UserIdParameter";    
                sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
            }

            if(onlyActive) 
            {
                parameters += ", @Active=@ActiveParameter";    
                sqlParameters.Add("@ActiveParameter", onlyActive, DbType.Boolean);
            }

            if (parameters.Length > 0) {
                sql += parameters.Substring(1);//, parameters.Length); 
            }

            IEnumerable<UserComplete> users = _dapper.LoadDataWithParameters<UserComplete>(sql, sqlParameters);
            return users;
        }

        [HttpPut("UpsertUser")]
        public IActionResult UpsertUser(UserComplete user)
        {
            if(_reusableSql.UpsertUser(user))
            {
                return Ok();
            }
            
            throw new Exception("Failed to update user");
        }

        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            string sql = @"
                EXEC TutorialAppSchema.spUsers_Delete
                    @UserId = @UserIdParameter";

            DynamicParameters sqlParameters = new DynamicParameters();       
            sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);

            if(_dapper.ExecuteSql(sql))
            {
                return Ok();
            }
            
            throw new Exception("Failed to delete user");
        }
    }
}
