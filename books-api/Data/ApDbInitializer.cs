using books_api.Data.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace books_api.Data
{
    public class ApDbInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>(); //creating context reference 

                if (!context.Books.Any())
                {
                    context.Books.AddRange(new Book() 
                    { 
                        Title = "Book title 1",
                        Description = "Book description 1",
                        IsRead = true,
                        DateRead = DateTime.Now.AddDays(-10),
                        Rate = 4,
                        Genre = "Biography",
                        CoverUrl = "https..",
                        DateAdded = DateTime.Now

                    },
                    new Book()
                    {
                        Title = "Book title 2",
                        Description = "Book description 2",
                        IsRead = false,
                        Genre = "Biography",
                        CoverUrl = "https..",
                        DateAdded = DateTime.Now
                    }
                    );

                    context.SaveChanges();
                }
            }
        }
    }
}
