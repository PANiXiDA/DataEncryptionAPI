using Microsoft.AspNetCore.Mvc;
using UI.Areas.Public.Models;
using Tools.AsymmetricEncryption;

namespace DataEncryption.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "AsymmetricEncryption")]
    public class EcdsaController : ControllerBase
    {
        private readonly EcdsaEncryption _ecdsaEncryption;

        public EcdsaController(EcdsaEncryption ecdsaEncryption)
        {
            _ecdsaEncryption = ecdsaEncryption;
        }

        [HttpPost("EcdsaSignData")]
        public IActionResult EcdsaSignData(UserModel user)
        {
            try
            {
                var signedUser = _ecdsaEncryption.Sign(user);
                return Ok(signedUser);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("VerifyEcdsaSignData")]
        public IActionResult VerifyEcdsaSignData(string signedUserData)
        {
            try
            {
                if (_ecdsaEncryption.VerifySignedData(signedUserData, out UserModel user))
                {
                    return Ok(user);
                }
                else
                {
                    return BadRequest("Signature verification failed");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
