using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
    //[Route("api/[Controller]")]
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private readonly ILogging _logger;
        public VillaAPIController(ILogging logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDto>> GetVillas()
        {
            _logger.Log("Getting all villas", "info");
            return Ok(VillaStore.VillaList);
        }



        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDto> GetVilla(int id)
        {
            if (id == 0)
            {
                _logger.Log("Id must be greater than 0", "error");
                return BadRequest("Id must be greater than 0");
            }

            var villa = VillaStore.VillaList.FirstOrDefault(v => v.Id == id);

            if (villa == null) return NotFound();

            return Ok(villa);
        }




        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDto> CreateVilla([FromBody] VillaDto villaDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (VillaStore.VillaList.FirstOrDefault(v => v.Name.ToLower() == villaDto.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa already exists");
                return BadRequest(ModelState);
            }

            if (villaDto == null) return BadRequest("Villa data is required");

            if (villaDto.Id > 0) return StatusCode(StatusCodes.Status500InternalServerError);

            if (string.IsNullOrEmpty(villaDto.Name)) return BadRequest("Name is required");

            villaDto.Id = VillaStore.VillaList.Max(v => v.Id) + 1;
            VillaStore.VillaList.Add(villaDto);

            return CreatedAtAction(nameof(GetVilla), new { id = villaDto.Id }, villaDto);
        }



        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateVilla([FromBody] VillaDto villaDto, int id)
        {
            if (villaDto == null || id != villaDto.Id) return BadRequest();
            var villa = VillaStore.VillaList.FirstOrDefault(v => v.Id == id);
            if (villa == null) return NotFound();
            villa.Name = villaDto.Name;
            return NoContent();
        }


        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeleteVilla(int id)
        {
            if (id == 0) return BadRequest("Villa doesn't exist");
            var villa = VillaStore.VillaList.FirstOrDefault(v => v.Id == id);
            if (villa == null) return NotFound();

            VillaStore.VillaList.Remove(villa);
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePartialVilla(JsonPatchDocument<VillaDto> patchDto, int id)
        {
            if (patchDto == null || id == 0) return BadRequest();

            var villa = VillaStore.VillaList.FirstOrDefault(v => v.Id == id);
            if (villa == null) return NotFound();

            patchDto.ApplyTo(villa, ModelState);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            return NoContent();
        }

    }
}
