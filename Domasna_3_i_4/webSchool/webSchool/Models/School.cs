using System.ComponentModel.DataAnnotations;

namespace webSchool.Models
{
    public class School
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public string city { get; set; }
        [Required]
        public string street { get; set; }
        public string modules { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string imageUrl { get; set; }
        public string contact { get; set; }
        public string email { get; set; }
        public string teachers { get; set; }
        public string workTime { get; set; }
        public int numOfStudents { get; set; }

        public School (string name, string city, string street, string modules, double latitude, double longitude,
             string imageUrl, string contact, string email, string teachers, string workTime, int numOfStudents)
        {
            this.name = name;
            this.imageUrl = imageUrl;
            this.city = city;
            this.street = street;  
            this.modules = modules;
            this.latitude = latitude;
            this.longitude = longitude;
            this.contact = contact;
            this.email = email;
            this.teachers = teachers;
            this.workTime = workTime;
            this.numOfStudents = numOfStudents;

            
        }

        public School() { }
    }
}
