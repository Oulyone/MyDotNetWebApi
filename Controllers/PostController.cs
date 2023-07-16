using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[Controller]")]
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly IConfiguration _config;
        public PostController(IConfiguration config)
        {
            _config = config;
            _dapper = new DataContextDapper(config);
        }
        [HttpGet("Posts/{postId}/{userId}/{searchParam}")]
        public IEnumerable<Post> GetPosts(int postId = 0, int userId = 0, string searchParam = "none")
        {
            string sql = "EXEC TutorialAppSchema.spPosts_Get";
            string parameters = "";

            if(postId != 0)
            {
                parameters += ", @PostId=" + postId.ToString();
            }

            if(userId != 0)
            {
                parameters += ", @UserId=" + userId.ToString();
            }

            if(searchParam.ToLower() != "none")
            {
                parameters += ", @SearchValue='" + searchParam + "'";
            }

            if(parameters.Length > 0)
            {
                sql += parameters.Substring(1);
            }

            return _dapper.LoadData<Post>(sql);
        }

        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPosts()
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Get @UserId=" 
                + this.User.FindFirst("userId")?.Value;
               
            return _dapper.LoadData<Post>(sql);
        }

        [HttpPut("UpsertPost")]
        public IActionResult AddPost(Post postToUpsert)
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Upsert
                    @UserId =" + this.User.FindFirst("userId")?.Value +
                    ", @PostTitle ='" + postToUpsert.PostTitle + "'" +
                    ", @PostContent ='" + postToUpsert.PostContent + "'";

            if(postToUpsert.PostId > 0)
            {
                sql += ", @PostId = " + postToUpsert.PostId;
            }
            
            if(_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to create new post!");
        }

        [HttpDelete("Post/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Delete
                @PostId = " + postId.ToString() +
                ", @UserId = " + User.FindFirst("userId")?.Value;

            if(_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to delete post!");
        }
    }
}