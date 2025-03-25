using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EmployeeAPI
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private const string connectionUri = "mongodb+srv://samiran96adak:z3HghoC5PQMcHvRk@cluster0.qz8uv.mongodb.net/?appName=Cluster0";
        private readonly MongoClient _client;
        private readonly IMongoCollection<Employee> _employeeCollection;

        public EmployeeController()
        {
            var settings = MongoClientSettings.FromConnectionString(connectionUri);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            _client = new MongoClient(settings);
            var database = _client.GetDatabase("EmployeeDb");
            _employeeCollection = database.GetCollection<Employee>("EmployeeDb");
        }

        // API to fetch all employees
        [HttpGet("GetEmployees")]
        public IActionResult GetEmployees()
        {
            try
            {
                // Fetch all employees
                var employees = _employeeCollection.Find(_ => true).ToList();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                // Return the error message if something goes wrong
                return BadRequest(ex.Message);
            }
        }

        // API to upsert an employee (update if exists, insert if not)
        [HttpPut("UpsertEmployee")]
        public IActionResult UpsertEmployee([FromBody] Employee updatedEmployee)
        {
            if (updatedEmployee == null)
            {
                return BadRequest("Employee data is required.");
            }

            try
            {
                // Define the filter to find the employee by their unique Id
                var filter = Builders<Employee>.Filter.Eq(emp => emp.Id, updatedEmployee.Id);

                // Define the update operation
                var update = Builders<Employee>.Update
                    .Set(emp => emp.Name, updatedEmployee.Name)
                    .Set(emp => emp.Email, updatedEmployee.Email)
                    .Set(emp => emp.Phone, updatedEmployee.Phone)
                    .Set(emp => emp.Gender, updatedEmployee.Gender)
                    .Set(emp => emp.Department, updatedEmployee.Department)
                    .Set(emp => emp.Skills, updatedEmployee.Skills);

                // Perform the upsert operation
                var result = _employeeCollection.UpdateOne(filter, update, new UpdateOptions { IsUpsert = true });

                // Return a response based on whether an update or insert occurred
                if (result.UpsertedId != null)
                {
                    return Ok(new { message = "Employee data inserted successfully!" });
                }

                return Ok(new { message = "Employee data updated successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // API to delete an employee
        [HttpDelete("DeleteEmployee/{id}")]
        public IActionResult DeleteEmployee(string id)
        {
            try
            {
                // Define the filter to find the employee by their unique Id
                var filter = Builders<Employee>.Filter.Eq(emp => emp.Id, id);

                // Perform the delete operation
                var result = _employeeCollection.DeleteOne(filter);

                // Check if the delete was successful
                if (result.DeletedCount == 0)
                {
                    return NotFound(new { message = "Employee not found." });
                }

                return Ok(new { message = "Employee deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    
}
