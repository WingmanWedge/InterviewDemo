using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterviewDemo
{
    public class Movie
    {
        public required string Name { get; set; }
        public required string Genre { get; set; }
        public required DateTime ReleaseDate { get; set; }
        public required MPAARating Rating { get; set; }

        /// <summary>if a movie is featured, this will be populated</summary>
        public DateTime? FeatureStartDate { get; set; }
    }    
}
