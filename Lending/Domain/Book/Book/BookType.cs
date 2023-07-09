﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain;

namespace BookAggregate
{
    public class BookType : Enumeration
    {
        public static BookType Restricted = new(1, nameof(Restricted));
        public static BookType Circulating = new(2, nameof(Circulating));

        public BookType(int id, string name)
            : base(id, name)
        {
        }

    }
}