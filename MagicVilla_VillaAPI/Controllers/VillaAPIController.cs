using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers
{
    //[Route("api/[Controller]")]
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private readonly ILogging _logger;
        private readonly ICacheService _cacheService;

        private readonly ApplicationDbContext _dbContext;


        public VillaAPIController(ILogging logger, ApplicationDbContext context, ICacheService cacheService)
        {
            _logger = logger;
            _dbContext = context;
            _cacheService = cacheService;
        }

        //[HttpGet]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<IActionResult> GetVillas()
        //{
        //    var cacheData = _cacheService.GetData<IEnumerable<VillaDto>>("Villas");

        //    if (cacheData != null && cacheData.Count() > 0)
        //    {
        //        _logger.Log("Getting all villas from cache", "info");
        //        return Ok(cacheData);
        //    }

        //    _logger.Log("Getting all villas", "info");
        //    cacheData = (IEnumerable<VillaDto>?)await _dbContext.Villas.ToListAsync();


        //    // Set expiry time to 30 seconds
        //    var expiryTime = DateTimeOffset.Now.AddSeconds(30);
        //    _cacheService.SetData("Villas", cacheData, expiryTime);

        //    return Ok(cacheData);

        //}

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVillas()
        {
            var cacheData = _cacheService.GetData<IEnumerable<VillaDto>>("Villas");

            if (cacheData != null && cacheData.Count() > 0)
            {
                _logger.Log("Getting all villas from cache", "info");
                return Ok(cacheData);
            }

            _logger.Log("Getting all villas", "info");
            var villas = await _dbContext.Villas.ToListAsync();
            cacheData = villas.Select(v => new VillaDto
            {
                Id = v.Id,
                Name = v.Name,
            });

            // Set expiry time to 30 seconds
            var expiryTime = DateTimeOffset.Now.AddSeconds(30);
            _cacheService.SetData("Villas", cacheData, expiryTime);

            return Ok(cacheData);
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

            var villa = _dbContext.Villas.FirstOrDefault(v => v.Id == id);

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

            if (_dbContext.Villas.FirstOrDefault(v => v.Name.ToLower() == villaDto.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa already exists");
                return BadRequest(ModelState);
            }

            if (villaDto == null) return BadRequest("Villa data is required");

            if (villaDto.Id > 0) return StatusCode(StatusCodes.Status500InternalServerError);

            if (string.IsNullOrEmpty(villaDto.Name)) return BadRequest("Name is required");

            Villa model = new()
            {
                Id = villaDto.Id,
                Name = villaDto.Name,
                Details = villaDto.Details,
                Rate = villaDto.Rate,
                ImageUrl = villaDto.ImageUrl,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };
            _dbContext.Villas.Add(model);
            _dbContext.SaveChanges();

            return CreatedAtAction(nameof(GetVilla), new { id = villaDto.Id }, villaDto);
        }



        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateVilla([FromBody] VillaDto villaDto, int id)
        {
            if (villaDto == null || id != villaDto.Id) return BadRequest();

            //var villa = _dbContext.Villas.FirstOrDefault(v => v.Id == id);
            //if (villa == null) return NotFound();
            //villa.Name = villaDto.Name;


            Villa model = new()
            {
                Id = villaDto.Id,
                Name = villaDto.Name,
                Details = villaDto.Details,
                Rate = villaDto.Rate,
                ImageUrl = villaDto.ImageUrl,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };
            _dbContext.Villas.Update(model);
            _dbContext.SaveChanges();
            return NoContent();
        }



        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePartialVilla(JsonPatchDocument<VillaDto> patchDto, int id)
        {
            if (patchDto == null || id == 0) return BadRequest();

            var villa = _dbContext.Villas.FirstOrDefault(v => v.Id == id);
            if (villa == null) return NotFound();


            VillaDto villaDto = new()
            {
                Id = villa.Id,
                Name = villa.Name,
                Details = villa.Details,
                Rate = villa.Rate,
                ImageUrl = villa.ImageUrl,
            };


            patchDto.ApplyTo(villaDto, ModelState);


            Villa model = new()
            {
                Id = villaDto.Id,
                Name = villaDto.Name,
                Details = villaDto.Details,
                Rate = villaDto.Rate,
                ImageUrl = villaDto.ImageUrl,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };
            _dbContext.Villas.Update(model);
            _dbContext.SaveChanges();


            if (!ModelState.IsValid) return BadRequest(ModelState);

            return NoContent();
        }


        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeleteVilla(int id)
        {
            if (id == 0) return BadRequest("Villa doesn't exist");
            var villa = _dbContext.Villas.FirstOrDefault(v => v.Id == id);

            if (villa != null)
            {
                _dbContext.Villas.Remove(villa);
                _cacheService.RemoveData($"Villas{id}");
                _dbContext.SaveChanges();
                return NoContent();
            }

            return NotFound();
        }


    }
}
