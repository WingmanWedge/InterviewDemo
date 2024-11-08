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
    }
}
