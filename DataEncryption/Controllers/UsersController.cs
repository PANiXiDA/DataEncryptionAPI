using BL.Interfaces;
using Common.SearchParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UI.Areas.Public.Models;

namespace DataEncryption.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "Users")]
    public class UsersController : ControllerBase
    {
        private IUsersBL _usersBL;
        public UsersController(IUsersBL usersBL) 
        { 
            _usersBL = usersBL;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddOrUpdateUser")]
        public async Task<IActionResult> AddOrUpdateUser([FromBody] UserModel userModel)
        {
            if (userModel == null)
            {
                return BadRequest("UserModel is null");
            }

            var userEntity = UserModel.ToEntity(userModel);
            if (userEntity == null)
            {
                return BadRequest("Conversion to entity failed");
            }

            try
            {
                var userId = await _usersBL.AddOrUpdateAsync(userEntity);

                return Ok(userEntity);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var user = UserModel.FromEntity(await _usersBL.GetAsync(id));

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("SearchInUsers")]
        public async Task<IActionResult> SearchInUsers(UsersSearchParams searchParams)
        {
            try
            {
                var users = UserModel.FromEntitiesList((await _usersBL.GetAsync(searchParams)).Objects);

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _usersBL.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
