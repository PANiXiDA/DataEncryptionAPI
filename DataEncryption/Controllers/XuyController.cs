using BL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Common.SearchParams;
using UI.Areas.Public.Models;

namespace DataEncryption.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "Xuys")]
    public class XuyController : ControllerBase
    {
        private readonly IXuysBL _xuysBL;

        public XuyController(IXuysBL xuysBL)
        {
            _xuysBL = xuysBL;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddOrUpdateXuy")]
        public async Task<IActionResult> AddOrUpdateXuy([FromBody] XuyModel model)
        {
            if (model == null)
            {
                return BadRequest("Model is null");
            }

            var entity = XuyModel.ToEntity(model);
            if (entity == null)
            {
                return BadRequest("Conversion to entity failed");
            }

            try
            {
                var id = await _xuysBL.AddOrUpdateAsync(entity);
                model.Id = id;
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetXuy")]
        public async Task<IActionResult> GetXuy(int id)
        {
            try
            {
                var entity = await _xuysBL.GetAsync(id);
                var model = XuyModel.FromEntity(entity);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("SearchXuys")]
        public async Task<IActionResult> SearchXuys([FromQuery] XuysSearchParams searchParams)
        {
            try
            {
                var result = await _xuysBL.GetAsync(searchParams);
                var models = XuyModel.FromEntitiesList(result.Objects);
                return Ok(models);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteXuy")]
        public async Task<IActionResult> DeleteXuy(int id)
        {
            try
            {
                await _xuysBL.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
