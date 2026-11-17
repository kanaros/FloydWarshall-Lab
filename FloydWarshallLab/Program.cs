using System;
using System.Collections.Generic;

namespace FloydWarshallLab
{
    class Program
    {
        // Створюємо нескінченність. Беремо макс число і ділимо на 2
        // Це робиться, щоб при додаванні двох нескінченностей не сталося переповнення пам'яті через що зламалося логіка пошуку
        const int INF = int.MaxValue / 2;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // 1 завдання
            Console.WriteLine("=== Завдання 1: Граф за матрицею суміжності ===");
            int[,] adjMatrixVar5 = {
                { 0, 1, 1, 1, 0 },
                { 1, 0, 0, 1, 0 },
                { 1, 0, 0, 1, 0 },
                { 0, 0, 1, 0, 0 },
                { 1, 1, 0, 1, 0 }
            };
            PrintMatrix(adjMatrixVar5, false, false);
            Console.WriteLine();


            // 2 завдання
            Console.WriteLine("=== Завдання 2: Алгоритм Флойда-Уоршелла ===");

            // Виклик функції, яка змушує користувача ввести матрицю
            int[,] distGraph = ReadMatrixFromConsole();
            int n = distGraph.GetLength(0);

            int[,] dist = new int[n, n]; // змінна з найменшими знайденими відстанями
            int[,] next = new int[n, n]; // номер наступної вершини для побудови маршруту.

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    dist[i, j] = distGraph[i, j]; // переносимо відстань
                    if (distGraph[i, j] != INF && i != j) // перевіряємо чи є нескінченність і чи не стоїмо на місті
                        next[i, j] = j;
                    else
                        next[i, j] = -1; // -1 означає що шляху немає
                }
            }

            // Алгоритм Флойда-Уоршелла
            for (int k = 0; k < n; k++) // k — проміжна вершина через яку пробуємо пройти
            {
                for (int i = 0; i < n; i++) // i — початкова вершина
                {
                    for (int j = 0; j < n; j++) // j — кінцева вершина
                    {
                        if (dist[i, k] != INF && dist[k, j] != INF && dist[i, k] + dist[k, j] < dist[i, j]) // шукаємо найкоротший шлях
                        {
                            dist[i, j] = dist[i, k] + dist[k, j];
                            next[i, j] = next[i, k];
                        }
                    }
                }
            }

            Console.WriteLine("\n--- Матриця мінімальних відстаней ---");
            PrintMatrix(dist, true, false);

            Console.WriteLine("\n--- Матриця найкоротших шляхів (наступна вершина) ---");
            PrintMatrix(next, false, true);

            Console.WriteLine("\n--- Пошук шляху для будь-якої пари вершин ---");
            while (true)
            {
                Console.WriteLine($"\nВведіть початкову вершину (від 1 до {n}) або -1 для виходу:");
                int start = GetValidInput(-1, n); // перевірка даних
                if (start == -1) break;

                Console.WriteLine($"Введіть кінцеву вершину (від 1 до {n}):");
                int end = GetValidInput(1, n);

                // Переводимо номери вершин у індекси масиву
                start--;
                end--;

                PrintPath(start, end, dist, next);
            }
        }

        static void PrintPath(int u, int v, int[,] dist, int[,] next)
        {
            if (dist[u, v] == INF) // перевірка існування шляху
            {
                Console.WriteLine($"Шляху від {u + 1} до {v + 1} не існує.");
                return;
            }

            Console.Write($"Мінімальна відстань ({u + 1} -> {v + 1}): {dist[u, v]}. Маршрут: {u + 1}");
            int current = u;
            while (current != v)
            {
                current = next[current, v];

                if (current == -1)
                {
                    Console.WriteLine("\nПомилка побудови маршруту.");
                    return;
                }

                Console.Write($" -> {current + 1}");
            }
            Console.WriteLine();
        }

        static int GetValidInput(int min, int max) // перевірка на правильність введення даних
        {
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int result) && result >= min && result <= max)
                    return result;

                Console.WriteLine($"Помилка! Будь ласка, введіть ціле число від {min} до {max}:");
            }
        }

        // Введення матриці
        static int[,] ReadMatrixFromConsole() // функція для зчитування матриці з консолі
        {
            Console.Write("\nВведіть кількість вершин графу: ");
            int n = GetValidInput(2, 20);
            int[,] matrix = new int[n, n];

            Console.WriteLine("Введіть матрицю відстаней рядок за рядком, значення розділяйте пробелом.");
            Console.WriteLine("Для нескінченності (відсутності ребра) введіть 'i' або '∞':");

            for (int i = 0; i < n; i++)
            {
                while (true)
                {
                    Console.Write($"Рядок {i + 1}: ");
                    string input = Console.ReadLine(); // читаємо весь введений рядок тексту
                    // Розбиваємо рядок на окремі слова (елементи масиву) по пробілах.
                    string[] parts = input.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    // Якщо ввели менше або більше елементів ніж кількість вершин просимо переробити.
                    if (parts.Length != n)
                    {
                        Console.WriteLine($"Помилка! Очікується {n} елементів. Спробуйте ще раз.");
                        continue;
                    }

                    bool validRow = true; // змінна яка позначає чи правильний весь рядок
                    // Перебираємо кожне слово в рядку і намагаємося перетворити його на число або INF
                    for (int j = 0; j < n; j++)
                    {
                        string p = parts[j];
                        if (p == "i" || p == "∞")
                        {
                            matrix[i, j] = INF;
                        }
                        else if (int.TryParse(parts[j], out int val))
                        {
                            matrix[i, j] = val; // Записуємо коректне число в матрицю
                        }
                        else
                        {
                            Console.WriteLine($"Помилка у значенні '{parts[j]}'. Введіть весь рядок заново.");
                            validRow = false;
                            break;
                        }
                    }
                    if (validRow) break; // Якщо весь рядок правильний виходимо з циклу while і йдемо до наступного рядка i
                }
            }
            return matrix;
        }

        // метод використовується в програмі для виведення всіх наших матриць
        static void PrintMatrix(int[,] matrix, bool isDist, bool isNext) // isDist та isNext вказують, як саме форматувати символи INF або -1
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (isDist && matrix[i, j] == INF)
                        Console.Write("i\t");
                    else if (isNext && matrix[i, j] == -1)
                        Console.Write("-\t");
                    else if (isNext)
                        Console.Write((matrix[i, j] + 1) + "\t"); // Додаємо 1, щоб показувати номери вершин від 1
                    else
                        Console.Write(matrix[i, j] + "\t"); // \t - табуляція для рівних стовпців
                }
                Console.WriteLine();
            }
        }
    }
}
