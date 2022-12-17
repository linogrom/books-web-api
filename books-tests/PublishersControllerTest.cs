using books_api.Controllers;
using books_api.Data;
using books_api.Data.Models;
using books_api.Data.Services;
using books_api.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace books_tests
{
    public class PublishersControllerTest
    {
        private static DbContextOptions<AppDbContext> dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "BookDbControllerTest")
            .Options;



        AppDbContext context;
        PublisherService publisherService;
        PublisherController publishersController;

        [OneTimeSetUp]
        public void Setup()
        {
            context = new AppDbContext(dbContextOptions);
            context.Database.EnsureCreated();

            SeedDataBase();
            publisherService = new PublisherService(context);
            publishersController = new PublisherController(publisherService, new NullLogger<PublisherController>());
        }

        [Test, Order(1)]
        public void HTTPGET_GetAllPublishers_WithSortBySearchPageNr_ReturnOk_Test()
        {
            IActionResult actionResult = publishersController.GetAllPublishers("name_desc", "Publisher", 1);
            Assert.That(actionResult, Is.TypeOf<OkObjectResult>());
            var actionResultData = (actionResult as OkObjectResult).Value as List<Publisher>;
            Assert.That(actionResultData.First().Name, Is.EqualTo("Publisher 6"));
            Assert.That(actionResultData.First().Id, Is.EqualTo(6));
            Assert.That(actionResultData.Count, Is.EqualTo(5));

            IActionResult actionResultSecondPage = publishersController.GetAllPublishers("name_desc", "Publisher", 2);
            Assert.That(actionResultSecondPage, Is.TypeOf<OkObjectResult>());
            var actionResultDataSecondPage = (actionResultSecondPage as OkObjectResult).Value as List<Publisher>;
            Assert.That(actionResultDataSecondPage.First().Name, Is.EqualTo("Publisher 1"));
            Assert.That(actionResultDataSecondPage.First().Id, Is.EqualTo(1));
            Assert.That(actionResultDataSecondPage.Count, Is.EqualTo(1));
        }

        [Test, Order(2)]
        public void HTTPGET_GetPublsherById_ReturnOk_Test()
        {
            int publisherId = 1;

            IActionResult actionresult = publishersController.GetPublisherById(publisherId);

            Assert.That(actionresult, Is.TypeOf<OkObjectResult>());


            var publisherData = (actionresult as ObjectResult).Value as Publisher;
            Assert.That(publisherData.Id, Is.EqualTo(1));
            Assert.That(publisherData.Name, Is.EqualTo("publisher 1").IgnoreCase);
        }

        [Test, Order(3)]
        public void HTTPGET_GetPublsherById_ReturnNotFound_Test()
        {
            int publisherId = 99;

            IActionResult actionresult = publishersController.GetPublisherById(publisherId);

            Assert.That(actionresult, Is.TypeOf<NotFoundResult>());

        }

        [Test, Order(4)]
        public void HTTPPOST_AddPublisher_ReturnsCreated_Test()
        {
            var newPublisherVM = new PublisherVM()
            {
                Name = "New publisher"
            };

            IActionResult acitonResult = publishersController.AddPublisher(newPublisherVM);

            Assert.That(acitonResult, Is.TypeOf<CreatedResult>());
        }

        [Test, Order(5)]
        public void HTTPPOST_AddPublisher_ReturnsBadRequest_Test()
        {
            var newPublisherVM = new PublisherVM()
            {
                Name = "123 New publisher"
            };

            IActionResult acitonResult = publishersController.AddPublisher(newPublisherVM);

            Assert.That(acitonResult, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test, Order(6)]
        public void HTTPDELETE_DeletePublisherById_ReturnsOk_Test()
        {
            var publisherId = 1;

            IActionResult actionResult = publishersController.DeletePublisherById(publisherId);

            Assert.That(actionResult, Is.TypeOf<OkResult>());
        }

        [Test, Order(6)]
        public void HTTPDELETE_DeletePublisherById_ReturnsBadRequest_Test()
        {
            var publisherId = 6;

            IActionResult actionResult = publishersController.DeletePublisherById(publisherId);

            Assert.That(actionResult, Is.TypeOf<BadRequestObjectResult>());
        }


        private void SeedDataBase()
        {
            var publishers = new List<Publisher>
            {
                    new Publisher() {
                        Id = 1,
                        Name = "Publisher 1"
                    },
                    new Publisher() {
                        Id = 2,
                        Name = "Publisher 2"
                    },
                    new Publisher() {
                        Id = 3,
                        Name = "Publisher 3"
                    },
                    new Publisher() {
                        Id = 4,
                        Name = "Publisher 4"
                    },
                    new Publisher() {
                        Id = 5,
                        Name = "Publisher 5"
                    },
                    new Publisher() {
                        Id = 6,
                        Name = "Publisher 6"
                    },
            };
            context.Publishers.AddRange(publishers);

            context.SaveChanges();
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            context.Database.EnsureDeleted();
        }
    }
}
