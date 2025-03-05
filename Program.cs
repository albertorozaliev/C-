using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Programm.UserTask;

namespace Programm
{
    public abstract class People
    {
        public string Name { get; set; }
        protected int Password { get; set; }
        public abstract Task ShowMenu();
    }

    public class User : People
    {
        private List<UserTask> tasks = new List<UserTask>();

        public User(string name, int password)
        {
            Name = name;
            Password = password;
        }

        public override async Task ShowMenu()
        {
            await UserMenu();
        }

        private async Task UserMenu()
        {
            while (true)
            {
                Console.WriteLine("Меню пользователя");
                Console.WriteLine("1) Создать задачу");
                Console.WriteLine("2) Редактировать задачу");
                Console.WriteLine("3) Удалить задачу");
                Console.WriteLine("4) Выход");
                int viborUser;
                if (!int.TryParse(Console.ReadLine(), out viborUser))
                {
                    Console.WriteLine("Некорректный ввод!");
                    continue;
                }

                switch (viborUser)
                {
                    case 1:
                        await CreateTask();
                        break;
                    case 2:
                        await EditTask();
                        break;
                    case 3:
                        await RemoveTask();
                        break;
                    case 4:
                        return;
                    default:
                        Console.WriteLine("Некорректный выбор.");
                        break;
                }
            }
        }

        private async Task CreateTask()
        {
            Console.Write("Название задачи: ");
            string title = Console.ReadLine();
            Console.Write("Описание: ");
            string overview = Console.ReadLine();
            Data.AddTask(new UserTask(title, overview, UserTask.TaskPriority.Average, UserTask.TaskStatus.Completed));
            Console.WriteLine("Задача добавлена.");
        }

        private async Task RemoveTask()
        {
            Console.Write("Введите название задачи: ");
            string title = Console.ReadLine();

            UserTask task = null;
            foreach (var t in Data.GetAllTasks())
            {
                if (t.Title == title)
                {
                    task = t;
                    break; 
                }
            }


            if (task != null)
            {
                Data.RemoveTask(title);
                Console.WriteLine("Задача удалена.");
            }
            else
            {
                Console.WriteLine("Задача не найдена.");
            }
        }


        private async Task EditTask()
        {
            Console.Write("Введите название задачи: ");
            string title = Console.ReadLine();
            UserTask task = null;
            foreach (var t in Data.GetAllTasks())
            {
                if (t.Title == title)
                {
                    task = t;
                    break; 
                }
            }


            if (task != null)
            {
                Console.WriteLine("Что изменить?\n1) Описание\n2) Статус\n3) Приоритет");
                int choice;
                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Некорректный ввод!");
                    return;
                }

                switch (choice)
                {
                    case 1:
                        Console.Write("Введите новое описание: ");
                        task.Overview = Console.ReadLine();
                        Console.WriteLine("Описание обновлено.");
                        break;
                    case 2:
                        Console.WriteLine("Выберите статус:\n1) В работе\n2) Завершено\n3) В процессе");
                        int statusChoice;
                        if (int.TryParse(Console.ReadLine(), out statusChoice))
                        {
                            switch (statusChoice)
                            {
                                case 1: 
                                    task.SetStatus(UserTask.TaskStatus.Busy); 
                                    break;
                                case 2: 
                                    task.SetStatus(UserTask.TaskStatus.Completed); 
                                    break;
                                case 3: 
                                    task.SetStatus(UserTask.TaskStatus.InProgress);
                                    break;
                                default: 
                                    Console.WriteLine("Некорректный выбор.");
                                    break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Некорректный ввод.");
                        }
                        break;
                    case 3:
                        Console.WriteLine("Выберите приоритет:\n1) Низкий\n2) Средний\n3) Высокий");
                        int priorityChoice;
                        if (int.TryParse(Console.ReadLine(), out priorityChoice))
                        {
                            switch (priorityChoice)
                            {
                                case 1:
                                    task.SetPriority(UserTask.TaskPriority.Low); 
                                    break;
                                case 2:
                                    task.SetPriority(UserTask.TaskPriority.Average); 
                                    break;
                                case 3:
                                    task.SetPriority(UserTask.TaskPriority.High); 
                                    break;
                                default: 
                                    Console.WriteLine("Некорректный выбор."); 
                                    break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Некорректный ввод.");
                        }
                        break;
                    default:
                        Console.WriteLine("Некорректный выбор.");
                        break;
                }
                Data.SaveTasks();
                Console.WriteLine("Задача обновлена.");
            }
            else
            {
                Console.WriteLine("Задача не найдена.");
            }
        }


    }

    public class UserAc
    {
        public string UserName { get; set; }
        public int Password { get; set; }
        public UserAc(string userName, int password)
        {
            UserName = userName;
            Password = password;
        }
    }

    public class UserTask
    {
        public string Title { get; set; }
        public string Overview { get; set; }
        public TaskPriority Priority { get; set; }
        public TaskStatus Status { get; set; }

        public enum TaskPriority { Low, High, Average }
        public enum TaskStatus { Busy, Completed, InProgress }

        public UserTask(string title, string overview, TaskPriority priority, TaskStatus status)
        {
            Title = title;
            Overview = overview;
            Priority = priority;
            Status = status;
        }

        public void SetStatus(TaskStatus newStatus)
        {
            Status = newStatus;
        }

        public void SetPriority(TaskPriority newPriority)
        {
            Priority = newPriority;
        }

    }

    public static class Data
    {
        private static List<UserTask> tasks = new List<UserTask>();
        private static List<UserAc> users = new List<UserAc>();

        public static async Task AddTask(UserTask task)
        {   
            tasks.Add(task);
            await SaveTasks();
        }

        public static async Task RemoveTask(string title)
        {
            var task = tasks.FirstOrDefault(t => t.Title == title);
            if (task != null)
            {
                tasks.Remove(task);
                await SaveTasks();
            }
        }

        public static async Task SaveTasks()
        {
            var lines = tasks.Select(b => $"{b.Title};{b.Overview};{b.Status};{b.Priority}");
            File.WriteAllLines("tasks.txt", lines);
        }

        public static async Task LoadTasks()
        {
            tasks.Clear();
            if (File.Exists("tasks.txt"))
            {
                var lines = await File.ReadAllLinesAsync("tasks.txt");
                foreach (var line in lines)
                {
                    var parts = line.Split(';');
                    if (parts.Length == 4 &&
                        Enum.TryParse(parts[2], out UserTask.TaskStatus status) &&
                        Enum.TryParse(parts[3], out UserTask.TaskPriority priority))
                    {
                        tasks.Add(new UserTask(parts[0], parts[1], priority, status));
                    }
                }
            }
        }

        public static void SaveUsers()
        {
            var lines = users.Select(u => $"{u.UserName};{u.Password}");
            File.WriteAllLines("users.txt", lines);
        }

        public static List<UserTask> GetAllTasks()
        {
            return tasks; 
        }

        public static UserAc FindUser(string name, int password)
        {
            return users.FirstOrDefault(u => u.UserName == name && u.Password == password);
        }

        public static void AddUser(UserAc user)
        {
            users.Add(user);
            SaveUsers();
        }


        public static async Task LoadUsers()
        {
            users.Clear();
            if (File.Exists("users.txt"))
            {
                foreach (var line in File.ReadAllLines("users.txt"))
                {
                    var parts = line.Split(';');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int password))
                    {
                        users.Add(new UserAc(parts[0], password));
                    }
                }
            }
        }


    }

    class Program
    {
        static async Task Main()
        {
            await Data.LoadTasks();
            await Data.LoadUsers();
            Console.WriteLine("Добро пожаловать");
            while (true)
            {
                Console.WriteLine("Выберите действие:\n1) Вход\n2) Регистрация\n3) Выход");
                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Некорректный ввод!");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        Login();
                        break;
                    case 2:
                        Register();
                        break;
                    case 3:
                        return;
                    default:
                        Console.WriteLine("Некорректный выбор.");
                        break;
                }
            }
        }

        static async Task Login()
        {
            Console.Write("Введите имя пользователя: ");
            string username = Console.ReadLine();
            Console.Write("Введите пароль: ");
            if (!int.TryParse(Console.ReadLine(), out int password))
            {
                Console.WriteLine("Некорректный ввод пароля.");
                return;
            }

            var user = Data.FindUser(username, password);
            if (user != null)
            {
                Console.WriteLine($"Вход выполнен");
                People person = new User(username, password);
                await person.ShowMenu();
            }
            else
            {
                Console.WriteLine("Неверные имя пользователя или пароль.");
            }
        }

        static async Task Register()
        {
            Console.Write("Введите имя: ");
            string username = Console.ReadLine();
            Console.Write("Введите пароль: ");
            if (!int.TryParse(Console.ReadLine(), out int password))
            {
                Console.WriteLine("Некорректный ввод пароля.");
                return;
            }

            if (Data.FindUser(username, password) != null)
            {
                Console.WriteLine("Пользователь с таким именем существует.");
                return;
            }

            Data.AddUser(new UserAc(username, password));
            Console.WriteLine("Регистрация успешна");
        }
    }
    
}
