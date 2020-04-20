using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodingTest.Data;
using CodingTest.Models;

namespace CodingTest.Controllers
{
    /// <summary>
    /// ATTENTION:
    /// This project is integration testing of Coding Test project 
    /// which is using SQL Server Database
    /// 
    /// Im sorry for not providing this project in docker
    /// because docker-desktop requires windows 10 OS while im still using windows 8.1 OS 
    /// and it's a bit complecated to integrated docker-toolbox with visual studio 2019
    /// 
    /// before run the project, please ensure that the database is migrated.
    /// to use migration, use syntax at Package Manager Console:
    /// 
    /// From the Tools menu, select NuGet Package Manager > Package Manager Console (PMC):
    /// In the PMC, enter the following commands:
    /// 
    /// PM> Update-Database
    /// 
    /// Then you can test the project manually or by using Test Project (TodoUnitTest) already provided
    /// 
    /// </summary>
    /// 

    [Route("[controller]")]
    [ApiController]
    public class TodoesController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoesController(TodoContext context)
        {
            _context = context;
        }


        //Required Operations:

        //Get All Todo’s
        //Get Specific Todo
        //Get Incoming ToDo(for today/next day/current week)
        //Create Todo
        //Update Todo
        //Set Todo percent complete
        //Delete Todo
        //Mark Todo as Done


        // GET: api/Todoes
        //Get All Todo’s
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Todo>>> GetTodo()
        {
            return await _context.Todo.ToListAsync();
        }

        //Get Specific Todo
        // GET: api/Todoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Todo>> GetTodo(int id)
        {
            var todo = await _context.Todo.FindAsync(id);

            if (todo == null)
            {
                return NotFound();
            }

            return todo;
        }

        //Get Incoming ToDo(for today/next day/current week)
        // GET: /Todoes/incoming/today => to get todo list for today
        // GET: /Todoes/incoming/nextday => to get todo list for tomorrow
        // GET: /Todoes/incoming/currentweek => to get todo list for this week
        [HttpGet("incoming/{inputString}")]
        [HttpGet("incoming")]
        public async Task<ActionResult<IEnumerable<Todo>>> GetIncomingTodo(string InputString)
        {
            var todo = from m in _context.Todo
                       select m;
            if (!string.IsNullOrEmpty(InputString))
            {
                if (InputString == "today")
                {
                    todo = todo.Where(s => s.Expiry > DateTime.Now & s.Expiry < DateTime.Now.AddDays(1));
                }
                else if (InputString == "nextday")
                {
                    todo = todo.Where(s => s.Expiry > DateTime.Now.AddDays(1) & s.Expiry < DateTime.Now.AddDays(2));
                }
                else if (InputString == "currentweek")
                {
                    todo = todo.Where(s => s.Expiry > DateTime.Now.AddDays(1) & s.Expiry < DateTime.Now.AddDays(7));
                }

            }

            return await todo.ToListAsync();
        }

        //Create Todo
        // POST: /Todoes
        /*With Request Body:
         {
              "title":"Meeting Editted",
              "description":"Meeting Description Editted",
              "expiry":"2020-04-21T02:52:00",
              "completion":25
            }
         */
        [HttpPost]
        public async Task<ActionResult<Todo>> PostTodo(Todo todo)
        {
            _context.Todo.Add(todo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTodo", new { id = todo.Id }, todo);
        }

        //Update Todo
        //Eg:
        //PUT: /Todoes/5
        /*With Request Body:
         {
              "id":5,
              "title":"Meeting Editted",
              "description":"Meeting Description Editted",
              "expiry":"2020-04-21T02:52:00",
              "completion":25
            }
             */

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodo(int id, Todo todo)
        {
            if (id != todo.Id)
            {
                return BadRequest();
            }

            _context.Entry(todo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Content("Data with id: " + id + " is Updated");
        }

        //Mark Todo as Done
        // PUT: Todoes/complete/5
        [HttpPut("complete/{id}")]
        public async Task<IActionResult> CompleteTodo(int id)
        {
            var todo = await _context.Todo.FindAsync(id);
            if (id != todo.Id)
            {
                return BadRequest();
            }
            todo.Completion = 100;
            _context.Entry(todo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Content("Data with id: " + id + " is set to 100% completion");
        }

        //Delete Todo
        // Eg:
        // DELETE: api/Todoes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Todo>> DeleteTodo(int id)
        {
            var todo = await _context.Todo.FindAsync(id);
            if (todo == null)
            {
                return NotFound();
            }

            _context.Todo.Remove(todo);
            await _context.SaveChangesAsync();

            return Content("Data with id: " + id + " is Deleted");
        }

        //Set Todo percent complete
        // Eg:
        // PUT: /Todoes/set-completion/5/30
        [HttpPut("set-completion/{id}/{completion}")]
        public async Task<IActionResult> SetCompletion(int id, decimal completion)
        {
            var todo = await _context.Todo.FindAsync(id);
            if (id != todo.Id)
            {
                return BadRequest();
            }
            todo.Completion = completion;
            _context.Entry(todo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Content("Data with id: " + id + " is set to " + completion + "% completion");
        }




        private bool TodoExists(int id)
        {
            return _context.Todo.Any(e => e.Id == id);
        }
    }
}
