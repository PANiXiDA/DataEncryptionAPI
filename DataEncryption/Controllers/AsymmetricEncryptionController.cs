using Microsoft.AspNetCore.Mvc;
using Tools.AsymmetricEncryption;
using UI.Areas.Public.Models;

namespace DataEncryption.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "AsymmetricEncryption")]
    public class AsymmetricEncryptionController : ControllerBase
    {
        private readonly RsaEncryption _rsaEncryption;
        private readonly EcdsaEncryption _ecdsaEncryption;

        public AsymmetricEncryptionController(RsaEncryption rsaEncryption, EcdsaEncryption ecdsaEncryption)
        {
            _rsaEncryption = rsaEncryption;
            _ecdsaEncryption = ecdsaEncryption;
        }

        [HttpPost("RsaEncryptData")]
        public IActionResult RsaEncryptData([FromBody] object data)
        {
            try
            {
                var response = _rsaEncryption.Encrypt(data);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("RsaDecryptData")]
        public IActionResult RsaDecryptData([FromBody] string encryptedData)
        {
            try
            {
                var response = _rsaEncryption.Decrypt(encryptedData);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("EcdsaSignatureData")]
        public IActionResult EcdsaSignatureData([FromBody] UserModel user)
        {
            //_ecdsaEncryption.GenerateKeys();
            return Ok(user);
        }
    }
}
