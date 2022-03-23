
using System;
using System.Collections;
using System.Collections.Generic;
using TodoApi2.Data;
using Microsoft.AspNetCore.Mvc;
using TodoApi2.Service;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using TodoApi2.ExceptionHandle;

namespace TodoApi2.Models.Builders
{
    public class WorkStationBuilder
    {
        private MyAppContext _context { get; set; }
        private readonly IStringLocalizer<WorkStationBuilder> _localizer;
        public WorkStationBuilder(MyAppContext context, IStringLocalizer<WorkStationBuilder> localizer)
        {
            _context = context;
            _localizer = localizer;

        }
        public async Task<ActionResult<IEnumerable<WorkStationModel>>> Build(IEnumerable<WorkStation> items, string[] fields)
        {
            var WorkStationsModels = new List<WorkStationModel>();

            var branches = _context.Branches
                .Where(x => items.Select(y => y.BranchId).Contains(x.Id))
                .ToDictionary(x => x.Id);
            if (fields!=null)
            {
                foreach (var item in items)
                {
                    Branch branch = branches[item.BranchId];

                    WorkStationModel workStationModel = new WorkStationModel();
                    if (fields.Contains(nameof(WorkStationModel.Id))) workStationModel.Id = item.Id;
                    if (fields.Contains(nameof(WorkStationModel.Name))) workStationModel.Name = item.Name;
                    if (fields.Contains(nameof(WorkStationModel.BranchId))) workStationModel.BranchId = item.BranchId;
                    if (fields.Contains(nameof(WorkStationModel.BranchName))) workStationModel.BranchName = branch.Name;

                    WorkStationsModels.Add(workStationModel);
                }
            }
            else
            {
                foreach (var item in items)
                {
                    Branch branch = branches[item.BranchId];

                    WorkStationModel workStationModel = new WorkStationModel
                    {
                        Id = item.Id,
                        Name = item.Name,
                        BranchId = item.BranchId,
                        BranchName = branch.Name
                    };

                    WorkStationsModels.Add(workStationModel);
                }
            }


            return WorkStationsModels;
        }

        public async Task<WorkStation> BuildData(WorkStationModelPersist workStationModel, Guid id)
        {
            WorkStation workStation = null;

            if (id != default)
            {
                workStation = await _context.WorkStations.FindAsync(id);
                if (workStation == null)
                {
                    var returnString = _localizer["No workstation"];
                    throw new MyException(returnString);
                }

                workStation.Name = workStationModel.Name;
                workStation.BranchId = workStationModel.BranchId;
                workStation.Id = id;

                _context.WorkStations.Update(workStation);
                await _context.SaveChangesAsync();
            }
            else
            {
                workStation = new WorkStation
                {
                    Name = workStationModel.Name,
                    BranchId = workStationModel.BranchId,
                    Id = workStationModel.Id
                };
            }

            if (id == default)
                workStation.Id = workStationModel.Id;
            else
                workStation.Id = id;

            return workStation;
        }

        public WorkStationModelPersist BuildModel(WorkStation workStation, Guid memberId)
        {
            WorkStationModelPersist workStationModel = new WorkStationModelPersist
            {
                Id = workStation.Id,
                Name = workStation.Name,
                BranchId = memberId
            };
            return workStationModel;
        }
    }
}