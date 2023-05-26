using System;
using System.Collections.Generic;
using System.IO;


namespace ProjectManagementSystem
{
    class Task
    {
        public string Name { get; set; }
        public int CompletionTime { get; set; }
        public List<string> Dependencies { get; set; }

        public Task(string name, int completionTime, List<string> dependencies)
        {
            Name = name;
            CompletionTime = completionTime;
            Dependencies = dependencies;
        }
    }

    class Program
    {
        static Dictionary<string, Task> tasks = new Dictionary<string, Task>();
        static string inputFilePath = "";

        static void Main(string[] args)
        {
            bool exit = false;

            while (!exit)
            {
                //Interface
                Console.WriteLine("+=========================+");
                Console.WriteLine("|Project Management System|");
                Console.WriteLine("+=========================+");
                Console.WriteLine("1. Load project from a text file");
                Console.WriteLine("2. Add a new task");
                Console.WriteLine("3. Remove a task");
                Console.WriteLine("4. Change completion time");
                Console.WriteLine("5. Save project to file");
                Console.WriteLine("6. Find task sequence");
                Console.WriteLine("7. Find earliest task times");
                Console.WriteLine("8. Exit");
                Console.WriteLine("Enter your choice:");

                int options;
                if (int.TryParse(Console.ReadLine(), out options))
                {
                    //Options
                    switch (options)
                    {
                        case 1:
                            LoadFromFile();
                            break;
                        case 2:
                            AddNewT();
                            break;
                        case 3:
                            RemoveT();
                            break;
                        case 4:
                            ChangeCTime();
                            break;
                        case 5:
                            SaveToFile();
                            break;
                        case 6:
                            Tasksequence();
                            break;
                        case 7:
                            TaskEarliestTTimes();
                            break;
                        case 8:
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("|Invalid choice| Please try again.");
                            break;
                    }
                }
                else
                {
                    //Exception
                    Console.WriteLine("|Invalid input| Please enter a valid choice.");
                }

                Console.WriteLine();
            }
        }

        //Select file and load it into program
        static void LoadFromFile()
        {
            Console.WriteLine("Enter the path of the text file:");
            inputFilePath = Console.ReadLine();

            try
            {
                string[] lines = File.ReadAllLines(inputFilePath);
                tasks.Clear();

                foreach (string line in lines)
                {
                    string[] elements = line.Split(',');

                    string taskName = elements[0].Trim();
                    int completionTime = int.Parse(elements[1].Trim());
                    List<string> dependencies = new List<string>();

                    for (int i = 2; i < elements.Length; i++)
                    {
                        dependencies.Add(elements[i].Trim());
                    }

                    Task task = new Task(taskName, completionTime, dependencies);
                    tasks.Add(taskName, task);
                }

                Console.WriteLine("Project loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading project: " + ex.Message);
            }
        }

        //Add new task into opened file
        static void AddNewT()
        {
            Console.WriteLine("Enter the task name:");
            string taskName = Console.ReadLine();

            if (tasks.ContainsKey(taskName))
            {
                Console.WriteLine("Task already exists.");
                return;
            }

            Console.WriteLine("Enter the completion time for the task:");
            int completionTime;

            if (!int.TryParse(Console.ReadLine(), out completionTime))
            {
                Console.WriteLine("Invalid completion time. Task not added.");
                return;
            }

            Console.WriteLine("Enter the names of tasks that this task depends on (separated by commas):");
            string dependenciesInput = Console.ReadLine();
            List<string> dependencies = new List<string>();

            if (!string.IsNullOrEmpty(dependenciesInput))
            {
                string[] dependencyNames = dependenciesInput.Split(',');

                foreach (string dependencyName in dependencyNames)
                {
                    string dependency = dependencyName.Trim();

                    if (!tasks.ContainsKey(dependency))
                    {
                        Console.WriteLine($"Dependency task '{dependency}' does not exist. Task not added.");
                        return;
                    }

                    dependencies.Add(dependency);
                }
            }

            Task newTask = new Task(taskName, completionTime, dependencies);
            tasks.Add(taskName, newTask);

            Console.WriteLine("Task added successfully.");
        }

        //Remove task which is opened
        static void RemoveT()
        {
            Console.WriteLine("Enter the name of the task to remove:");
            string taskName = Console.ReadLine();

            if (tasks.ContainsKey(taskName))
            {
                tasks.Remove(taskName);
                foreach (Task task in tasks.Values)
                {
                    task.Dependencies.RemoveAll(dependency => dependency.Equals(taskName, StringComparison.OrdinalIgnoreCase));
                }
                Console.WriteLine("Task removed successfully.");
            }
            else
            {
                Console.WriteLine("Task not found.");
            }
        }

        //Change task's completion time 
        static void ChangeCTime()
        {
            Console.WriteLine("Enter the name of the task to change completion time:");
            string taskName = Console.ReadLine();

            if (tasks.ContainsKey(taskName))
            {
                Console.WriteLine("Enter the new completion time:");
                int newCompletionTime;

                if (int.TryParse(Console.ReadLine(), out newCompletionTime))
                {
                    tasks[taskName].CompletionTime = newCompletionTime;
                    Console.WriteLine("Completion time changed successfully.");
                }
                else
                {
                    Console.WriteLine("Invalid completion time.");
                }
            }
            else
            {
                Console.WriteLine("Task not found.");
            }
        }

        //Save any changes in opened file
        static void SaveToFile()
        {
            if (string.IsNullOrEmpty(inputFilePath))
            {
                Console.WriteLine("No input file loaded. Please load the project from a file first.");
                return;
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(inputFilePath))
                {
                    foreach (Task task in tasks.Values)
                    {
                        string line = task.Name + ", " + task.CompletionTime;

                        if (task.Dependencies.Count > 0)
                        {
                            line += ", " + string.Join(", ", task.Dependencies);
                        }

                        writer.WriteLine(line);
                    }
                }

                Console.WriteLine("Project saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving project: " + ex.Message);
            }
        }

        //To show the sequence of the text and save it in seqeunce.txt file
        private static void Tasksequence()
        {
            Dictionary<string, int> sequencet = new Dictionary<string, int>();

            foreach (Task task in tasks.Values)
            {
                findsequenceDFS(task, sequencet);
            }

            Console.WriteLine("Sequence of the text:");
            foreach (var kvp in sequencet)
            {
                Console.WriteLine($"{kvp.Key}");
            }

            SavesequenceToTextFile(sequencet);
        }

        //Find the sequence with traversal depth first search algorithm
        private static int findsequenceDFS(Task task, Dictionary<string, int> sequencet)
        {
            if (sequencet.ContainsKey(task.Name))
            {
                return sequencet[task.Name];
            }

            if (task.Dependencies.Count == 0)
            {
                sequencet[task.Name] = 0;
            }
            else
            {
                int maxDependencyTime = 0;
                foreach (string dependency in task.Dependencies)
                {
                    Task dependentTask;
                    if (tasks.TryGetValue(dependency, out dependentTask))
                    {
                        int dependentTaskTime = findsequenceDFS(dependentTask, sequencet);
                        maxDependencyTime = Math.Max(maxDependencyTime, dependentTaskTime + dependentTask.CompletionTime);
                    }
                }
                sequencet[task.Name] = maxDependencyTime;
            }

            return sequencet[task.Name];
        }

        //Save it into file
        private static void SavesequenceToTextFile(Dictionary<string, int> sequencet)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter("Sequence.txt"))
                {
                    foreach (var kvp in sequencet)
                    {
                        writer.WriteLine($"{kvp.Key}");
                    }
                }

                Console.WriteLine("Task sequence found and saved to 'Sequence.txt'..");
            }
            catch (Exception)
            {
                Console.WriteLine("An error occurred while saving Sequence task to the text file.");
            }
        }

        //To show earliest task times and save it in Earliesttimes.txt 
        private static void TaskEarliestTTimes()
        {
            Dictionary<string, int> earliestTimes = new Dictionary<string, int>();

            List<Task> sortedTasks = new List<Task>(tasks.Values);
            sortedTasks.Sort((t1, t2) => t1.Dependencies.Count.CompareTo(t2.Dependencies.Count));

            foreach (Task task in sortedTasks)
            {
                EarliestTTimeInsertion(task, earliestTimes);
            }

            Console.WriteLine("Earliest Task Times:");
            foreach (var kvp in earliestTimes)
            {
                Console.WriteLine($"{kvp.Key}, {kvp.Value}");
            }

            SaveEarliestToTextFile(earliestTimes);
        }

        //Find earliest task time using insertion algorithm
        private static void EarliestTTimeInsertion(Task task, Dictionary<string, int> earliestTimes)
        {
            int maxDependencyTime = 0;

            foreach (string dependency in task.Dependencies)
            {
                Task dependentTask;
                if (tasks.TryGetValue(dependency, out dependentTask))
                {
                    if (!earliestTimes.ContainsKey(dependency))
                    {
                        EarliestTTimeInsertion(dependentTask, earliestTimes);
                    }

                    int dependentTaskTime = earliestTimes[dependency] + dependentTask.CompletionTime;
                    maxDependencyTime = Math.Max(maxDependencyTime, dependentTaskTime);
                }
            }

            earliestTimes[task.Name] = maxDependencyTime;
        }

        //Save earliest task time into txt file
        private static void SaveEarliestToTextFile(Dictionary<string, int> earliestTimes)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter("EarliestTimes.txt"))
                {
                    foreach (var kvp in earliestTimes)
                    {
                        writer.WriteLine($"{kvp.Key}, {kvp.Value}");
                    }
                }

                Console.WriteLine("Earliest task times saved to 'EarliestTimes.txt'.");
            }
            catch (Exception)
            {
                Console.WriteLine("An error occurred while saving earliest task times to the text file.");
            }

        }
    }
}