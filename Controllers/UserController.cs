using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    DataContextDapper _dapper;
    public UserController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }


    [HttpGet("TestConnection")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>
            ("SELECT GETDATE();");
    }

    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        string sql = @"
            SELECT [UserId],
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active] 
            FROM TutorialAppSchema.Users";
        
        IEnumerable<User> users = _dapper.LoadData<User>(sql);
        return users;
    }

    [HttpGet("GetUserSingle/{userId}")]
    public User GetUser(int userId)
    {
        string sql = @"
            SELECT [UserId],
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active] 
            FROM TutorialAppSchema.Users
                WHERE UserId = " + userId.ToString();
        
        User user = _dapper.LoadDataSingle<User>(sql);
        return user;
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        string sql = @"
        UPDATE TutorialAppSchema.Users
            SET [FirstName] = '" + user.FirstName +
            "', [LastName] = '" + user.LastName +
            "', [Email] = '" + user.Email +
            "', [Gender] = '" + user.Gender +
            "', [Active] = '" + user.Active +
            "' WHERE UserId = " + user.UserId;

        if(_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        
        throw new Exception("Failed to update user");
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        string sql = @"
            INSERT INTO TutorialAppSchema.Users(
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active] 
            ) VALUES (" +
                "'" + user.FirstName +
                "', '" + user.LastName +
                "', '" + user.Email +
                "', '" + user.Gender +
                "', '" + user.Active +
            "')";

        System.Console.WriteLine(sql);

        if(_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to add user");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @"
            DELETE FROM TutorialAppSchema.Users
                WHERE UserId =" + userId.ToString();

        if(_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        
        throw new Exception("Failed to delete user");
    }

    [HttpGet("UserSalary/{userId}")]
    public UserSalary GetUserSalary(int userId)
    {
        string sql = @"
            SELECT UserSalary.UserId
                    , UserSalary.Salary
                    , UserSalary.AvgSalary
            FROM TutorialAppSchema.UserSalary
                WHERE UserSalary.UserId =" + userId.ToString();
        UserSalary userSalary = _dapper.LoadDataSingle<UserSalary>(sql);
        return userSalary;
    }

    [HttpPost("UserSalary")]
    public IActionResult PostUserSalary(UserSalary userSalaryForInsert)
    {
        string sql = @"
            INSERT INTO TutorialAppSchema.UserSalary (
                UserId,
                Salary
            ) VALUES (" + userSalaryForInsert.UserId.ToString() 
            + ", " + userSalaryForInsert.Salary.ToString() 
            + ")";

        if(_dapper.ExecuteSql(sql))
        {
            return Ok(userSalaryForInsert);
        }
    throw new Exception("Failed to add UserSalary");
    }

    [HttpPut("UserSalary")]
    public IActionResult PutUserSalary(UserSalary userSalaryForUpdate)
    {
        string sql = "UPDATE TutorialAppSchema.UserSalary SET Salary="
            + userSalaryForUpdate.Salary.ToString() 
            + "WHERE UserSalary =" + userSalaryForUpdate.UserId.ToString();

        if(_dapper.ExecuteSql(sql)) 
        {
            return Ok(userSalaryForUpdate);
        }

        throw new Exception("Updating UserSalary failed to save");
    }

    [HttpDelete("UserSalary/{userId}")]
    public IActionResult DeleteUserSalary(int userId)
    {
        string sql = "DELETE FROM TutorialAppSchema.UserSalary WHERE UserID =" + userId.ToString();
        
        if(_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Deleting UserSalary failed to save");
    }

    [HttpGet("UserJobInfo/{userId}")]
    public UserJobInfo GetUserJobInfo(int userId)
    {
        string sql = @"
            SELECT [UserId],
                [JobTitle],
                [Department]
            FROM TutorialAppSchema.UserJobInfo 
                WHERE UserId =" + userId.ToString();
        
        return _dapper.LoadDataSingle<UserJobInfo>(sql);
    }

    [HttpPost("UserJobInfo")]
    public IActionResult PostUserJobInfo(UserJobInfo userJobInfoForInsert)
    {
        string sql = @"
            INSERT INTO TutorialAppSchema.UserJobInfo (
                UserId,
                JobTitle,
                Department
            ) VALUES (" + userJobInfoForInsert.UserId.ToString()
            + ", '" + userJobInfoForInsert.JobTitle
            + "', '" + userJobInfoForInsert.Department
            + "')";

        if(_dapper.ExecuteSql(sql))
        {
            return Ok(userJobInfoForInsert);
        }

        throw new Exception("Adding UserJobInfo failed to save");
    }

    [HttpPut("UserJobInfo")]
    public IActionResult PutUserJobInfo(UserJobInfo userJobInfoForUpdate)
    {
        string sql = @"
            UPDATE TutorialAppSchema.UserJobInfo SET JobTitle = '"
            + userJobInfoForUpdate.JobTitle + "', Department = '"
            + userJobInfoForUpdate.Department + "' WHERE UserId ="
            + userJobInfoForUpdate.UserId.ToString();

        if(_dapper.ExecuteSql(sql))
        {
            return Ok(userJobInfoForUpdate);
        }
            
        throw new Exception("Updating UserJobInfo failed to save");
    }

    [HttpDelete("UserJobInfo/{userId}")]
    public IActionResult DeleteUserJobInfo(int userId)
    {
        string sql = "DELETE FROM TutorialAppSchema.UserJobInfo WHERE UserId =" + userId.ToString();

        if(_dapper.ExecuteSql(sql)) 
        {
            return Ok();
        }

        throw new Exception("Deleting UserJobInfo failed to save");
    }

}
