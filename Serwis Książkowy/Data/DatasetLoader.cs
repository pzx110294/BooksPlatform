using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Serwis_Książkowy.Models;

namespace Serwis_Książkowy.Data;

public static class DatasetLoader
{
    private const string filePath = "Dataset/books.csv";
    private static ApplicationDbContext _dbContext;
    
    public static void LoadDataset(ApplicationDbContext dbContext)
    {
        if (dbContext.IsAlreadyLoaded()) return;
        
        _dbContext = dbContext;
        CsvConfig csvConfig = ConfigureCsvLoader();
        var records = ReadCsvFile(csvConfig);
        AddRecordsToDb(records);
    }


    private static bool IsAlreadyLoaded(this ApplicationDbContext dbContext)
    {
        return dbContext.Books.Any();
    }

    private static CsvConfig ConfigureCsvLoader()
    {
        var csvConfig = new CsvConfig(new StreamReader(filePath), new CsvConfiguration(CultureInfo.InvariantCulture));
        return csvConfig;
    }
    private static IEnumerable<BookCsvModel> ReadCsvFile(CsvConfig csvConfig)
    {
        using var csv = new CsvReader(csvConfig.Reader, csvConfig.Config);
        return csv.GetRecords<BookCsvModel>().ToList();
    }

    private static void AddRecordsToDb(IEnumerable<BookCsvModel> records)
    {
        foreach (BookCsvModel record in records)
        {
            Author author = AddAuthor(record.Authors);
            AddBook(record, author);
            
            _dbContext.SaveChanges();
        }
    }

    private static Author AddAuthor(string authorName)
    {
        authorName = authorName.Trim();
        Author author;
        Author? existingAuthor = CheckIfAuthorExists(authorName);

        if (existingAuthor != null)
        {
            return existingAuthor;
        }
        author = new Author { Name = authorName };
        _dbContext.Authors.Add(author);
        return author;
    }
    
    
    private static Author? CheckIfAuthorExists(string name)
    {
        return _dbContext.Authors.SingleOrDefault(a => a.Name == name);
    }
    private static void AddBook(BookCsvModel record, Author author)
    {
        var bookTitle = record.Title.Trim();
        if (CheckIfBookExists(bookTitle)) return;
        List<Genre> genres = _dbContext.Genres.ToList();
        Random random = new();
        Genre randomGenre = genres[random.Next(genres.Count)];
        _dbContext.Books.Add(new Book
        {
            Title = bookTitle,
            Author = author,
            PublicationDate = record.PublicationDate,
            Isbn = record.ISBN13,
            Rating = record.AverageRating,
            Genre = randomGenre
        });
    }

    private static bool CheckIfBookExists(string title)
    {
        return _dbContext.Books.Count(b => b.Title == title) > 0;
    }
}

class CsvConfig
{
    public StreamReader Reader { get; private set; }
    public CsvConfiguration Config { get; private set; }

    public CsvConfig(StreamReader reader, CsvConfiguration config)
    {
        config.Mode = CsvMode.NoEscape;
        Reader = reader;
        Config = config;
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

