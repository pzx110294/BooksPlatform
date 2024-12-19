using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Serwis_Książkowy.Models;

namespace Serwis_Książkowy.Data;

public static class DatasetLoader
{
    private static string filePath = "Dataset/books.csv";
    public static void LoadDataset(ApplicationDbContext dbContext)
    {
        if (dbContext.Books.Any()) return;
        
        using var reader = new StreamReader(filePath);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture);
        config.Mode = CsvMode.NoEscape;
        
        using (var csv = new CsvReader(reader, config))
        {
            var records = csv.GetRecords<BookCsvModel>();

            foreach (BookCsvModel record in records)
            {
                //Add author if not exists
                var authorName = record.Authors.Trim();
                Author author;
                var existingAuthor = dbContext.Authors.SingleOrDefault(a => a.Name == authorName);
                if (existingAuthor == null)
                {
                    author = new Author { Name = authorName };
                    dbContext.Authors.Add(author);
                }
                else
                {
                    author = existingAuthor;
                }
                
                //Add book
                var book = record.Title.Trim();
                var existingBook = dbContext.Books.Count(b => b.Title == book);
                if (existingBook == 0)
                {
                    dbContext.Books.Add(new Book
                    {
                        Title = book,
                        Author = author,
                        PublicationDate = record.PublicationDate,
                        Isbn = record.ISBN13,
                        Rating = record.AverageRating
                    });
                }

                dbContext.SaveChanges();
            }
        }
    }
}

public class BookCsvModel
{
    [Name("bookID")]
    public int BookID { get; set; }
    
    [Name("title")]
    public string Title { get; set; }
    
    [Name("authors")]
    public string Authors { get; set; }
    
    [Name("average_rating")]
    public float AverageRating { get; set; }
    
    [Name("isbn")]
    public string ISBN { get; set; }
    
    [Name("isbn13")]
    public string ISBN13 { get; set; }
    
    [Name("language_code")]
    public string LanguageCode { get; set; }
    
    [Name("num_pages")]
    public int NumPages { get; set; }
    
    [Name("ratings_count")]
    public int RatingsCount { get; set; }
    
    [Name("text_reviews_count")]
    public int TextReviewsCount { get; set; }
    
    [Name("publication_date")]
    public DateTime PublicationDate { get; set; }
    
    [Name("publisher")]
    public string Publisher { get; set; }
    
}

