using books_api.Data.Models;
using books_api.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace books_api.Data.Services
{
    public class AuthorsService 
    {
        private AppDbContext _context;

        public AuthorsService(AppDbContext context)
        {
            _context = context;
        }


        public void AddAuthor(AuthorVM author)
        {
            var _author = new Author()
            {

                FullName = author.FullName
                
            };

            _context.Authors.Add(_author);
            _context.SaveChanges();
        }

        public void GetAllAuthors() => _context.Authors.ToList();

        public AuthorWithBooksVM GetAuthorWithBooks(int authorId)
        {
            var author = _context.Authors.Where(n => n.Id == authorId).Select(n => new AuthorWithBooksVM()
            {
                FullName = n.FullName,
                BooksNames = n.Book_Authors.Select(n=> n.Book.Title).ToList()
            }).FirstOrDefault();

            return author;
        }
    }
}
