using Microsoft.AspNetCore.Mvc;
using System.Text;
using Tools.HashEncryption;
using UI.Areas.Public.Models;
using Newtonsoft.Json;
using BL.Interfaces;
using Microsoft.Extensions.Options;
using Common.Configuration;
using DataEncryption.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;

namespace DataEncryption.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "HashEncryption")]
    public class HashController : ControllerBase
    {
        private readonly IUsersBL _usersBL;
        private readonly HashEncryption _hashEncryption;
        private readonly string _baseUrl;
        private readonly IHttpClientFactory _httpClientFactory;

        public HashController(
            IUsersBL usersBL,
            HashEncryption hashEncryption,
            IOptions<SharedConfiguration> sharedConfiguration,
            IHttpClientFactory httpClientFactory)
        {
            _usersBL = usersBL;
            _hashEncryption = hashEncryption;
            _baseUrl = sharedConfiguration.Value.BaseUrl;
            _httpClientFactory = httpClientFactory;
        }

        [Authorize(Roles = "Developer, Admin")]
        [HttpPost("VerifyTokenAndGetUser")]
        public async Task<IActionResult> VerifyTokenAndGetUser(int userId)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("Client");

                var JsonWebToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JsonWebToken);

                UserModel? user = UserModel.FromEntity(await _usersBL.GetAsync(userId));
                string userHash = _hashEncryption.GetHash(user);

                var getTokenResponse = await httpClient.PostAsync(
                    $"{_baseUrl}/api/Aes/GetTokenByUser",
                    new StringContent(JsonConvert.SerializeObject(new { id = userId, hash = userHash }),
                    Encoding.UTF8, "application/json"));

                if (!getTokenResponse.IsSuccessStatusCode)
                {
                    return BadRequest($"Failed to get token: {await getTokenResponse.Content.ReadAsStringAsync()}");
                }

                var getTokenResponseContent = await getTokenResponse.Content.ReadAsStringAsync();
                var getTokenResponseModel = JsonConvert.DeserializeObject<GetTokenResponse>(getTokenResponseContent);

                if (getTokenResponseModel == null)
                {
                    return BadRequest("Failed to parse token response.");
                }

                var token = getTokenResponseModel.Token;

                if (userHash != getTokenResponseModel.Hash)
                {
                    return BadRequest("Some of the data was intercepted.");
                }

                var getUserResponse = await httpClient.PostAsync(
                    $"{_baseUrl}/api/Aes/GetUserByToken",
                    new StringContent(JsonConvert.SerializeObject(new { token = token, hash = userHash }),
                    Encoding.UTF8, "application/json"));

                if (!getUserResponse.IsSuccessStatusCode)
                {
                    return BadRequest($"Failed to validate token: {await getUserResponse.Content.ReadAsStringAsync()}");
                }

                var userResponseContent = await getUserResponse.Content.ReadAsStringAsync();
                var getUserResponseModel = JsonConvert.DeserializeObject<GetUserResponse>(userResponseContent);

                if (getUserResponseModel == null)
                {
                    return BadRequest("Failed to parse user response.");
                }

                user = getUserResponseModel.User;

                if (userHash != getUserResponseModel.Hash)
                {
                    return BadRequest("Some of the data was intercepted.");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
