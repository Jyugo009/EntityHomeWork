using EntityHomeWork.DBModels;
using Microsoft.EntityFrameworkCore;
using System;

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
                var librarian = context.Librarians.FirstOrDefault(u => u.Login == login && u.Password == password);

                var reader = context.Readers.FirstOrDefault(u => u.Login == login && u.Password == password);

                if (librarian != null)
                {
                    Console.WriteLine($"Welcome librarian: {librarian.Login}!");
                }

                else if (reader != null)
                {
                    Console.WriteLine($"Welcome reader: {reader.Login}!");
                }

                else
                {
                    Console.WriteLine("Wrong Login/Password or User does not exist.");
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
                    context.Readers.Add(new Reader()
                    {
                        Login = login,
                        Password = password,
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
                    context.Librarians.Add(new Librarian() { Login = login, Password = password, Email = librarianEmail });
                    Console.WriteLine($"Librarian account created successfully!");
                    context.SaveChanges();
                }
                else
                {
                    Console.WriteLine("Something went wrong. Try again, checking for every entry.");
                }
            }
        }
    }
}



    






       

