using MovieManager.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieManager.Core.DataTransferObjects
{
    public class MovieCategoryDto
    {
        public Category Category { get; set; }
        public int Count { get; set; }
        public int OverallDuration { get; set; }
    }
}
