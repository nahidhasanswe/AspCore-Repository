using System.ComponentModel.DataAnnotations;

namespace AspNetCore.Sample.Service.Model
{
    public class TestClass
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}