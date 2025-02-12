using System;

class Rectangle : IDisposable
{
    public int Height;
    public int Width;
    public int S;
    public int P;
    public Rectangle()
    {
        Height = 4;
        Width = 1;
        S=Height*Width;
        P =(Width+Height)*2;
        Console.WriteLine($"Площадь:{S}\n "+$"Периметр: {P}\n"+"Конструктор по умолчанию вызван");
        
    }
    public Rectangle(int Height, int Width)
    {
        this.Height = Height;
        this.Width = Width;
        S = Height * Width;
        P = (Width + Height) * 2;
        Console.WriteLine($"Площадь:{S}\n " + $"Периметр: {P}\n"+"Конструктор с параметрами вызван");
    }
    public Rectangle(Rectangle all)
    {
        Height = all.Height;
        Width = all.Width;
        S = Height * Width;
        P = 2 * (Height + Width);
        Console.WriteLine("Конструктор копирования вызван");
    }
    public void Dispose()
    {
        Console.WriteLine($"Деструктор");
        GC.SuppressFinalize(this); 
    }
    ~Rectangle()
    {
        Console.WriteLine("Деструктор");
    }

}

class Programm
{

    static void Main(string[] args)
    {
        Rectangle a1 = new Rectangle();
        Rectangle a2 = new Rectangle(3, 5);
        Rectangle a3 = new Rectangle(a1);

        using (Rectangle a4 = new Rectangle()) { }
        GC.Collect();
        GC.WaitForPendingFinalizers();

    }
}