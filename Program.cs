using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleApp;
using ConsoleApp.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Globalization; // Додано для CultureInfo
using System.Text; // Для кодування


// Основний клас програми для роботи з каталогом медіа через EF Core
class Program
{
    // Колекція для зберігання всіх матеріалів у пам’яті
    private static List<Media> mediaList = new List<Media>();

    // Культура для парсингу та форматування цін (українська локалізація)
    private static readonly CultureInfo UkrainianCulture = CultureInfo.GetCultureInfo("uk-UA");

    // Точка входу в програму
    static void Main(string[] args)
    {
        // Встановлюємо кодування UTF-8 для коректного відображення українських символів
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        // Завантажуємо дані з бази даних при старті програми
        LoadFromDatabase();

        // Основний цикл програми, який працює, поки користувач не вибере вихід
        bool running = true;
        while (running)
        {
            // Виводимо меню з доступними опціями
            Console.WriteLine("\n=== Меню каталогу медіа ===");
            Console.WriteLine("1. Додати аудіо запис");
            Console.WriteLine("2. Додати відео");
            Console.WriteLine("3. Видалити матеріал");
            Console.WriteLine("4. Редагувати матеріал");
            Console.WriteLine("5. Переглянути відео, впорядковані за ціною");
            Console.WriteLine("6. Пошук матеріалів");
            Console.WriteLine("7. Вийти");

            // Отримуємо вибір користувача
            Console.Write("Виберіть опцію (1-7): ");
            string choice = Console.ReadLine();

            // Обробляємо вибір користувача
            switch (choice)
            {
                case "1":
                    AddAudio(); // Додаємо аудіо
                    break;
                case "2":
                    AddVideo(); // Додаємо відео
                    break;
                case "3":
                    RemoveMedia(); // Видаляємо матеріал
                    break;
                case "4":
                    EditMedia(); // Редагуємо матеріал
                    break;
                case "5":
                    ShowVideosByPrice(); // Сортуємо відео за ціною
                    break;
                case "6":
                    SearchMedia(); // Універсальний пошук
                    break;
                case "7":
                    running = false; // Завершуємо програму
                    break;
                default:
                    Console.WriteLine("Невірний вибір! Спробуйте ще раз.");
                    break;
            }
        }

        // Зберігаємо зміни в базі даних перед завершенням
        SaveToDatabase();
    }

    // Метод для завантаження даних із бази даних
    static void LoadFromDatabase()
    {
        try
        {
            using (var context = new MediaContext())
            {
                // Ініціалізуємо базу даних і створюємо таблицю Media, якщо вона не існує
                context.Database.EnsureCreated();

                // Очищаємо список перед завантаженням
                mediaList.Clear();

                // Завантажуємо всі медіа з бази даних
                mediaList.AddRange(context.Media.ToList());
                Console.WriteLine($"Дані успішно завантажено з бази даних. Аудіо: {mediaList.Count(m => m is Audio)}, Відео: {mediaList.Count(m => m is Video)}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка при завантаженні даних: {ex.Message}");
        }
    }

    // Метод для збереження даних у базу даних
    static void SaveToDatabase()
    {
        try
        {
            using (var context = new MediaContext())
            {
                // Видаляємо всі існуючі записи, щоб уникнути дублювання
                context.Media.RemoveRange(context.Media);

                // Додаємо всі записи
                context.Media.AddRange(mediaList);

                // Зберігаємо зміни в базі даних
                context.SaveChanges();
                Console.WriteLine("Дані успішно збережено в базу даних.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка при збереженні даних: {ex.Message}");
        }
    }

    // Метод для додавання нового аудіо запису
    static void AddAudio()
    {
        try
        {
            Console.WriteLine("\n=== Додавання аудіо запису ===");
            Console.Write("Введіть код: ");
            string code = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(code))
            {
                Console.WriteLine("Помилка: Код не може бути порожнім!");
                return;
            }

            Console.Write("Введіть назву: ");
            string title = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Помилка: Назва не може бути порожньою!");
                return;
            }

            Console.Write("Введіть автора: ");
            string author = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(author))
            {
                Console.WriteLine("Помилка: Автор не може бути порожнім!");
                return;
            }

            Console.Write("Введіть виконавця: ");
            string performer = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(performer))
            {
                Console.WriteLine("Помилка: Виконавець не може бути порожнім!");
                return;
            }

            Console.Write("Введіть тривалість (у секундах): ");
            string durationInput = Console.ReadLine();
            if (!int.TryParse(durationInput, out int duration) || duration <= 0)
            {
                Console.WriteLine("Помилка: Тривалість має бути позитивним цілим числом!");
                return;
            }

            Console.Write("Введіть формат (наприклад, MP3): ");
            string format = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(format))
            {
                Console.WriteLine("Помилка: Формат не може бути порожнім!");
                return;
            }

            Console.Write("Введіть рік випуску: ");
            string yearInput = Console.ReadLine();
            if (!int.TryParse(yearInput, out int year) || year < 1900 || year > DateTime.Now.Year)
            {
                Console.WriteLine($"Помилка: Рік має бути між 1900 і {DateTime.Now.Year}!");
                return;
            }

            Console.Write("Введіть ціну (наприклад, 9,99): ");
            string priceInput = Console.ReadLine();
            if (!double.TryParse(priceInput, NumberStyles.Any, UkrainianCulture, out double price) || price < 0)
            {
                Console.WriteLine("Помилка: Ціна має бути невід’ємним числом (використовуйте кому для десяткових чисел, наприклад, 9,99)!");
                return;
            }

            // Перевіряємо, чи код унікальний
            if (mediaList.Any(m => m.Code == code))
            {
                Console.WriteLine("Помилка: Матеріал із таким кодом уже існує!");
                return;
            }

            // Створюємо новий аудіо запис і додаємо до списку
            mediaList.Add(new Audio(code, title, author, performer, duration, format, year, price));
            SaveToDatabase();
            Console.WriteLine("Аудіо запис успішно додано!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка при додаванні аудіо: {ex.Message}");
        }
    }

    // Метод для додавання нового відео
    static void AddVideo()
    {
        try
        {
            Console.WriteLine("\n=== Додавання відео ===");
            Console.Write("Введіть код: ");
            string code = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(code))
            {
                Console.WriteLine("Помилка: Код не може бути порожнім!");
                return;
            }

            Console.Write("Введіть назву: ");
            string title = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Помилка: Назва не може бути порожньою!");
                return;
            }

            Console.Write("Введіть режисера: ");
            string director = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(director))
            {
                Console.WriteLine("Помилка: Режисер не може бути порожнім!");
                return;
            }

            Console.Write("Введіть головного актора: ");
            string mainActor = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(mainActor))
            {
                Console.WriteLine("Помилка: Головний актор не може бути порожнім!");
                return;
            }

            Console.Write("Введіть формат (наприклад, MP4): ");
            string format = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(format))
            {
                Console.WriteLine("Помилка: Формат не може бути порожнім!");
                return;
            }

            Console.Write("Введіть рік випуску: ");
            string yearInput = Console.ReadLine();
            if (!int.TryParse(yearInput, out int year) || year < 1900 || year > DateTime.Now.Year)
            {
                Console.WriteLine($"Помилка: Рік має бути між 1900 і {DateTime.Now.Year}!");
                return;
            }

            Console.Write("Введіть ціну (наприклад, 9,99): ");
            string priceInput = Console.ReadLine();
            if (!double.TryParse(priceInput, NumberStyles.Any, UkrainianCulture, out double price) || price < 0)
            {
                Console.WriteLine("Помилка: Ціна має бути невід’ємним числом (використовуйте кому для десяткових чисел, наприклад, 9,99)!");
                return;
            }

            // Перевіряємо, чи код унікальний
            if (mediaList.Any(m => m.Code == code))
            {
                Console.WriteLine("Помилка: Матеріал із таким кодом уже існує!");
                return;
            }

            // Створюємо нове відео і додаємо до списку
            mediaList.Add(new Video(code, title, director, mainActor, format, year, price));
            SaveToDatabase();
            Console.WriteLine("Відео успішно додано!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка при додаванні відео: {ex.Message}");
        }
    }

    // Метод для видалення матеріалу за кодом
    static void RemoveMedia()
    {
        Console.Write("\nВведіть код матеріалу для видалення: ");
        string code = Console.ReadLine();

        // Видаляємо матеріал із колекції
        int removedCount = mediaList.RemoveAll(m => m.Code == code);

        // Перевіряємо, чи було щось видалено
        if (removedCount > 0)
        {
            SaveToDatabase();
            Console.WriteLine("Матеріал успішно видалено!");
        }
        else
        {
            Console.WriteLine("Матеріал із таким кодом не знайдено!");
        }
    }

    // Метод для редагування матеріалу
    static void EditMedia()
    {
        Console.Write("\nВведіть код матеріалу для редагування: ");
        string code = Console.ReadLine();

        // Шукаємо матеріал
        var media = mediaList.FirstOrDefault(m => m.Code == code);
        if (media == null)
        {
            Console.WriteLine("Матеріал із таким кодом не знайдено!");
            return;
        }

        try
        {
            if (media is Audio audio)
            {
                Console.WriteLine("\n=== Редагування аудіо ===");
                Console.Write("Введіть нову ціну (або Enter, щоб залишити без змін, наприклад, 9,99): ");
                string priceInput = Console.ReadLine();
                if (!string.IsNullOrEmpty(priceInput))
                {
                    if (!double.TryParse(priceInput, NumberStyles.Any, UkrainianCulture, out double price) || price < 0)
                    {
                        Console.WriteLine("Помилка: Ціна має бути невід’ємним числом (використовуйте кому для десяткових чисел)!");
                        return;
                    }
                    audio.Price = price;
                }

                Console.Write("Введіть нового виконавця (або Enter, щоб залишити без змін): ");
                string performer = Console.ReadLine();
                if (!string.IsNullOrEmpty(performer))
                    audio.Performer = performer;
            }
            else if (media is Video video)
            {
                Console.WriteLine("\n=== Редагування відео ===");
                Console.Write("Введіть нову ціну (або Enter, щоб залишити без змін, наприклад, 9,99): ");
                string priceInput = Console.ReadLine();
                if (!string.IsNullOrEmpty(priceInput))
                {
                    if (!double.TryParse(priceInput, NumberStyles.Any, UkrainianCulture, out double price) || price < 0)
                    {
                        Console.WriteLine("Помилка: Ціна має бути невід’ємним числом (використовуйте кому для десяткових чисел)!");
                        return;
                    }
                    video.Price = price;
                }

                Console.Write("Введіть нового режисера (або Enter, щоб залишити без змін): ");
                string director = Console.ReadLine();
                if (!string.IsNullOrEmpty(director))
                    video.Director = director;
            }

            SaveToDatabase();
            Console.WriteLine("Матеріал успішно відредаговано!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка при редагуванні: {ex.Message}");
        }
    }

    // Метод для відображення відео, відсортованих за ціною
    static void ShowVideosByPrice()
    {
        Console.WriteLine("\n=== Відео, впорядковані за ціною ===");
        var videos = mediaList.OfType<Video>().OrderBy(v => v.Price).ToList();
        if (videos.Count == 0)
        {
            Console.WriteLine("Відео відсутні!");
            return;
        }

        foreach (var video in videos)
        {
            Console.WriteLine($"{video}, Знижкова ціна: {video.CalculateDiscountedPrice().ToString("N2", UkrainianCulture)}");
        }
    }

    // Універсальний метод пошуку матеріалів
    static void SearchMedia()
    {
        Console.WriteLine("\n=== Пошук матеріалів ===");
        Console.WriteLine("Виберіть тип матеріалів (1 - Аудіо, 2 - Відео, 3 - Усі): ");
        string typeChoice = Console.ReadLine();
        bool filterByAudio = typeChoice == "1";
        bool filterByVideo = typeChoice == "2";
        bool showAll = typeChoice == "3";

        if (!filterByAudio && !filterByVideo && !showAll)
        {
            Console.WriteLine("Невірний вибір типу!");
            return;
        }

        Console.Write("Введіть критерій пошуку (залиште порожнім для відображення всіх): ");
        string criterion = Console.ReadLine()?.ToLower();
        Console.Write("Виберіть поле пошуку (1 - Назва, 2 - Виконавець (для аудіо), 3 - Режисер (для відео), 4 - Без фільтра): ");
        string fieldChoice = Console.ReadLine();

        var results = mediaList.AsEnumerable();
        if (filterByAudio)
            results = results.OfType<Audio>();
        else if (filterByVideo)
            results = results.OfType<Video>();

        if (!string.IsNullOrEmpty(criterion))
        {
            switch (fieldChoice)
            {
                case "1": // Назва
                    results = results.Where(m => m.Title.ToLower().Contains(criterion));
                    break;
                case "2": // Виконавець (тільки для аудіо)
                    if (filterByAudio || showAll)
                        results = results.OfType<Audio>().Where(a => a.Performer.ToLower().Contains(criterion));
                    else
                        Console.WriteLine("Пошук за виконавцем доступний лише для аудіо!");
                    break;
                case "3": // Режисер (тільки для відео)
                    if (filterByVideo || showAll)
                        results = results.OfType<Video>().Where(v => v.Director.ToLower().Contains(criterion));
                    else
                        Console.WriteLine("Пошук за режисером доступний лише для відео!");
                    break;
                case "4": // Без фільтра
                    results = results.Where(m =>
                        m.Title.ToLower().Contains(criterion) ||
                        (m is Audio audio && (audio.Performer.ToLower().Contains(criterion) || audio.Author.ToLower().Contains(criterion))) ||
                        (m is Video video && (video.Director.ToLower().Contains(criterion) || video.MainActor.ToLower().Contains(criterion))));
                    break;
                default:
                    Console.WriteLine("Невірний вибір поля!");
                    return;
            }
        }

        Console.WriteLine("\n=== Результати пошуку ===");
        var resultList = results.ToList();
        if (resultList.Count == 0)
        {
            Console.WriteLine("Матеріали не знайдено!");
            return;
        }

        foreach (var media in resultList)
        {
            Console.WriteLine($"{media}, Знижкова ціна: {media.CalculateDiscountedPrice().ToString("N2", UkrainianCulture)}");
        }
    }
}