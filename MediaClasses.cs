using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Globalization; // Додано для CultureInfo


// Абстрактний базовий клас для всіх медіа матеріалів (аудіо та відео)
public abstract class Media
{
    // Унікальний код матеріалу (використовується як первинний ключ у базі даних)
    [Key]
    public string Code { get; set; }

    // Назва матеріалу (наприклад, назва пісні чи фільму)
    public string Title { get; set; }

    // Формат запису (наприклад, MP3 для аудіо, MP4 для відео)
    public string Format { get; set; }

    // Рік випуску матеріалу
    public int Year { get; set; }

    // Вартість матеріалу
    public double Price { get; set; }

    // Конструктор для ініціалізації спільних властивостей медіа
    public Media(string code, string title, string format, int year, double price)
    {
        Code = code;
        Title = title;
        Format = format;
        Year = year;
        Price = price;
    }

    // Віртуальний метод для обчислення ціни зі знижкою (за замовчуванням повертає звичайну ціну)
    public virtual double CalculateDiscountedPrice()
    {
        return Price;
    }

    // Перевизначений метод ToString для виведення інформації про матеріал
    public override string ToString()
    {
        return $"Код: {Code}, Назва: {Title}, Формат: {Format}, Рік: {Year}, Ціна: {Price.ToString("N2", CultureInfo.GetCultureInfo("uk-UA"))}";
    }
}

// Похідний клас для аудіо матеріалів, успадковує від Media
public class Audio : Media
{
    // Автор аудіо запису (наприклад, композитор)
    public string Author { get; set; }

    // Виконавець аудіо запису (наприклад, співак чи група)
    public string Performer { get; set; }

    // Тривалість аудіо запису в секундах
    public int Duration { get; set; }

    // Конструктор для ініціалізації аудіо запису
    public Audio(string code, string title, string author, string performer, int duration, string format, int year, double price)
        : base(code, title, format, year, price)
    {
        Author = author;
        Performer = performer;
        Duration = duration;
    }

    // Перевизначений метод для обчислення ціни зі знижкою (10% знижка для аудіо)
    public override double CalculateDiscountedPrice()
    {
        return Price * 0.9;
    }

    // Перевизначений метод ToString для виведення специфічної інформації про аудіо
    public override string ToString()
    {
        return base.ToString() + $", Автор: {Author}, Виконавець: {Performer}, Тривалість: {Duration / 60} хв {Duration % 60} сек";
    }
}

// Похідний клас для відео матеріалів, успадковує від Media
public class Video : Media
{
    // Режисер відео
    public string Director { get; set; }

    // Головний актор у відео
    public string MainActor { get; set; }

    // Конструктор для ініціалізації відео
    public Video(string code, string title, string director, string mainActor, string format, int year, double price)
        : base(code, title, format, year, price)
    {
        Director = director;
        MainActor = mainActor;
    }

    // Перевизначений метод для обчислення ціни зі знижкою (15% знижка для відео)
    public override double CalculateDiscountedPrice()
    {
        return Price * 0.85;
    }

    // Перевизначений метод ToString для виведення специфічної інформації про відео
    public override string ToString()
    {
        return base.ToString() + $", Режисер: {Director}, Головний актор: {MainActor}";
    }
}