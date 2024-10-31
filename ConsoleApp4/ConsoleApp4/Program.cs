using System;

class Program
{
    static void Main()
    {
        // Входные данные
        string[] startTimes = { "10:00", "11:00", "15:00", "15:30", "16:50" }; // Начало занятых интервалов
        int[] durations = { 60, 30, 10, 10, 40 }; // Длительность занятых интервалов
        string workingHours = "08:00-18:00"; // Рабочие часы
        int consultationTime = 30; // Необходимое время для консультации

        // Вывод входных данных в виде таблицы
        Console.WriteLine("Входные данные:");
        Console.WriteLine("Занятые промежутки времени:");
        Console.WriteLine("Начало\tДлительность");
        for (int i = 0; i < startTimes.Length; i++)
        {
            Console.WriteLine($"{startTimes[i]}\t{durations[i]}");
        }
        Console.WriteLine($"Рабочие часы: {workingHours}");
        Console.WriteLine($"Консультационное время: {consultationTime} минут");
        Console.WriteLine();

        // Получаем свободные временные интервалы
        string[] freeIntervals = GetFreeTimeSlots(startTimes, durations, workingHours, consultationTime);

        // Вывод результата
        Console.WriteLine("Свободные временные интервалы:");
        if (freeIntervals.Length == 0)
        {
            Console.WriteLine("Нет свободных интервалов.");
        }
        else
        {
            foreach (var interval in freeIntervals)
            {
                Console.WriteLine(interval);
            }
        }
    }

    static string[] GetFreeTimeSlots(string[] startTimes, int[] durations, string workingHours, int consultationTime)
    {
        // Определяем границы рабочего дня
        string[] workingTime = workingHours.Split('-');
        int workStart = TimeToMinutes(workingTime[0]);
        int workEnd = TimeToMinutes(workingTime[1]);

        // Создаем массив для хранения занятых интервалов
        int busyCount = startTimes.Length;
        int[,] busySlots = new int[busyCount, 2]; // [][0] - начало, [][1] - конец

        // Заполняем массив занятых интервалов
        for (int i = 0; i < busyCount; i++)
        {
            int start = TimeToMinutes(startTimes[i]);
            int end = start + durations[i];
            busySlots[i, 0] = start; // Начало
            busySlots[i, 1] = end;   // Конец
        }

        // Сортируем занятые интервалы по началу с помощью сортировки выбором
        for (int i = 0; i < busyCount - 1; i++)
        {
            for (int j = i + 1; j < busyCount; j++)
            {
                if (busySlots[i, 0] > busySlots[j, 0])
                {
                    // Меняем местами
                    int tempStart = busySlots[i, 0];
                    int tempEnd = busySlots[i, 1];
                    busySlots[i, 0] = busySlots[j, 0];
                    busySlots[i, 1] = busySlots[j, 1];
                    busySlots[j, 0] = tempStart;
                    busySlots[j, 1] = tempEnd;
                }
            }
        }

        // Находим свободные интервалы
        string[] freeSlots = new string[0];
        int currentStart = workStart;

        for (int i = 0; i < busyCount; i++)
        {
            // Если есть свободный промежуток перед занятым интервалом
            while (currentStart + consultationTime <= busySlots[i, 0])
            {
                // Добавляем новый свободный интервал
                Array.Resize(ref freeSlots, freeSlots.Length + 1);
                freeSlots[freeSlots.Length - 1] = $"{MinutesToTime(currentStart)}-{MinutesToTime(currentStart + consultationTime)}";
                currentStart += consultationTime; // Увеличиваем на необходимое время для консультации
            }
            currentStart = Math.Max(currentStart, busySlots[i, 1]); // Обновляем текущее время
        }

        // Проверка свободного времени после последнего занятого интервала до конца рабочего дня
        while (currentStart + consultationTime <= workEnd)
        {
            // Добавляем новый свободный интервал
            Array.Resize(ref freeSlots, freeSlots.Length + 1);
            freeSlots[freeSlots.Length - 1] = $"{MinutesToTime(currentStart)}-{MinutesToTime(currentStart + consultationTime)}";
            currentStart += consultationTime; // Увеличиваем на необходимое время для консультации
        }

        return freeSlots;
    }

    static int TimeToMinutes(string time)
    {
        var parts = time.Split(':');
        return int.Parse(parts[0]) * 60 + int.Parse(parts[1]);
    }

    static string MinutesToTime(int minutes)
    {
        int hours = minutes / 60;
        int mins = minutes % 60;
        return $"{hours:D2}:{mins:D2}";
    }
}
