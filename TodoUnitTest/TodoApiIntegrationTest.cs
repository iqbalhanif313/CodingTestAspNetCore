using CodingTest;
using CodingTest.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TodoUnitTest
{
    public class TodoApiIntegrationTest
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
        /// Then you can run this Project testing
        /// 
        /// </summary>
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public TodoApiIntegrationTest()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();
        }


        [Test]
        public async Task Test_Create_Todo()
        {
            Todo todo1 = new Todo()
            {
                Title = "Todo 1",
                Description = "Todo 1 Description",
                Expiry = DateTime.Now,
                Completion = 30
            };
            Todo todo2 = new Todo()
            {
                Title = "Todo 2",
                Description = "Todo 2 Description",
                Expiry = DateTime.Now,
                Completion = 30
            };

            Todo todo3 = new Todo()
            {
                Title = "Todo 3",
                Description = "Todo 3 Description",
                Expiry = DateTime.Now,
                Completion = 30
            };

            Todo todo4 = new Todo()
            {
                Title = "Todo 4",
                Description = "Todo 4 Description",
                Expiry = DateTime.Now,
                Completion = 30
            };
            Todo todo5 = new Todo()
            {
                Title = "Test",
                Description = "Todo 5 Description",
                Expiry = DateTime.Now,
                Completion = 30
            };

            List<Todo> todoList = new List<Todo>();
            todoList.Add(todo1);
            todoList.Add(todo2);
            todoList.Add(todo3);
            todoList.Add(todo4);
            todoList.Add(todo5);
            var response = new HttpResponseMessage();
            foreach (Todo todo in todoList)
            {
                var stringContent = JsonConvert.SerializeObject(todo, Formatting.Indented);
                response = await _client.PostAsync("/todoes"
                        , new StringContent(stringContent
                        ,
                    Encoding.UTF8,
                    "application/json"));

                response.EnsureSuccessStatusCode();
            }
           

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);


        }


        [Test]
        public async Task Test_Get_All()
        {

            var response = await _client.GetAsync("/todoes");
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var responseString = await response.Content.ReadAsStringAsync();
        }

        [Test]
        public async Task Test_Get_Specific_Todo()
        {
            var id = 3;
            var response = await _client.GetAsync("/todoes/"+id);
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var responseString = await response.Content.ReadAsStringAsync();
        }

        [Test]

        public async Task Test_Get_Incoming_Todo()
        {
            var id = 3;
            var response = await _client.GetAsync("/todoes/incoming/today");
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            response = await _client.GetAsync("/todoes/incoming/nextday");
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            response = await _client.GetAsync("/todoes/incoming/currentweek");
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task Test_Update_Todo()
        {

            var id = 3;//<<<<<<<<<There must be data which has this ID at database
            var response = await _client.PutAsync("/todoes/" + id
                    , new StringContent(
                    JsonConvert.SerializeObject(new Todo()
                    {
                        Id = id,
                        Title = "Test NEW EDITTED",
                        Description = "John NEW EDITTED",
                        Expiry = DateTime.Now,
                        Completion = 30
                    }),
                Encoding.UTF8,
                "application/json"));

            response.EnsureSuccessStatusCode();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task Test_Set_Completion_Todo()
        {
            var id = 3;//<<<<<<<<<There must be data which has this ID at database
            var completion = 30;//<<<<<<<<<< Set Completion into 30%

            var response = await _client.PutAsync("/todoes/set-completion/" + id + "/" + completion
                , new StringContent(""));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        }
        [Test]
        public async Task Test_Set_Complete_Todo()
        {
            var id = 3;//<<<<<<<<<There must be data which has this ID at database

            var response = await _client.PutAsync("/todoes/complete/" + id
                , new StringContent(""));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        [Test]
        public async Task Test_Delete_Todo()
        {
            var id = 2;//<<<<<<<<<There must be data which has this ID at database

            var response = await _client.DeleteAsync("/todoes/" + id);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
