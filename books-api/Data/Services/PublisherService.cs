using books_api.Data.Models;
using books_api.Data.ViewModels;
using books_api.Exceptions;
using my_books.Data.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace books_api.Data.Services
{
    public class PublisherService
    {
        private AppDbContext _context;

        public PublisherService(AppDbContext context)
        {
            _context = context;
        }


        public List<Publisher> GetAllPublishers(string sortBy, string searchString, int? pageNumber)
        {
            var allPublishers = _context.Publishers.OrderBy(n => n.Name).ToList();

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "name_desc":
                        allPublishers = allPublishers.OrderByDescending(n => n.Name).ToList();
                        break;
                    default:
                        break;
                }
            }


            if (!string.IsNullOrEmpty(searchString))
            {
                allPublishers = allPublishers.Where(n => n.Name.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)).ToList();
            }

            // Paging
            int pageSize = 5;
            allPublishers = PaginatedList<Publisher>.Create(allPublishers.AsQueryable(), pageNumber ?? 1, pageSize);

            return allPublishers;
        }



        public Publisher AddPublisher(PublisherVM publisher)
        {
            if (StringStartsWithNumber(publisher.Name))
            {
                throw new PublisherNameException("Name starts with number",publisher.Name);
            }

            var _publisher = new Publisher()
            {

                Name = publisher.Name

            };

            _context.Publishers.Add(_publisher);
            _context.SaveChanges();

            return _publisher;
        }

        public void GetAllAuthors() => _context.Authors.ToList();

        public PublisherWithBooksAndAuthorsVM GetPublisherData(int publisherId)
        {
            var _publisherData = _context.Publishers.Where(n => n.Id == publisherId)
                .Select(n => new PublisherWithBooksAndAuthorsVM()
                {
                    Name = n.Name,
                    BookAuthors = n.Books.Select(n => new BookAuthorVM()
                    {
                        BookName = n.Title,
                        BookAuthors = n.Book_Authors.Select(n => n.Author.FullName).ToList()
                    }).ToList()
                }).FirstOrDefault();

            return _publisherData;
        }

        public void DeletePublisherById(int id)
        {
            var _publisher = _context.Publishers.FirstOrDefault(n=>n.Id == id);

            if (_publisher != null)
            {
                _context.Publishers.Remove(_publisher);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception($"Publisher with id {id} does not exsist!");
            }
        }

        public Publisher GetPublisherById(int id)
        {
            return _context.Publishers.FirstOrDefault(n=>n.Id == id);
        }

        private bool StringStartsWithNumber(string name) => Regex.IsMatch(name, @"^\d");


    }
}
