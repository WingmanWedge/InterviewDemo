using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterviewDemo
{
    public  class Moviegoer
    {
        public required string Name { get; set; }
        public required DateTime BirthDate { get; set; }
        public List<Movie>? ViewingHistory { get; set; }
        public int Age
            {
                get
                {
                    var today = DateTime.Today;
                    var age = today.Year - BirthDate.Year;
                    if (BirthDate.Date > today.AddYears(-age)) age--;
                    return age;
                }
        }
    }
}
