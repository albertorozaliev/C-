using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

namespace Programm
{
    public abstract class People
    {
        public string Name { get; set; }
        public abstract void ShowMenu();
    }

    public class User : People
    {
        public override void ShowMenu() => UserMenu();

        public User(string name)
        {
            Name = name;
        }

        private void UserMenu()
        {
            while (true)
            {
                Console.WriteLine($"\nМеню пользователя {Name}:");
                Console.WriteLine("1. Просмотреть доступные книги");
                Console.WriteLine("2. Взять книгу");
                Console.WriteLine("3. Вернуть книгу");
                Console.WriteLine("4. Просмотреть мои книги");
                Console.WriteLine("5. Выход");

                //if (!int.TryParse(Console.ReadLine(), out int choice))
                //{
                //    Console.WriteLine("Ошибка ввода, попробуйте снова.");
                //    continue;
                //}

                //switch (choice)
                //{
                //    case 1:
                //        DataManager.ShowAvailableBooks();
                //        break;
                //    case 2:
                //        BorrowBook();
                //        break;
                //    case 3:
                //        ReturnBook();
                //        break;
                //    case 4:
                //        ShowMyBooks();
                //        break;
                //    case 5:
                //        return;
                //    default:
                //        Console.WriteLine("Некорректный выбор.");
                //        break;
                //}
            }
        }

        private readonly List<Book> _borrowedBooks = new List<Book>();
        public IReadOnlyList<Book> BorrowedBooks => _borrowedBooks.AsReadOnly();

        public void BorrowBook(Book book)
        {
            if (book.Status == Book.BookStatus.Available)
            {
                _borrowedBooks.Add(book);
                book.MarkAsBorrowed();
            }
        }

        public void ReturnBook(Book book)
        {
            if (_borrowedBooks.Contains(book))
            {
                _borrowedBooks.Remove(book);
                book.MarkAsAvailable();
            }
        }
    }

    public class Admin : People
    {
        public override void ShowMenu() => AdminMenu();

        public Admin(string name)
        {
            Name = name;
        }

        private void AdminMenu()
        {
            var books = DataManager.LoadBooks();

            while (true)
            {
                Console.WriteLine("\nМеню администратора:");
                Console.WriteLine("1. Добавить книгу");
                Console.WriteLine("2. Удалить книгу");
                Console.WriteLine("3. Выход");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Ошибка ввода, попробуйте снова.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        Console.Write("Название книги: ");
                        string title = Console.ReadLine();
                        Console.Write("Автор: ");
                        string author = Console.ReadLine();
                        books.Add(new Book(title, author, Book.BookStatus.Available));
                        break;
                    case 2:
                        Console.Write("Введите номер книги для удаления: ");
                        if (!int.TryParse(Console.ReadLine(), out int index) || index <= 0 || index > books.Count)
                        {
                            Console.WriteLine("Некорректный номер книги.");
                            continue;
                        }
                        books.RemoveAt(index - 1);
                        break;
                    case 3:
                        DataManager.SaveBooks(books);
                        return;
                    default:
                        Console.WriteLine("Некорректный выбор.");
                        break;
                }
            }
        }
    }

    public class Book
    {
        public enum BookStatus { Available, Borrowed }
        public string Name { get; set; }
        public string Author { get; set; }
        public BookStatus Status { get; private set; }

        public Book(string name, string author, BookStatus status)
        {
            Name = name;
            Author = author;
            Status = status;
        }

        public void MarkAsBorrowed() => Status = BookStatus.Borrowed;
        public void MarkAsAvailable() => Status = BookStatus.Available;
    }

    public static class DataManager
    {
        private static readonly string booksFile = "books.txt";
        private static readonly string usersFile = "users.txt";

        public static List<Book> LoadBooks()
        {
            var books = new List<Book>();
            if (File.Exists(booksFile))
            {
                foreach (var line in File.ReadAllLines(booksFile))
                {
                    var parts = line.Split(';');
                    if (parts.Length < 3) continue;
                    if (Enum.TryParse(parts[2], out Book.BookStatus status))
                    {
                        books.Add(new Book(parts[0], parts[1], status));
                    }
                }
            }
            return books;
        }

        public static List<User> LoadUsers()
        {
            return new List<User>(); // Заглушка
        }

        public static void SaveBooks(List<Book> books)
        {
            var lines = books.Select(b => $"{b.Name};{b.Author};{b.Status}");
            File.WriteAllLines(booksFile, lines);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            People person = null;

            Console.WriteLine("Добро пожаловать");
            Console.WriteLine("Выберите номер для входа: 1) админ 2) Пользователь");
            int vubor = Convert.ToInt32(Console.ReadLine());

            if (vubor == 1)
            {
                person = new Admin("Администратор");
            }
            else if (vubor == 2)
            {
                person = new User("Гость");
            }
            else
            {
                Console.WriteLine("Неверный выбор");
                return;
            }

            person.ShowMenu();
        }
    }
}

//    class Programm
//    {
//        static void Main(string[] args)
//        {
//            People person = null; // Базовый класс

//            Console.WriteLine("Добро пожаловать");
//            Console.WriteLine("Выберите номер для входа: 1) админ 2) Пользователь");
//            int vubor = Convert.ToInt32(Console.ReadLine());

//            if (vubor == 1)
//            {
//                person = new Admin("Администратор");
//            }
//            else if (vubor == 2)
//            {
//                person = new User("Гость");
//            }
//            else
//            {
//                Console.WriteLine("Неверный выбор");
//                return;
//            }

//            person.ShowMenu(); // Запуск меню выбранной роли
//        }
//    }
//}


