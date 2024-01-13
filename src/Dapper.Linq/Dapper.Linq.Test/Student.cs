namespace Dapper.Linq.Test
{
    [Table("student")]
    public class Student
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string? Name { get; set; }
        [Column("(st.id + 1)")]
        public int? Gs { get; set; }
    }
}
