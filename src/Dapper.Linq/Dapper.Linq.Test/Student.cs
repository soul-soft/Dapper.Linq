namespace Dapper.Linq.Test
{
    [Table("student")]
    public class Student
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string? Name { get; set; }
    }
}
