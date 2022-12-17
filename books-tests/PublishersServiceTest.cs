using books_api.Data;
using books_api.Data.Models;
using books_api.Data.Services;
using books_api.Data.ViewModels;
using books_api.Exceptions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace books_tests
{
    public class PublishersServiceTest
    {
        private static DbContextOptions<AppDbContext> dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "BookDbTest")
            .Options;


        AppDbContext context;
        PublisherService publisherService;

        [OneTimeSetUp]
        public void Setup()
        {
            context = new AppDbContext(dbContextOptions);
            context.Database.EnsureCreated();

            SeedDataBase();
            publisherService = new PublisherService(context);
        }

        [Test, Order(1)]
        public void GetAllPublishers_WithNoSortBy_WithNoSearchString_WithNoPageNumber_Test()
        {
            var result = publisherService.GetAllPublishers("", "", null);

            Assert.That(result.Count, Is.EqualTo(5));
        }

        [Test, Order(2)]
        public void GetAllPublishers_WithNoSortBy_WithNoSearchString_WithPageNumber_Test()
        {
            var result = publisherService.GetAllPublishers("", "", 2);

            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test, Order(3)]
        public void GetAllPublishers_WithNoSortBy_WithSearchString_WithNoPageNumber_Test()
        {
            var result = publisherService.GetAllPublishers("", "3", null);

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.FirstOrDefault().Name, Is.EqualTo("Publisher 3"));
        }

        [Test, Order(4)]
        public void GetAllPublishers_WithSortBy_WithNoSearchString_WithNoPageNumber_Test()
        {
            var result = publisherService.GetAllPublishers("name_desc", "", null);

            Assert.That(result.Count, Is.EqualTo(5));
            Assert.That(result.FirstOrDefault().Name, Is.EqualTo("Publisher 6"));
        }

        [Test, Order(5)]
        public void GetPublisherById_WithResponse_Test()
        {
            var result = publisherService.GetPublisherById(1);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Name, Is.EqualTo("Publisher 1"));
        }

        [Test, Order(6)]
        public void GetPublisherById_WithoutResponse_Test()
        {
            var result = publisherService.GetPublisherById(99);
            Assert.That(result, Is.Null);
        }


        [Test, Order(7)]
        public void AddPublisher_WithException_Test()
        {
            var newPublisher = new PublisherVM()
            {
                Name = "123 with Exception"
            };

            Assert.That(() => publisherService.AddPublisher(newPublisher), Throws.Exception.TypeOf<PublisherNameException>()
                .With.Message.EqualTo("Name starts with number")
                );
        }


        [Test, Order(8)]
        public void AddPublisher_WithoutException_Test()
        {
            var newPublisher = new PublisherVM()
            {
                Name = "Without Exception"
            };

            var result = publisherService.AddPublisher(newPublisher);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Does.StartWith("Without"));
            Assert.That(result.Id.ToString(), Is.Not.Null);
        }


        [Test, Order(9)]
        public void GetPublisherData_Test()
        {
            var result = publisherService.GetPublisherData(1);

            Assert.That(result.Name, Is.EqualTo("Publisher 1"));
            Assert.That(result.BookAuthors, Is.Not.Empty);
            Assert.That(result.BookAuthors.Count, Is.GreaterThan(0));
            var firstBookName = result.BookAuthors.OrderBy(n => n.BookName).FirstOrDefault().BookName;
            Assert.That(firstBookName, Is.EqualTo("Book 1 Title"));
        }

        [Test, Order(10)]
        public void DeletePublisherById_PublisherExists_Test()
        {
            int publisherId = 6;

            var publisherBefore = publisherService.GetPublisherById(publisherId);
            Assert.That(publisherBefore, Is.Not.Null);
            Assert.That(publisherBefore.Name, Is.EqualTo("Publisher 6"));

            publisherService.DeletePublisherById(publisherId);

            var publisherAfter = publisherService.GetPublisherById(publisherId);
            Assert.That(publisherAfter, Is.Null);
        }

        //[Test, Order(11)]
        //public void DeletePublisherById_PublisherDoesNotExists_Test()
        //{
        //    int publisherId = 6;

        //    Assert.That(() => publisherService.DeletePublisherById(publisherId), Throws.Exception.TypeOf<Exception>().With.Message.EqualTo($"The publisher with id: {publisherId} does not exist"));
        //}



        [OneTimeTearDown]
        public void CleanUp()
        {
            context.Database.EnsureDeleted();
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

            var authors = new List<Author>()
            {
                new Author()
                {
                    Id = 1,
                    FullName = "Author 1"
                },
                new Author()
                {
                    Id = 2,
                    FullName = "Author 2"
                }
            };
            context.Authors.AddRange(authors);


            var books = new List<Book>()
            {
                new Book()
                {
                    Id = 1,
                    Title = "Book 1 Title",
                    Description = "Book 1 Description",
                    IsRead = false,
                    Genre = "Genre",
                    CoverUrl = "https://...",
                    DateAdded = DateTime.Now.AddDays(-10),
                    PublisherId = 1
                },
                new Book()
                {
                    Id = 2,
                    Title = "Book 2 Title",
                    Description = "Book 2 Description",
                    IsRead = false,
                    Genre = "Genre",
                    CoverUrl = "https://...",
                    DateAdded = DateTime.Now.AddDays(-10),
                    PublisherId = 1
                }
            };
            context.Books.AddRange(books);

            var books_authors = new List<Book_Author>()
            {
                new Book_Author()
                {
                    Id = 1,
                    BookId = 1,
                    AuthorId = 1
                },
                new Book_Author()
                {
                    Id = 2,
                    BookId = 1,
                    AuthorId = 2
                },
                new Book_Author()
                {
                    Id = 3,
                    BookId = 2,
                    AuthorId = 2
                },
            };
            context.Books_Authors.AddRange(books_authors);


            context.SaveChanges();
        }
    }
}
