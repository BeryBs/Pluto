using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace EntityFrProject
{
    public class Course
    {
        public int Id { get; set; } //CourseID
        public string Title { get; set; }
        public float FullPrice { get; set; }
        //[Required] Descriptionun null olmasını engeller.
        public string Description { get; set; }
        public CourseLevel Level { get; set; } //Navigation Property1 Bir dersin birden fazla seviyesi olabilir Beginner,Indermediate,Advanced..
        public Author Author { get; set; } //Navigation Property2 Bir ders birden fazla yazar tarafından hazırlanmış olabilir.
        public IList<Tag> Tags { get; set; } //Navigation Property3 Bir dersin birden fazla tag'i olabilir.

    }
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<Course> Courses { get; set; } //Bir yazar birden fazla ders vermiş olabilir.
    }
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public IList<Course> Courses { get; set; } //Bir tag birden çok kursa ait olabilir.


    }
    public enum CourseLevel
    {

        Beginner = 1,
        Intermediate = 2,
        Advanced = 3
    }

    public class PlutoContext : DbContext
    {
        public DbSet<Course> Courses { get; set; } //Veritabanındaki courses tablolarına erişim sağlanır.
        public DbSet<Tag> Tags { get; }
        public DbSet<Author> Authors { get; }

        public PlutoContext() //Constructorda app.config içinde verdiğimiz connection string ismini yazıyoruz.
            : base("name=DefaultConnection")
        {

        }

    }
    internal class Program
    {
        static void Main(string[] args)
        {
            PlutoContext p = new PlutoContext(); //DbContext veritabanı nesnesi
            Course c = new Course(); //c isminde courses tablosu 
            Author a = new Author();
            Tag t = new Tag();

            a.Name = "Bloom";
            a.Id = 4;
           
            c.Title = "Easy";
            c.Description = "MacQueen";
            c.FullPrice = 3;
            c.Level = CourseLevel.Beginner;

            t.Name = "A1/Level";
            t.Id = 5;
            c.Tags = new List<Tag>();   
            
           
            p.Courses.Add(c);
            c.Author = a;
            c.Tags.Add(t);
            t.ToString();   
            
            p.SaveChanges();
            //LINQ SYNTAX

            var query =
                from d in p.Courses //d geçici değişkenle veritabanındaki Courseslara erişim sağla 
                where d.Description.Contains("te") //Courses tablosunda descriptionı "te" içerenler için
                orderby d.Id //Id numaralarına göre sırala 
                select d; //ve d değerini al 

            foreach (var course in query)
                Console.WriteLine(course.Id);
           //EXTENSION METHODS

            var courses=p.Courses.Where(d=>d.Description.Contains("lla")).OrderBy(d=>d.Id);
                           

            foreach (var course in courses)
                Console.WriteLine(course.Description);

          

            //LINQ
            //We want to get all records that Id is higher than or equal to 3 and order tham by fullprice.
            //projection ile p'den başka bir class oluşturup daha az özellik döndürebiliriz

            var myquery =
                from d in p.Courses 
                where d.Id >= 3
                orderby d.FullPrice ascending
                select new {Id=d.Id, FullPrice=d.FullPrice};    //Burada d classını kullanmak yerine anonymus bir class kullanarak id ve fullprice özelliklerini verdik.

            var mysquery =
                 from d in p.Courses
                 group d by d.FullPrice into g //Group by ile g isimli değikene gönderilir
                 select g;

            foreach (var group in mysquery)
            {
                Console.WriteLine(group.Key);

                foreach (var course in group)
                    Console.WriteLine("\t{0}", course.Description);
               
            }
            //JOINING
            //Lınk two tables together
            //Extension kullanılıyorsa link yapmaya gerek olmadan navigation property ile tablolar birleştirilebilir.


            /*var result = p.Courses                                       //Outer Table
                   .Join(p.Tags,                                   //Inner Table to join
                         d => new { p1 = d.Id, p2 = d.Title },     //Condition from outer table
                         e => new { p1 = e.Id, p2 = e.Courses },          //Condition from inner table
                         (d, e) => new {                                 //Result
                 Field1 = d.Id,
                             Field2 = d.Title,
                             someField = e.Id
                         }
                         ).ToList();*/

            /*var y =
                from v in p.Courses
                join l in p.Authors on v.Id equals l.Id into g
                select new { Title = v.Title, Id = g };
        foreach(var x in y)
                Console.WriteLine("{0} ({1})",x.Title,x.Id);*/

            //EXTENSIONS
            //We want to get all records that Id is higher than or equal to 3 and order tham by fullprice.
            //projection ile p'den başka bir class oluşturup daha az özellik döndürebiliriz
            var cour=p.Courses.Where(h=>h.FullPrice>=3).OrderByDescending(h=>h.Id).ThenByDescending(h=>h.Level); 
            //projection
           






            //var c = p.Courses.Where<Course>(x => (x.FullPrice >= 6 ) ).Count(); 

            /* for (int j = 0; j < 100; j++)
             {

                 PlutoContext p = new PlutoContext();

                 for (int i = 0; i < 1000; i++)
                 {
                     Course c = new Course();
                     c.Description = "alo";
                     c.FullPrice = 5;
                     p.Courses.Add(c);
                 }
                 p.SaveChanges();
                 Console.WriteLine(j);
             }*/


        }
    }
}
