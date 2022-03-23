using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
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
    public class BranchesController : ControllerBase
    {
        private readonly BranchService _branchService;
        private readonly IStringLocalizer<BranchesController> _localizer;
        public BranchesController(BranchService branchService, IStringLocalizer<BranchesController> localizer)
        {
            _branchService = branchService;
            _localizer = localizer;
        }

        // GET: api/Branches/lookup
        [HttpPost("lookup")]
        public async Task<ActionResult<IEnumerable<BranchModel>>> GetBranches([FromBody] BranchLookup lookup)
        {
            //return await _context.Branches.ToListAsync();
            return await _branchService.GetBranches(lookup);
        }
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<BranchModel>>> GetBranches()
        //{
        //    //return await _context.Branches.ToListAsync();
        //    return await _branchService.GetBranches();
        //}


        // GET: api/Branches/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BranchModel>> GetBranch(Guid id)
        {
            var branch = await _branchService.GetBranch(id);

            if (branch == null)
            {
                return NotFound();
            }

            return branch;
        }

        // PUT: api/Branches/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBranch(Guid id, BranchModelPersist branch)
        {
            if (id != branch.Id)
            {
                return BadRequest();
            }

            //_context.Entry(branch).State = EntityState.Modified;

            try
            {
                // await _context.SaveChangesAsync();
                await _branchService.PutBranch(id, branch);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BranchExists(id))
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

        // POST: api/Branches
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<BranchModel>> PostBranch(BranchModelPersist branch)
        {
            //_context.Branches.Add(branch);
            //await _context.SaveChangesAsync();
            var validationResult = new BranchValidator().Validate(branch);
            if (validationResult.IsValid == false)
            {
                var returnString = _localizer["Invalid Branch"];
                throw new MyException(returnString);
            }
            await _branchService.AddSave(branch);

            return CreatedAtAction("GetBranch", new { id = branch.Id }, branch);
        }

        // DELETE: api/Branches/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<BranchModelPersist>> DeleteBranch(Guid id)
        {
            var branch = await _branchService.FindBranch(id);
            if (branch == null)
            {
                return NotFound();
            }

            //_context.Branches.Remove(branch);
            //await _context.SaveChangesAsync();
            return await _branchService.RemoveBranch(branch);

            //return branch;
        }

        private bool BranchExists(Guid id)
        {
            //return _context.Branches.Any(e => e.Id == id);
            return _branchService.BranchExists(id);
        }
    }
}
