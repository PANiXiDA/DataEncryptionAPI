using BL.Interfaces;
using Common.SearchParams;
using DataEncryption.Models.Requests;
using DataEncryption.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Tools.JsonWebTokenEncryption;
using UI.Areas.Public.Models;

namespace DataEncryption.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "Auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUsersBL _usersBL;
        private readonly ITokensBL _tokensBL;
        private readonly JsonWebTokenEncryption _jsonWebTokenEncryption;

        public AuthController(IUsersBL usersBL, ITokensBL tokensBL, JsonWebTokenEncryption jsonWebTokenEncryption) 
        {
            _usersBL = usersBL;
            _tokensBL = tokensBL;
            _jsonWebTokenEncryption = jsonWebTokenEncryption;
        }

        [HttpPost("Registration")]
        public async Task<IActionResult> Registration([FromBody] UserModel userModel)
        {
            if (userModel == null)
            {
                return BadRequest("UserModel is null");
            }

            var userEntity = await _usersBL.GetAsync(userModel.Login);
            if (userEntity != null)
            {
                return BadRequest("User with same login was already registered");
            }

            var newUserEntity = UserModel.ToEntity(userModel);
            if (newUserEntity == null)
            {
                return BadRequest("Conversion to entity failed");
            }

            try
            {
                var userId = await _usersBL.AddOrUpdateAsync(newUserEntity);

                return Ok(newUserEntity);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest user)
        {
            if (user == null)
            {
                return BadRequest("UserModel is null");
            }

            var userEntity = await _usersBL.VerifyPasswordAsync(user.Login, user.Password);
            if (userEntity == null)
            {
                return BadRequest("No user with same login and password");
            }

            try
            {
                var userAgent = Request.Headers["User-Agent"].ToString();
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var token = _jsonWebTokenEncryption.GenerateAccessToken(userEntity, userAgent, ipAddress);

                var userModel = UserModel.FromEntity(userEntity);
                var tokenModel = new TokenModel()
                {
                    AccessToken = token.AccessToken,
                    RefreshToken = token.RefreshToken,
                    User = userModel,
                    IsActive = true,
                    CreatedAt = token.CreatedAt,
                    AccessTokenExpiresAt = token.AccessTokenExpiresAt,
                    RefreshTokenExpiresAt = token.RefreshTokenExpiresAt,
                    UserAgent = userAgent,
                    IpAddress = ipAddress
                };

                var tokenEntityId = await _tokensBL.AddOrUpdateAsync(TokenModel.ToEntity(tokenModel)!);

                return Ok(new LoginResponse { User = userModel, Token = tokenModel });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("RefreshTokens")]
        public async Task<IActionResult> RefreshTokens([FromBody] RefreshJsonWebTokenRequest request)
        {
            try
            {
                var tokenEntity = (await _tokensBL.GetAsync(new TokensSearchParams()
                {
                    AccessToken = request.AccessToken,
                    RefreshToken = request.RefreshToken
                })).Objects.FirstOrDefault();

                if (tokenEntity == null || tokenEntity.RefreshTokenExpiresAt < DateTime.UtcNow)
                {
                    return BadRequest("Invalid or expired refresh token");
                }

                tokenEntity.IsActive = false;
                await _tokensBL.AddOrUpdateAsync(tokenEntity);

                var userAgent = Request.Headers["User-Agent"].ToString();
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var newToken = _jsonWebTokenEncryption.GenerateAccessToken(tokenEntity.User, userAgent, ipAddress);
                var newTokenModel = new TokenModel
                {
                    AccessToken = newToken.AccessToken,
                    RefreshToken = newToken.RefreshToken,
                    User = UserModel.FromEntity(tokenEntity.User),
                    IsActive = true,
                    CreatedAt = newToken.CreatedAt,
                    AccessTokenExpiresAt = newToken.AccessTokenExpiresAt,
                    RefreshTokenExpiresAt = newToken.RefreshTokenExpiresAt,
                    UserAgent = userAgent,
                    IpAddress = ipAddress
                };

                var newTokenModelToEntity = TokenModel.ToEntity(newTokenModel);
                await _tokensBL.AddOrUpdateAsync(newTokenModelToEntity!);

                return Ok(newTokenModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
