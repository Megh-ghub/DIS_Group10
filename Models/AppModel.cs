using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DIS_Group10.Models
{
    public class Park
    {
        public string ID { get; set; }
        public string url { get; set; }
        public string fullName { get; set; }
        public string parkCode { get; set; }
        public string description { get; set; }
        public ICollection<StatePark> states { get; set; }
        public ICollection<ParkActivity> activities { get; set; }

    }

    public class Activity
    {
        public string ID { get; set; }
        public string name { get; set; }
        public ICollection<ParkActivity> parks { get; set; }
    }

    public class ParkActivity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public Activity activity { get; set; }
        public Park park { get; set; }
    }

    public class State
    {
        public string ID { get; set; }
        public string name { get; set; }
        public ICollection<StatePark> parks { get; set; }
    }

    public class StatePark
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public State state { get; set; }
        public Park park { get; set; }
    }

    public class AddNewPark
    {
        public string ID { get; set; }
        [Required]
        [Url]
        public string url { get; set; }
        [Required]
        public string fullName { get; set; }
        [Required]
        public string parkCode { get; set; }
        [Required]
        public string description { get; set; }
        [Required]
        public ICollection<string> statenames { get; set; }
        [Required]
        public ICollection<string> activitynames { get; set; }
    }
}