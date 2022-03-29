using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ToDoAPI.API.Models;//for access to the vm's (Data Transfer Objects)
using ToDoAPI.DATA.EF;
using System.Web.Http.Cors;//This allows us to modify CORS permissions to what we need in this app


namespace ResourceAPI.API.Controllers
{
    //Upon creating this controller (Web API 2 controller), We need to add some functionality to the controllers. 
    //1. We need to add using statements for our models
    //2. Add a using for the data layer
    //3. Install-Package Microsoft.AspNet.WebApi.Cors
    //4. Navigate to the App_Start/WebApiConfig.cs and add a line of code to allow Cross Origin Resource Sharing globally
    //5. Add a Cors using statement 
    //6. Add the code below to limit who can request data from our API

    //In the code below, we are gining permission to specific URLs(origins), specific types of data (headers), and specific
    //methods(GET/POST/PUT/DELETE).
    //GET = READ
    //POST = CREATE
    //PUT = EDIT
    //DELETE = DELETE
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ToDoController : ApiController
    {

        //Create a connection to the db
        ToDoEntities db = new ToDoEntities();

        //GET - /api/resources
        public IHttpActionResult GetToDos()
        {
            //Below we create a list of Entity Framework resource objects. In an API, it is best practice to install Entity
            //Framework to the API layer when needing to accomplish this task. 
            List<ToDoItemViewModel> resources = db.TodoItems.Include("Category").Select(t => new ToDoItemViewModel()
            {
                //Assign the columns of the Resources db Table to the ResourceViewModel object, so we can use the data (send the data back to the 
                //requesting app)
                ToDoId = t.TodoId,
                Action = t.Action,
                Done = t.Done,
                CategoryId = t.CategoryId,
                Category = new CategoryViewModel()
                {
                    CategoryId = t.Category.CategoryId,
                    CategoryName = t.Category.Name,
                    CategoryDescription = t.Category.Description
                }
            }).ToList<ToDoItemViewModel>();

            //Check the results and handle accordingly below
            if (resources.Count == 0)
            {
                return NotFound();
            }
            //Everything is good, return the data
            return Ok(resources);//resources are being passed in the response back to the requesting app. 
        }//end GetResources()

        //GET - api/resources/id
        public IHttpActionResult GetToDo(int id)
        {
            //Create a new ResourceViewModel object and assign it to the appropriate resoure from the db
            ToDoItemViewModel toDoItem = db.TodoItems.Include("Category").Where(t => t.TodoId == id).Select(t =>
                new ToDoItemViewModel()
                {
                    //COPY THE ASSIGNMENTS FROM THE GetResources() and paste below
                    ToDoId = t.TodoId,
                    Action = t.Action,
                    Done = t.Done,
                    CategoryId = t.CategoryId,
                    Category = new CategoryViewModel()
                    {
                        CategoryId = t.Category.CategoryId,
                        CategoryName = t.Category.Name,
                        CategoryDescription = t.Category.Description
                    }
                }).FirstOrDefault();

            //scopeless if - once the return excutes the scopes are closed.
            if (toDoItem == null)
                return NotFound();

            return Ok(toDoItem);
        }//end GetResource

        //POST - apt/Resources (HttpPost)
        public IHttpActionResult PostResource(ToDoItemViewModel toDoItem)
        {
            //1. Check to validate the object - we need to know that all the data necessary to create a resource is there.
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }//end if 

            TodoItem newtoDoItem = new TodoItem()
            {
                TodoId = toDoItem.ToDoId,
                Action = toDoItem.Action,
                Done = toDoItem.Done,
                CategoryId = toDoItem.CategoryId
            };

            //add the record and save changes
            db.TodoItems.Add(newtoDoItem);
            db.SaveChanges();

            return Ok(newtoDoItem);

        }//end PostResource


        //PUT - api/resources (HTTPPut)
        public IHttpActionResult PutResource(ToDoItemViewModel toDoItem)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            //Get the resource from the db so we can modify it
            TodoItem existingTodoItem = db.TodoItems.Where(t => t.TodoId == toDoItem.ToDoId).FirstOrDefault();

            //modify the resource 
            if (existingTodoItem != null)
            {
                existingTodoItem.TodoId = toDoItem.ToDoId;
                existingTodoItem.Action = toDoItem.Action;
                existingTodoItem.Done = toDoItem.Done;
                existingTodoItem.CategoryId = toDoItem.CategoryId;
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }



        }//end Put

        //DELETE - api/Resources/id (HTTPDelete)
        public IHttpActionResult DeleteTodoItem(int id)
        {
            //Get the resource from the API to make sure there's a resource with this id
            TodoItem todoItem = db.TodoItems.Where(t => t.TodoId == id).FirstOrDefault();

            if (todoItem != null)
            {
                db.TodoItems.Remove(todoItem);
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }//end Delete

        //We use the Dispose() below to dispose of any connections to the database after we are done with them - best
        //practice to handle performance - dispose of teh instance of the controller and db connection when we are done with
        //it. 
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }//end class
}//end namespace
