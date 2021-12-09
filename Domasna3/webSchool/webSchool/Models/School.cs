using System.ComponentModel.DataAnnotations;

namespace webSchool.Models
{
    public class School
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string name { get; set; }
        public string imageUrl { get; set; }
        [Required]
        public string city { get; set; }
        [Required]
        public string street { get; set; }
        public string modules { get; set; }
        
        public School (string name, string imageUrl, string city, string street, string modules)
        {
            this.name = name;
            this.imageUrl = imageUrl;
            this.city = city;
            this.street = street;  
            this.modules = modules;
        }

        public School() { }
    }
}
