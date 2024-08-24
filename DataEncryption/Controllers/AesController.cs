using BL.Interfaces;
using DataEncryption.Models.Requests;
using DataEncryption.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tools.HashEncryption;
using Tools.SymmetricEncryption;
using UI.Areas.Public.Models;

namespace DataEncryption.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "SymmetricEncryption")]
    public class AesController : ControllerBase
    {
        private readonly IUsersBL _usersBL;
        private readonly AesEncryption _aesEncryption;
        private readonly HashEncryption _hashEncryption;

        public AesController(
            IUsersBL usersBL,
            AesEncryption aesEncryption,
            HashEncryption hashEncryption)
        {
            _usersBL = usersBL;
            _aesEncryption = aesEncryption;
            _hashEncryption = hashEncryption;
        }

        [Authorize(Roles = "Developer, Admin")]
        [HttpPost("GetTokenByUser")]
        public async Task<IActionResult> GetTokenByUser([FromBody] GetTokenRequest getTokenRequest)
        {
            try
            {
                var user = UserModel.FromEntity(await _usersBL.GetAsync(getTokenRequest.Id));

                if (!string.IsNullOrEmpty(getTokenRequest.Hash) && user != null)
                {
                    if (getTokenRequest.Hash != _hashEncryption.GetHash(user))
                    {
                        return BadRequest("Some of the data was intercepted.");
                    }
                }

                var token = _aesEncryption.Encrypt(user);

                return Ok(new GetTokenResponse { Token = token, Hash = getTokenRequest.Hash } );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Developer, Admin")]
        [HttpPost("GetUserByToken")]
        public IActionResult GetUserByToken([FromBody] GetUserRequest getUserRequest)
        {
            try
            {
                UserModel user = _aesEncryption.Decrypt<UserModel>(getUserRequest.Token);

                if (!string.IsNullOrEmpty(getUserRequest.Hash) && user != null)
                {
                    if (getUserRequest.Hash != _hashEncryption.GetHash(user))
                    {
                        return BadRequest("Some of the data was intercepted.");
                    }
                }

                return Ok(new GetUserResponse { User = user, Hash = getUserRequest.Hash } );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
