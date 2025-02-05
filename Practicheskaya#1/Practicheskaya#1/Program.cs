
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
                Console.WriteLine("Меню пользователя");
                Console.WriteLine("1)Просмотреть книги");
                Console.WriteLine("2)Взять книгу");
                Console.WriteLine("3)Вернуть книгу");
                Console.WriteLine("4)Просмотреть мои книги");
                Console.WriteLine("5)Выход");
                int viborUser = Convert.ToInt32(Console.ReadLine());
                switch (viborUser)
                {
                    case 1:
                        ShowBooks();
                        break;
                    case 2:
                        GetBook();
                        break;
                    case 3:
                        ReturnBook();
                        break;
                    case 4:
                        ShowMyBooks();
                        break;
                    case 5:
                        return;
                    default:
                        Console.WriteLine("Некорректный выбор.");
                        break;
                }
            }
        }

        private readonly List<Book> Books = new List<Book>();
        public IReadOnlyList<Book> ReadBooks => Books.AsReadOnly();

        private void GetBook()
        {
            Console.WriteLine("Введите название книги");
            string title = Console.ReadLine();
            Book book = Data.FindBook(title);
            if (book.Status == Book.BookStatus.open)
            {
                Books.Add(book);
                book.close();
                Data.SaveBooks();
                Console.WriteLine("Книга успешно взята.");
            }
            else
            {
                Console.WriteLine("Нет");
            }
        }
        private void ShowMyBooks()
        {
            Console.WriteLine("Ваши книги:");
            foreach (var book in Books)
                Console.WriteLine(book.Name);
        }
        private void ReturnBook()
        {
            Console.Write("Введите название книги: ");
            string title = Console.ReadLine();
            Book book = Books.FirstOrDefault(b => b.Name == title);
            if (book != null)
            {
                Books.Remove(book);
                book.open();
                Data.SaveBooks();
                Console.WriteLine("Книга возвращена");
            }
            else
            {
                Console.WriteLine("Книга не найдена");
            }
        }

        private void ShowBooks()
        {
            Console.WriteLine("Доступные книги:");
            foreach (var book in Data.open())
            {
                Console.WriteLine($"{book.Name} - {book.Author}");
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
            while (true)
            {
                Console.WriteLine("Меню администратора:");
                Console.WriteLine("1)Добавить книгу");
                Console.WriteLine("2)Удалить книгу");
                Console.WriteLine("3)Добавить пользователя");
                Console.WriteLine("4)Просмотреть всех пользователей");
                Console.WriteLine("5)Просмотреть все книги");
                Console.WriteLine("6)Выход");
                int viborAdmina = Convert.ToInt32(Console.ReadLine());
                switch (viborAdmina)
                {
                    case 1:
                        Console.Write("Название книги: ");
                        string title = Console.ReadLine();
                        Console.Write("Автор: ");
                        string author = Console.ReadLine();
                        Data.AddBook(new Book(title, author, Book.BookStatus.open));
                        break;
                    case 2:
                        Console.Write("Введите название книги для удаления: ");
                        string bookTitle = Console.ReadLine();
                        Data.RemoveBook(bookTitle);
                        break;
                    case 3:
                        AddNewUser();
                        break;
                    case 4:
                        ShowAllUsers();
                        break;
                    case 5:
                        ShowAllBooks();
                        break;
                    case 6:
                        Data.SaveBooks();
                        return;
                    default:
                        Console.WriteLine("Некорректный выбор.");
                        break;
                }
            }
        }
        private void ShowAllBooks()
        {
            Console.WriteLine("Список всех книг:");
            foreach (var book in Data.GetAllBooks())
            {
                Console.WriteLine($"{book.Name} - {book.Author} Статус:{book.Status}");
            }
        }
        private void ShowAllUsers()
        {
            Console.WriteLine("Список пользователей:");
            foreach (var user in Data.GetUsers())
            {
                Console.WriteLine(user.UserName);
            }
        }
        private void AddNewUser()
        {
            Console.Write("Введите имя нового пользователя: ");
            string userName = Console.ReadLine();
            var newUser = new UserAccount(userName);
            Data.AddUser(newUser);
            Data.SaveUsers();
            Console.WriteLine($"Пользователь {userName} добавлен.");
        }

    }

    public class UserAccount
    {
        public string UserName { get; set; }

        public UserAccount(string userName)
        {
            UserName = userName;
        }
    }

    public class Book
    {
        public enum BookStatus { busy, open }
        public string Name { get; set; }
        public string Author { get; set; }
        public BookStatus Status { get; private set; }

        public Book(string name, string author, BookStatus status)
        {
            Name = name;
            Author = author;
            Status = status;
        }

        public void close() => Status = BookStatus.busy;
        public void open() => Status = BookStatus.open;
    }

    public static class Data
    {
        private static List<Book> books = new List<Book>();
        private static List<UserAccount> users = new List<UserAccount>();

        public static List<Book> open() => books.Where(b => b.Status == Book.BookStatus.open).ToList();

        public static Book FindBook(string title) => books.FirstOrDefault(b => b.Name.Equals(title, StringComparison.OrdinalIgnoreCase));

        public static void AddBook(Book book)
        {
            books.Add(book);
            SaveBooks();
        }

        public static void RemoveBook(string title)
        {
            var book = FindBook(title);
            if (book != null)
            {
                books.Remove(book);
                SaveBooks();
            }
        }

        public static void SaveBooks()
        {
            var lines = books.Select(b => $"{b.Name};{b.Author};{b.Status}");
            File.WriteAllLines("books.txt", lines);
        }

        public static void LoadBooks()
        {
            books.Clear();
            if (File.Exists("books.txt"))
            {
                foreach (var line in File.ReadAllLines("books.txt"))
                {
                    var parts = line.Split(';');
                    if (parts.Length == 3 && Enum.TryParse(parts[2], out Book.BookStatus status))
                    {
                        books.Add(new Book(parts[0], parts[1], status));
                    }
                }
            }
        }
        public static void SaveUsers()
        {
            var lines = users.Select(u => u.UserName);
            File.WriteAllLines("users.txt", lines);
        }
        public static void LoadUsers()
        {
            users.Clear();
            if (File.Exists("users.txt"))
            {
                foreach (var line in File.ReadAllLines("users.txt"))
                {
                    users.Add(new UserAccount(line));
                }
            }
        }
        public static List<UserAccount> GetUsers()
        {
            return users;
        }
        public static List<Book> GetAllBooks()
        {
            return books;
        }
        public static void AddUser(UserAccount user)
        {
            users.Add(user);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Data.LoadBooks();
            Data.LoadUsers();
            Console.WriteLine("Добро пожаловать");
            Console.WriteLine("Выберите номер для входа: 1) админ 2) Пользователь");
            int vubor = Convert.ToInt32(Console.ReadLine());

            if (vubor == 1)
            {
                People person= new Admin("Администратор");
                person.ShowMenu();
            }
            else if (vubor == 2)
            {
                People person = new User("Гость");
                person.ShowMenu();
            }
            else
            {
                Console.WriteLine("Неверный выбор");
                return;
            }

            
        }
    }
}




