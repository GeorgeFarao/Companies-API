using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi2.Data;
using TodoApi2.Models;
using TodoApi2.Models.Builders;
using TodoApi2.Models.Deleters;
using TodoApi2.Models.Lookups;

namespace TodoApi2.Service
{
    public class WorkStationService
    {

        private MyAppContext _context { get; set; }
        private WorkStationBuilder _builder { get; set; }
        public WorkStationDeleter _deleter { get; set; }
        public WorkStationService(MyAppContext context, WorkStationBuilder builder, WorkStationDeleter deleter)
        {
            _context = context;
            _builder = builder;
            _deleter = deleter;
        }

        public async Task<ActionResult<IEnumerable<WorkStationModel>>> GetWorkStations(WorkStationLookup lookup)
        {
            if (lookup.BranchId == default && lookup.Like == default)
            {
                var workStations = await _context.WorkStations.ToListAsync();

                return await _builder.Build(workStations, null);
            }
            else
            {
                var workstations = _context.WorkStations as IQueryable<WorkStation>;
                if (lookup.BranchId !=null && lookup.BranchId.Count > 0)
                {
                    workstations = workstations.Where(x => lookup.BranchId.Contains(x.BranchId));
                }

                if (lookup.Like !=null)
                {
                    workstations = workstations.Where(x => x.Name.Contains(lookup.Like));
                }

                if (lookup.PagingInfo != null)
                {
                    if (lookup.PagingInfo.Offset != 0 | lookup.PagingInfo.Size !=0)
                    {
                        workstations = workstations.Skip(lookup.PagingInfo.Offset)
                            .Take(lookup.PagingInfo.Size);
                    }
                }

                if (lookup.Fields != null)
                {
                    var parameter = Expression.Parameter(typeof(WorkStation), "e");
                    var bindings = lookup.Fields.Select(x =>
                    {
                        var internalProperty = typeof(WorkStation).GetProperty(x);
                        var expression = Expression.Property(parameter, internalProperty);
                        return Expression.Bind(internalProperty, expression);
                    });
                    var body = Expression.MemberInit(Expression.New(typeof(WorkStation)), bindings);
                    var selector = Expression.Lambda<Func<WorkStation, WorkStation>>(body, parameter);
                    workstations = workstations.Select(selector);
                   
                }

                return await _builder.Build(await workstations.ToListAsync(), lookup.Fields);
         
            }

           
        }

        public async Task<ActionResult<WorkStationModel>> GetWorkStation(Guid id)
        {
            //return await _context.WorkStations.FindAsync(id);
            var workStation = await _context.WorkStations.FindAsync(id);
       
            var workStationList = new List<WorkStation>
            {
                workStation
            };
            var workStationModels = await _builder.Build(workStationList, null);

            return workStationModels.Value.First();
        }

        public async Task<int> PutWorkStation(Guid id, WorkStationModelPersist workStationModel)
        {
            _context.Entry(_builder.BuildData(workStationModel, id)).State = EntityState.Modified;

            return await _context.SaveChangesAsync();

        }

        public async Task<int> AddSave(WorkStationModelPersist workStationModel)
        {
            _context.WorkStations.Add(await _builder.BuildData(workStationModel,default));
            return await _context.SaveChangesAsync();
        }



        public async Task<ActionResult<WorkStationModelPersist>> RemoveWorkStation(WorkStationModelPersist workStationModel)
        {
            var workStation =  await _context.WorkStations.FindAsync(workStationModel.Id);
           // _context.WorkStations.Remove(workStation);
           await _deleter.Delete(new List<Guid>
           {
               workStation.Id
           });
            //await _context.SaveChangesAsync();
            return workStationModel;
        }

        public async ValueTask<WorkStationModelPersist> FindWorkStation(Guid id)
        {
            var workStation =  await _context.WorkStations.FindAsync(id);
            var branch = await _context.Branches.FindAsync(workStation.BranchId);

            return _builder.BuildModel(workStation,branch.Id);
        }

        public bool WorkStationExists(Guid id)
        {
            return _context.WorkStations.Any(e => e.Id == id);
        }

    }
}