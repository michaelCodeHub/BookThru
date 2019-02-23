using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookThru.Models
{
    public class CreateModel
    {
        public Book Book { get; set; }
        public List<Category> Categories { get; set; }
        public List<CourseCode> CourseCodes { get; set; }
    }
}
