using System;
using System.IO;
using System.Text.Json;

class Node
{
    public string QuestionOrAnswer { get; set; }
    public Node Yes { get; set; }
    public Node No { get; set; }

    public Node(string questionOrAnswer)
    {
        QuestionOrAnswer = questionOrAnswer;
        Yes = null;
        No = null;
    }
}

class Akinator
{
    private Node root;

    public Akinator(string filePath)
    {
        string json = File.ReadAllText(filePath);
        root = JsonSerializer.Deserialize<Node>(json);
    }

    public void StartGame()
    {
        Console.WriteLine("Выберите режим игры:");
        Console.WriteLine("1.Вы загадываете, компьютер угадывает.");
        Console.WriteLine("2. Компьютер загадывает, вы угадываете.");

        string choice = Console.ReadLine();
        if (choice == "1")
        {
            Console.WriteLine("Вы загадали предмет. Компьютер начнет угадывать.");
            TraverseTree(root);
        }
        else if (choice == "2")
        {
            Console.WriteLine("Компьютер загадывает предмет, попробуйте его угадать.");
            ComputerZagadki(root);
        }
        else
        {
            Console.WriteLine("Неверный выбор.");
        }
    }

    // Метод для режима, когда компьютер загадывает, а игрок угадывает
    private void TraverseTree(Node node)
    {
        if (node == null)
        {
            return;
        }

        Console.WriteLine(node.QuestionOrAnswer);
        string answer = Console.ReadLine().Trim().ToLower();

        if (answer == "да")
        {
            if (node.Yes != null)
            {
                TraverseTree(node.Yes);
            }
            else
            {
                Console.WriteLine("Я угадал!");
            }
        }
        else if (answer == "нет")
        {
            if (node.No != null)
            {
                TraverseTree(node.No);
            }
            else
            {
                // Если тупик, добавляем новый вопрос и ответ
                AddNewQuestion(node);
            }
        }
        else
        {
            Console.WriteLine("Ответьте 'да' или 'нет'.");
            TraverseTree(node); // Снова задаём тот же вопрос
        }
    }

    // Метод для режима, когда игрок загадывает, а компьютер пытается угадать
    private void ComputerZagadki(Node node)
    {
        if (node == null)
        {
            return;
        }

        // Прямо задаем вопросы, чтобы игрок отвечал
        if (node.Yes == null && node.No == null)
        {
            Console.WriteLine($"Компьютер загадал: {node.QuestionOrAnswer}");
            Console.WriteLine("Вы угадали предмет?");
            string answer = Console.ReadLine().Trim().ToLower();

            if (answer == "да")
            {
                Console.WriteLine("Вы угадали предмет!");
            }
            else
            {
                Console.WriteLine("Вы не угадали предмет. Попробуйте снова.");
            }
        }
        else
        {
            Console.WriteLine(node.QuestionOrAnswer);
            string answer = Console.ReadLine().Trim().ToLower();

            if (answer == "да")
            {
                ComputerZagadki(node.Yes);
            }
            else if (answer == "нет")
            {
                ComputerZagadki(node.No);
            }
            else
            {
                Console.WriteLine("Ответьте 'да' или 'нет'.");
                ComputerZagadki(node); // Задаем тот же вопрос снова
            }
        }
    }

    private void AddNewQuestion(Node node)
    {
        Console.WriteLine("Я не смог угадать. Что это было?");
        string correctAnswer = Console.ReadLine();

        Console.WriteLine($"Какой вопрос поможет отличить {correctAnswer} от других?");
        string newQuestion = Console.ReadLine();

        Console.WriteLine($"Для {correctAnswer} ответ будет 'да' или 'нет'?");
        string correctAnswerResponse = Console.ReadLine().Trim().ToLower();

        // Текущий ответ становится "нет", а новое правильное - "да"
        Node oldAnswerNode = new Node(node.QuestionOrAnswer);
        node.QuestionOrAnswer = newQuestion;

        if (correctAnswerResponse == "да")
        {
            node.Yes = new Node(correctAnswer);
            node.No = oldAnswerNode;
        }
        else
        {
            node.No = new Node(correctAnswer);
            node.Yes = oldAnswerNode;
        }

        Console.WriteLine("Спасибо! Теперь я буду знать больше.");
    }


    public void SaveTree(string filePath)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(root, options);
        File.WriteAllText(filePath, json);
        Console.WriteLine("Дерево сохранено в файл.");
    }
}

class Program
{
    static void Main(string[] args)
    {
        string filePath = "akinator_tree_large.json"; // Укажите путь к JSON файлу с деревом
        Akinator akinator = new Akinator(filePath);

        akinator.StartGame();
        akinator.SaveTree(filePath);
    }
}
