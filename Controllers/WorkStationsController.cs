using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using TodoApi2.Data;
using TodoApi2.ExceptionHandle;
using TodoApi2.Models;
using TodoApi2.Models.Lookups;
//using TodoApi2.Models;
using TodoApi2.Service;
using TodoApi2.Validator;


namespace TodoApi2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkStationsController : ControllerBase
    {
        private readonly WorkStationService _workStationService;
        private readonly IStringLocalizer<WorkStationsController> _localizer;
        public WorkStationsController(WorkStationService workStationService, IStringLocalizer<WorkStationsController> localizer)
        {
            _workStationService = workStationService;
            _localizer = localizer;
        }

        // GET: api/WorkStations/lookup
        [HttpPost("lookup")]
        public async Task<ActionResult<IEnumerable<WorkStationModel>>> GetWorkStations([FromBody] WorkStationLookup lookup)
        {
            //return await _context.WorkStations.ToListAsync();
            return await _workStationService.GetWorkStations(lookup);
        }
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<WorkStationModel>>> GetWorkStations()
        //{
        //    //return await _context.WorkStations.ToListAsync();
        //    return await _workStationService.GetWorkStations();
        //}

        // GET: api/WorkStations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkStationModel>> GetWorkStation(Guid id)
        {
            var workStation = await _workStationService.GetWorkStation(id);

            if (workStation == null)
            {
                return NotFound();
            }

            return workStation;
        }

        // PUT: api/WorkStations/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWorkStation(Guid id, WorkStationModelPersist workStation)
        {
            if (id != workStation.Id)
            {
                return BadRequest();
            }

            //_context.Entry(workStation).State = EntityState.Modified;

            try
            {
                await _workStationService.PutWorkStation(id,workStation);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkStationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/WorkStations
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<WorkStationModel>> PostWorkStation(WorkStationModelPersist workStation)
        {
            //_context.WorkStations.Add(workStation);
            //await _context.SaveChangesAsync();
            var validationResult = new WorkStationValidator().Validate(workStation);
            if (validationResult.IsValid == false)
            {
                var returnString = _localizer["Invalid WorkStation"];
                throw new MyException(returnString);
            }
            await _workStationService.AddSave(workStation);

            return CreatedAtAction("GetWorkStation", new { id = workStation.Id }, workStation);
        }

        // DELETE: api/WorkStations/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<WorkStationModelPersist>> DeleteWorkStation(Guid id)
        {
            var workStation = await _workStationService.FindWorkStation(id);
            if (workStation == null)
            {
                return NotFound();
            }

            //_context.WorkStations.Remove(workStation);
            //await _context.SaveChangesAsync();
            return await _workStationService.RemoveWorkStation(workStation);

            //return workStation;
        }

        private bool WorkStationExists(Guid id)
        {
            //return _context.WorkStations.Any(e => e.Id == id);
            return _workStationService.WorkStationExists(id);
        }
    }
}
