﻿using Newtonsoft.Json;
using Northwind.Infrastructure;

namespace Northwind.Web.Controllers.Organization
{
    using System.Web.Mvc;
    using Domain.Contracts.Tasks;

    using SharpArch.NHibernate.Web.Mvc;
    using SharpArch.Web.Mvc.JsonNet;
    using ViewModels;

    [HandleError]
    public class EmployeeRiaController : Controller
    {
        private readonly IEmployeeTasks employeeTasks;
        private readonly ITerritoryTasks territoryTasks;

        public EmployeeRiaController(IEmployeeTasks employeeTasks, ITerritoryTasks territoryTasks) 
        {
            this.employeeTasks = employeeTasks;
            this.territoryTasks = territoryTasks;
        }

        [HttpGet]
        public ActionResult Index() 
        {
            return View();
        }

        [Transaction]
        [HttpGet]
        public JsonNetResult GetEmployeeFormViewModel() 
        {
            var employeeViewModel = new ViewEmployeesViewModel 
            {
                Employees = this.employeeTasks.GetAllEmployees(),
                AvailableTerritories = this.territoryTasks.GetTerritories()
            };

            return new JsonNetResult { Data = employeeViewModel };
        }

        [Transaction]
        [HttpPost]
        public JsonNetResult Edit(CreateEmployeeViewModel createEmployeeViewModel) 
        {
            this.employeeTasks.RiaCreateOrUpdate(createEmployeeViewModel.Employee, createEmployeeViewModel.TerritoriesString);

            var serializer = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new NHibernateContractResolver(),
            };

            var jsonNetResult = new JsonNetResult
            {
                Data = createEmployeeViewModel.Employee,
                SerializerSettings = serializer
            };

            return jsonNetResult;
        }

        [Transaction]
        [HttpPost]
        public JsonNetResult Delete(int id) 
        {
            this.employeeTasks.Delete(id);
            return new JsonNetResult { Data = this.employeeTasks.GetAllEmployees() };
        }
    }
}