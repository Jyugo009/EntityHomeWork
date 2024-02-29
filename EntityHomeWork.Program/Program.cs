using EntityHomeWork.DBModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq.Expressions;
using System.Reflection.PortableExecutable;
using System.Xml.Linq;

namespace EntityHomeWork.Program
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello! Do you want to Login or Register?(L/R)");

            var startInput = Console.ReadLine();

            if (startInput != null)
            {
                if (startInput == "L")
                {
                    UserLogin();
                }

                else if (startInput == "R")
                {
                    Console.WriteLine("Are you a Librarian or a Reader? (L/R)");
                    var userType = Console.ReadLine();

                    if (userType == "L")
                    {
                        LibrarianRegistrarion();
                    }

                    else if (userType == "R")
                    {
                        ReaderRegistration();
                    }

                    else
                    {
                        Console.WriteLine("Something went wrong. Try again with using L or R.");
                    }

                }

                else
                {
                    Console.WriteLine("Something went wrong. Try again with using L or R.");
                }
            }
        }


        static void UserLogin()
        {
            Console.WriteLine("Enter Login:");
            var login = Console.ReadLine();

            Console.WriteLine("Enter Password:");
            var password = Console.ReadLine();

            using (var context = new LibraryContext())
            {
                var librarian = context.Librarians.FirstOrDefault(u => u.Login == login);

                // Verify Password
                if (librarian != null)
                {
                    if (!VerifyPassword(password, librarian.PasswordHash, librarian.PasswordSalt))
                        Console.WriteLine("Invalid password.");
                    else
                        LibrarianOptions(librarian);

                    return;
                }

                var reader = context.Readers.FirstOrDefault(u => u.Login == login);

                if (reader != null)
                {
                    if (!VerifyPassword(password, reader.PasswordHash, reader.PasswordSalt))
                        Console.WriteLine("Invalid password.");
                    else
                        ReaderOptions(reader);

                    return;
                }

                Console.WriteLine("User does not exist.");
            }
        }

        private static bool VerifyPassword(string enteredPassword, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(enteredPassword));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i])
                        return false;
                }
            }

            return true;
        }

        private static void ReaderOptions(Reader reader)
        {
            Console.WriteLine($"Welcome, {reader.FirstName} {reader.LastName}!");

            while (true)
            {
                Console.WriteLine("Please select an option:");
                Console.WriteLine("1. Books avialible for take.");
                Console.WriteLine("2. View author info.");
                Console.WriteLine("3. View taken books.");
                Console.WriteLine("4. Take new book.");
                Console.WriteLine("5.Logout.");

                int option = 0;

                try
                {
                    option = Int32.Parse(Console.ReadLine());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Something went wrong. Try again!");
                    return;
                }

                switch (option)
                {
                    case 1:
                        SearchBook();
                        break;
                    case 2:
                        AuthorInfo();
                        break;
                    case 3:
                        TakenBooksByReader(reader);
                        break;
                    case 4:
                        TakeNewBook(reader);
                        break;
                    case 5:
                        Console.WriteLine("Logging out...");
                        return;
                    default:
                        Console.WriteLine("Invalid input. Try again, enter 1,2,3,4 or 5.");
                        break;
                }
            }

        }

        private static void LibrarianOptions(Librarian librarian)
        {
            Console.WriteLine($"Welcome, librarian - {librarian.Login}!");

            while (true)
            {
                Console.WriteLine("Please select an option:");
                Console.WriteLine("1. Books in Library.");
                Console.WriteLine("2. View author info.");
                Console.WriteLine("3. Add and Update Books.");
                Console.WriteLine("4. Add new Authors.");
                Console.WriteLine("5. Operations by Reades.");
                Console.WriteLine("6. Information about Books taken by readers.");
                Console.WriteLine("7.Logout.");

                int option = 0;

                try
                {
                    option = Int32.Parse(Console.ReadLine());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Something went wrong. Try again!");
                    return;
                }

                switch (option)
                {
                    case 1:
                        SearchBook();
                        break;
                    case 2:
                        AuthorInfo();
                        break;
                    case 3:
                        AddOrUpdateBook();
                        break;
                    case 4:
                        AddOrUpdateAuthor();
                        break;
                    case 5:
                        OperationsByReaders();
                        break;
                    case 6:
                        ReviewReadersHistoryAndInfo();
                        break;
                    case 7:
                        Console.WriteLine("Logging out...");
                        return;
                    default:
                        Console.WriteLine("Invalid input. Try again, enter only numbers in list.");
                        break;
                }
            }
        }

        private static void ReviewReadersHistoryAndInfo()
        {
            using (var context = new LibraryContext())
            {
                var today = DateTime.Now;

                var allReaders = context.Readers.Include(r => r.BookOnHands).ThenInclude(b => b.Book).ToList();

                Console.WriteLine("Debtors and what they owe:");
                foreach (var reader in allReaders)
                {
                    if (reader.BookOnHands != null && reader.BookOnHands.Any())
                    {
                        var overdueBooks = reader.BookOnHands.Where(b => b.DueDate.HasValue && b.DueDate.Value < today).ToList();
                        if (overdueBooks.Count > 0)
                        {
                            Console.WriteLine($"Reader {reader.FirstName} {reader.LastName} has {overdueBooks.Count} overdue books:");
                            foreach (var book in overdueBooks.Where(b => b.Book != null))
                                Console.WriteLine($"\t{book.Book.Title}, Due Date: {book.DueDate}");
                        }

                        Console.WriteLine("\nAll who borrowed books and which books:");
                    }
                }

                foreach (var reader in allReaders)
                {
                    if (reader.BookOnHands != null && reader.BookOnHands.Any())
                    {
                        Console.WriteLine($"{reader.Login} - Reader {reader.FirstName} {reader.LastName} has borrowed the following books:");
                        foreach (var bookOnHand in reader.BookOnHands)
                        {
                            Console.WriteLine($"\t{bookOnHand.Book.Title}, Due Date: {bookOnHand.DueDate}");
                        }
                    }
                }

                Console.Write("Enter Reader's FirstName and LastName for history check: ");
                string searchLogin = Console.ReadLine();

                var selectedReader = allReaders.FirstOrDefault(r => r.Login == searchLogin);

                if (selectedReader != null)
                {
                    var selectedHistory = selectedReader.BookOnHands.OrderBy(b => b.CheckoutDate);
                    if (selectedHistory.Any())
                    {
                        int countOverdue = 0;
                        foreach (var record in selectedHistory)
                        {
                            string statusLine;
                            if (record.ReturnDate.HasValue)
                            {
                                statusLine = $"Returned on: {record.ReturnDate.Value.ToShortDateString()}";
                                if (record.ReturnDate.Value > record.DueDate) countOverdue++;
                            }
                            else statusLine = "Not returned yet";

                            Console.WriteLine($"{record.CheckoutDate?.ToShortDateString() ?? "Unknown Checkout Date"} - '{record.Book?.Title}' - due by {(record.DueDate?.ToShortDateString() ?? "unknown date")} - {statusLine}");
                        }

                        Console.WriteLine($"Total number of overdue returns by this Reader is :{countOverdue}");
                    }
                    else
                        Console.WriteLine("No checkout history found for this user.");


                }
                else Console.WriteLine("Invalid Login!");
            }
        }

        private static void OperationsByReaders()
        {
            using (var context = new LibraryContext())
            {
                Console.WriteLine("1. Register new Reader");
                Console.WriteLine("2. Update existing Reader");
                Console.WriteLine("3. Delete Reader");
                Console.Write("Choose an option: ");

                int option = 0;
                try
                {
                    option = Int32.Parse(Console.ReadLine());
                }
                catch(Exception ex) 
                {
                    Console.WriteLine("Something went wrong. Try again!");
                }

                switch (option)
                {
                    case 1:                        
                        ReaderRegistration();
                        break;

                    case 2:
                        var allReaders = context.Readers.ToList();

                        for (int i = 0; i < allReaders.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {allReaders[i].FirstName} {allReaders[i].LastName}");
                        }
                            
                        Console.Write("Select a reader to update: ");

                        int updateIndex = Convert.ToInt32(Console.ReadLine()) - 1;

                        var readerToUpdate = allReaders[updateIndex];

                        Console.Write($"User First Name - {readerToUpdate.FirstName} Enter new First Name: ");
                        string newFirstName = Console.ReadLine();

                        if (!string.IsNullOrWhiteSpace(newFirstName))
                        {
                            readerToUpdate.FirstName = newFirstName;
                        }

                        Console.Write($"User Last Name - {readerToUpdate.LastName} Enter new LastName: ");
                        string newLastName = Console.ReadLine();

                        if (!string.IsNullOrWhiteSpace(newLastName))
                        {
                            readerToUpdate.LastName = newLastName;
                        }                        

                        Console.Write($"User document type - {readerToUpdate.DocumentType.TypeName} " +
                            $"Enter new Document Type: 1 - Passport, 2 - Driving License.");
                        int newDocumentType = default;
                        try
                        {
                            newDocumentType = Int32.Parse(Console.ReadLine());
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine("Something went wrong. Try again.");
                        }

                        if (newDocumentType != 0 || newDocumentType !> 2)
                        {
                            readerToUpdate.DocumentTypeId = newDocumentType;
                        }

                        Console.Write($"User Document Number - {readerToUpdate.DocumentNumber} Enter new Document Number: ");
                        string newDocumentNumber = Console.ReadLine();

                        if (!string.IsNullOrWhiteSpace(newFirstName))
                        {
                            readerToUpdate.DocumentNumber = newDocumentNumber;
                        }

                        context.SaveChanges();
                        break;

                    case 3:
                        allReaders = context.Readers.ToList();

                        for (int i = 0; i < allReaders.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {allReaders[i].FirstName} {allReaders[i].LastName}");
                        }

                        Console.Write("Select a reader to delete: ");

                        int deleteIndex = Convert.ToInt32(Console.ReadLine()) - 1;

                        var readerToDelete = allReaders[deleteIndex];

                        context.Readers.Remove(readerToDelete);

                        context.SaveChanges();
                        break;
                }
            }
        }

        private static void AddOrUpdateAuthor()
        {
            using (var context = new LibraryContext())
            {
                Console.WriteLine("Enter Author's First Name:");
                var firstName = Console.ReadLine();

                Console.WriteLine("Enter Author's Last Name:");
                var lastName = Console.ReadLine();

                var author = context.Authors.FirstOrDefault(a => a.FirstName == firstName && a.LastName == lastName);
                if (author != null)
                {
                    Console.WriteLine($"Name: {author.FirstName}{author.SecondName}{author.LastName}, {author.DateOfBirth}");
                    Console.WriteLine($"Want to change 1.FirstName, 2.SecondName,3.LastName, 4.DateOfBirth? 5.NoChanges.");
                    int userInput = 0;
                    try 
                    {
                        userInput = Int32.Parse(Console.ReadLine());
                    }
                    catch (Exception e) 
                    {
                        Console.WriteLine("Something went wrong. Try again.");
                    }

                    switch(userInput) 
                    {
                        case 1:
                            Console.Write($"Enter new First Name for '{author.FirstName}{author.LastName}': ");
                            string newFirstName = Console.ReadLine();
                            author.FirstName = newFirstName;
                            context.SaveChanges();
                            break;
                        case 2:
                            Console.Write($"Enter new Second Name for '{author.FirstName}{author.LastName}': ");
                            string newSecondName = Console.ReadLine();
                            author.SecondName = newSecondName;
                            context.SaveChanges();
                            Console.WriteLine();
                            break;
                        case 3:
                            Console.Write($"Enter new Last Name for '{author.FirstName}{author.LastName}': ");
                            string newLastName = Console.ReadLine();
                            author.LastName = newLastName;
                            context.SaveChanges();
                            break;
                        case 4:
                            Console.Write($"Enter Date of Birth for '{author.FirstName}{author.LastName}' (format: YYYY-MM-DD): ");
                            DateTime newDateBirth = default;
                            try
                            {
                                newDateBirth = DateTime.Parse(Console.ReadLine());
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Invalid format! Please enter the date in the format: YYYY-MM-DD");
                                break;
                            }
                            author.DateOfBirth = newDateBirth;
                            context.SaveChanges();
                            break;
                        case 5:
                            Console.WriteLine("No changes...");
                            return;
                    }

                }

                else
                {
                    Console.WriteLine("No Authors found. Want to add? Y/N");
                    string userInput = Console.ReadLine();
                    if (userInput == "Y" || userInput != null)
                    {
                        Console.WriteLine("Enter a First Name of Author:");
                        string newFirstName = Console.ReadLine();

                        Console.WriteLine("Enter a Last Name of Author:");
                        string newLastName = Console.ReadLine();

                        Console.WriteLine("Enter Second Name of Author if it's exist.");
                        string newSecondName = Console.ReadLine();

                        Console.WriteLine("Enter Date of Birth of Author (format: YYYY-MM-DD):");
                        DateTime newDateBirth = default;
                        try
                        {
                            newDateBirth = DateTime.Parse(Console.ReadLine());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Invalid format! Please enter the date in the format: YYYY-MM-DD");
                        }

                        try
                        {
                            var newAuthor = new Author
                            {
                                FirstName = newFirstName,
                                LastName = newLastName,
                                SecondName = newSecondName,
                                DateOfBirth = newDateBirth,
                            };

                            context.Authors.Add(newAuthor);

                            Console.WriteLine($"A new book - '{newAuthor.FirstName}{newAuthor.LastName}' added to Library!");
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Something went wrong. Try again!");
                        }
                    }
                    else if (userInput == "N" && userInput != null)
                    {
                        return;
                    }
                    else
                    {
                        Console.WriteLine("No authors found matching your criteria.");
                    }
                }
            }
        }

    

        private static void AddOrUpdateBook()
        {
            using (var context = new LibraryContext())
            {
                Console.WriteLine("Enter title or author:");
                var searchString = Console.ReadLine();

                var books = context.Books.Include(b => b.Authors).ToList();

                var matchingBooks = books.Where(b => b.Title.Contains(searchString) ||
                                                     b.Authors.Any(a => $"{a.FirstName} {a.LastName}".Contains(searchString))).ToList();

                if (matchingBooks.Count > 0)
                {
                    Console.WriteLine("Here are matching books:");

                    for (int i = 0; i < matchingBooks.Count; i++)
                    {
                        Console.WriteLine($"Book number {i+1}");
                        Console.WriteLine($"\n Current Information for {matchingBooks[i].Title}:");
                        Console.WriteLine($"1.Publishing Code: {matchingBooks[i].PublishingCode}");
                        Console.WriteLine($"2.Year: {matchingBooks[i].Year}");
                        Console.WriteLine($"3.Country of Publishing: {matchingBooks[i].CountryOfPublishing}");
                        Console.WriteLine($"4.City of Publishing House: {matchingBooks[i].CityOfPublishHouse}\n");
                    }

                    Console.Write("Choose a number to update book: ");

                    int chosenBookIndex = 0;

                    try
                    {
                        chosenBookIndex = Convert.ToInt32(Console.ReadLine()) - 1;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Something went wrong. Try again! Enter only number of book.");
                    }


                    if (chosenBookIndex < matchingBooks.Count || chosenBookIndex == matchingBooks.Count)
                    {                        
                        Console.WriteLine("What you want to change? Enter the number of column of chosen book.");
                        int userInput = 0;

                        try
                        {
                            userInput = Int32.Parse(Console.ReadLine());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Something went wrong. Try again! Enter only 1,2,3,4 or 5.");
                        }

                        switch (userInput)
                        {
                            case 1:

                                Console.Write($"Enter new publishing code for '{matchingBooks[chosenBookIndex].Title}': ");
                                string newPublishingCode = Console.ReadLine();
                                matchingBooks[chosenBookIndex].PublishingCode = newPublishingCode;
                                context.SaveChanges();
                                break;

                            case 2:

                                Console.Write("Enter new date for Year (format: YYYY-MM-DD): ");
                                DateTime dateTime = default;
                                try
                                {
                                    dateTime = DateTime.Parse(Console.ReadLine());
                                }
                                catch (FormatException)
                                {
                                    Console.WriteLine("Invalid format! Please enter the date in the format: YYYY-MM-DD");
                                }
                                Console.WriteLine($"The date you entered is {dateTime.ToShortDateString()}");
                                context.SaveChanges();
                                break;

                            case 3:

                                Console.Write($"Enter new Country of Publishing for '{matchingBooks[chosenBookIndex].Title}': ");
                                string newCountryOfPublishing = Console.ReadLine();
                                matchingBooks[chosenBookIndex].CountryOfPublishing = newCountryOfPublishing;
                                context.SaveChanges();
                                Console.WriteLine();
                                break;

                            case 4:
                                Console.Write($"Enter new City of Publish House for '{matchingBooks[chosenBookIndex].Title}': ");
                                string newCityPublishHouse = Console.ReadLine();
                                matchingBooks[chosenBookIndex].CityOfPublishHouse = newCityPublishHouse;
                                context.SaveChanges();
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Something went wrong. Try again! Enter the number of column of chosen book!");
                    }
                }
                else
                {
                    Console.WriteLine("Book not found. Create new one. Start with a title:");
                    string newTittle = Console.ReadLine();

                    Console.WriteLine("Enter new Publishing code:");
                    string newPubCode = Console.ReadLine();

                    Console.WriteLine("Publishing code Type: 1) ISBN, 2)LCCN:");
                    int newPubCodeType = 0;
                    try
                    {
                        newPubCodeType = Int32.Parse(Console.ReadLine());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Something went wrong. Be careful...");
                    }

                    Console.WriteLine("Enter Year of publishing (format: YYYY-MM-DD):");
                    DateTime newYear = default;
                    try
                    {
                        newYear = DateTime.Parse(Console.ReadLine());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Invalid format! Please enter the date in the format: YYYY-MM-DD");
                    }

                    Console.WriteLine("Enter a Country of Publishing:");
                    string newCountryOfPub = Console.ReadLine();

                    Console.WriteLine("Enter a City of Publish House:");
                    string newCityOfPub = Console.ReadLine();

                    try
                    {
                        var newBook = new Book
                        {
                            Title = newTittle,
                            PublishingCode = newPubCode,
                            PublishingCodeTypeId = newPubCodeType,
                            Year = newYear,
                            CountryOfPublishing = newCountryOfPub,
                            CityOfPublishHouse = newCityOfPub,
                        };

                        context.Books.Add(newBook);

                        Console.WriteLine($"A new book - '{newBook.Title}' added to Library!");
                    }
                    catch(Exception)
                    {
                        Console.WriteLine("Something went wrong. Try again!");
                    }



                }
            }
            
        }
    

        private static void SearchBook()
        {
            using (var context = new LibraryContext())
            {
                Console.WriteLine("Enter title or author:");
                var searchString = Console.ReadLine();

                var books = context.Books.Include(b => b.Authors).ToList();

                var matchingBooks = books.Where(b => b.Title.Contains(searchString) ||
                                                     b.Authors.Any(a => $"{a.FirstName} {a.LastName}".Contains(searchString))).ToList();
                if (!books.Any())
                {
                    Console.WriteLine("No books found matching your criteria.");
                    return;
                }

                foreach (var book in matchingBooks)
                {
                    Console.WriteLine($"Title: {book.Title}, Authors: {string.Join(", ", book.Authors.Select(a => $"{a.FirstName} {a.LastName}"))}");
                }
            }
        }

        private static void AuthorInfo()
        {
            using (var context = new LibraryContext())
            {
                Console.WriteLine("Enter Author's First Name:");
                var firstName = Console.ReadLine();

                Console.WriteLine("Enter Author's Last Name:");
                var lastName = Console.ReadLine();

                var author = context.Authors.FirstOrDefault(a => a.FirstName == firstName && a.LastName == lastName);

                if (author != null)
                {
                    Console.WriteLine($"Name: {author.FirstName} {author.LastName}, {author.DateOfBirth}");

                    var booksByThisAuthor = context.Books.Include(b => b.PublishingCodeType).Where(b => b.Authors.Any(a => a.AuthorId == author.AuthorId)).ToList();

                    if (!booksByThisAuthor.Any())
                        Console.WriteLine("No books found by this author.");

                    else
                    {
                        Console.WriteLine("Author's books:\n");
                        foreach (var book in booksByThisAuthor)
                        {
                            Console.WriteLine($"Title: {book.Title}, Year: {book.Year?.Year}, Country of Publishing: {book.CountryOfPublishing}, City of Publishing House: {book.CityOfPublishHouse}, Publishing Type: {book.PublishingCodeType?.TypeName}");
                        }
                    }
                }

                else
                {
                    Console.WriteLine("No authors found matching your criteria.");
                }
            }
        }

        private static void TakenBooksByReader(Reader reader)
        {
            using (var context = new LibraryContext())
            {
                var today = DateTime.Now;
              
                var borrowedBooks = context.BookOnHands.Include(b => b.Book).Where(b => b.TakenBy == reader.Login).ToList();

                if (borrowedBooks.Any())
                {
                    var overdueBooks = borrowedBooks.Where(b => b.DueDate.HasValue && b.DueDate.Value < today).OrderBy(b => b.DueDate);

                    var otherBooks = borrowedBooks.Except(overdueBooks).OrderBy(b => b.DueDate);

                    Console.ForegroundColor = ConsoleColor.Red;

                    foreach (var book in overdueBooks)
                        Console.WriteLine($"Overdue: {book.Book.Title}, Due Date: {book.DueDate}");

                    Console.ResetColor();

                    foreach (var book in otherBooks)
                        Console.WriteLine($"{book.Book.Title}, Due Date: {book.DueDate}");
                }

                else
                {
                    Console.WriteLine($"List a books for {reader.FirstName} is empty.");
                }
            }
        }

        private static void TakeNewBook(Reader reader) 
        {
            using (var context = new LibraryContext())
            {
                Console.WriteLine("Enter title or author:");
                var searchString = Console.ReadLine();

                var books = context.Books.Include(b => b.Authors).ToList();
                var matchingBooks = books.Where(b => b.Title.Contains(searchString) ||
                    b.Authors.Any(a => $"{a.FirstName} {a.LastName}".Contains(searchString))).ToList();

                if (matchingBooks.Count > 0)
                {
                    foreach (var book in matchingBooks)
                    {
                        var authorsNames = book.Authors.Select(a => $"{a.FirstName} {a.LastName}");
                        Console.WriteLine($"Title: {book.Title}, Author(s): {string.Join(", ", authorsNames)}");
                    }
                }
                else
                {
                    Console.WriteLine("Here are no matching books.");
                }

                if (matchingBooks.Count > 0)
                {
                    Console.WriteLine("Here are matching books:");

                    for (int i = 0; i < matchingBooks.Count; i++)
                    {
                        Console.WriteLine($"{matchingBooks[i].BookId}. {matchingBooks[i].Title} by {string.Join(", ", matchingBooks[i].Authors.Select(a => $"{a.FirstName} {a.LastName}"))}");
                    }

                    Console.Write("Choose a number to borrow book: ");
                    int chosenBookIndex = Convert.ToInt32(Console.ReadLine()) - 1;

                    var borrowedBook = new BookOnHand
                    {
                        TakenBy = reader.Login,
                        Book = books[chosenBookIndex],
                        CheckoutDate = DateTime.Now,
                        DueDate = DateTime.Now.AddDays(30),
                    };

                    context.BookOnHands.Add(borrowedBook);

                    context.SaveChanges();

                    Console.WriteLine($"Book {borrowedBook.Book.Title} has been added to the borrowed books {reader.FirstName} {reader.LastName}.Return the book in time!");
                }
                else
                {
                    Console.WriteLine("Here are no matching books.");
                }
            }
        }

        static void ReaderRegistration()
        {
            Console.WriteLine("Enter Login:");
            var login = Console.ReadLine();

            Console.WriteLine("Enter Password:");
            var password = Console.ReadLine();

            using (var context = new LibraryContext())
            {
                var existingReader = context.Readers.FirstOrDefault(u => u.Login == login);
                Console.WriteLine("Enter your Email:");
                var readerEmail = Console.ReadLine();
                var existingUserEmail = context.Librarians.FirstOrDefault(u => u.Email == readerEmail);
                if (existingUserEmail != null)
                {
                    Console.WriteLine("This email is already in use. Please try again with another one.");
                    return;
                }

                Console.WriteLine("Enter your First Name:");
                var readerFirstName = Console.ReadLine();

                Console.WriteLine("Enter your Last Name:");
                var readerLastName = Console.ReadLine();

                Console.WriteLine("Do you use a passport(1) or driving license(2)? (1/2)");
                int documentTypeId = 0;

                try
                {
                    documentTypeId = Int32.Parse(Console.ReadLine());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Something went wrong. Enter only 1 or 2.");
                    return;
                }

                string documentNumber = null;

                if (documentTypeId == 1)
                {
                    Console.WriteLine("Enter your Passport number:");
                    documentNumber = Console.ReadLine();
                }
                else if (documentTypeId == 2)
                {
                    Console.WriteLine("Enter your Driver License number:");
                    documentNumber = Console.ReadLine();
                }

                else
                {
                    documentNumber = null;
                }

                try
                {
                    CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

                    context.Readers.Add(new Reader()
                    {
                        Login = login,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        Email = readerEmail,
                        FirstName = readerFirstName,
                        LastName = readerLastName,
                        DocumentTypeId = documentTypeId,
                        DocumentNumber = documentNumber
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Something went wrong. Try again, checking for every entry.");
                    return;
                }

                try
                {
                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Something went wrong. Try again, checking for every entry.");
                    return;
                }
                Console.WriteLine($"Reader account created successfully!");
            }
        }
        static void LibrarianRegistrarion()
        {
            Console.WriteLine("Enter Login:");
            var login = Console.ReadLine();

            Console.WriteLine("Enter Password:");
            var password = Console.ReadLine();

            using (var context = new LibraryContext())
            {
                var existingLibrarianLogin = context.Librarians.FirstOrDefault(u => u.Login == login);
                if (existingLibrarianLogin != null)
                {
                    Console.WriteLine("This login is already in use. Please try again with another one.");
                    return;
                }

                Console.WriteLine("Enter your email:");
                var librarianEmail = Console.ReadLine();
                var existingUserEmail = context.Librarians.FirstOrDefault(u => u.Email == librarianEmail);
                if (existingUserEmail != null)
                {
                    Console.WriteLine("This email is already in use. Please try again with another one.");
                    return;
                }

                if (librarianEmail != null && login != null && password != null)
                {
                    CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

                    context.Librarians.Add(new Librarian()
                    {
                        Login = login,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        Email = librarianEmail
                    });

                    Console.WriteLine($"Librarian account created successfully!");
                    context.SaveChanges();
                }
                else
                {
                    Console.WriteLine("Something went wrong. Try again, checking for every entry.");
                }
            }
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}



    






       

