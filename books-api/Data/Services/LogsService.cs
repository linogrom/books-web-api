﻿using books_api.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace books_api.Data.Services
{
    public class LogsService
    {
        private AppDbContext _context;

        public LogsService(AppDbContext context)
        {
            _context = context;
        }

        public List<Log> GetAllLogsFromDb() => _context.Logs.ToList();

    }
}
