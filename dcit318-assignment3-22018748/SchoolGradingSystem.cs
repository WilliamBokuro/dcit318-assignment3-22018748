using System;
using System.Collections.Generic;
using System.IO;

namespace SchoolGradingSystem
{
    // a. Student Class
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int Score { get; set; }

        public Student(int id, string fullName, int score)
        {
            Id = id;
            FullName = fullName;
            Score = score;
        }

        public string GetGrade()
        {
            if (Score >= 80 && Score <= 100) return "A";
            if (Score >= 70 && Score <= 79) return "B";
            if (Score >= 60 && Score <= 69) return "C";
            if (Score >= 50 && Score <= 59) return "D";
            return "F";
        }
    }

    // b. Custom Exception: InvalidScoreFormatException
    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message) : base(message) { }
    }

    // c. Custom Exception: MissingFieldException
    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message) { }
    }

    // d. StudentResultProcessor Class
    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            List<Student> students = new List<Student>();

            if (!File.Exists(inputFilePath))
            {
                throw new FileNotFoundException($"Input file not found at: {inputFilePath}");
            }

            using (StreamReader reader = new StreamReader(inputFilePath))
            {
                string? line;
                int lineNumber = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split(',');

                    // Validate missing fields (Requires ID, Name, Score)
                    if (parts.Length < 3 || string.IsNullOrWhiteSpace(parts[0]) ||
                        string.IsNullOrWhiteSpace(parts[1]) || string.IsNullOrWhiteSpace(parts[2]))
                    {
                        throw new MissingFieldException($"Line {lineNumber} is incomplete or missing required fields: '{line}'");
                    }

                    // Validate ID format
                    if (!int.TryParse(parts[0].Trim(), out int id))
                    {
                        throw new FormatException($"Line {lineNumber}: Student ID '{parts[0]}' is not a valid integer.");
                    }

                    string fullName = parts[1].Trim();

                    // Validate Score format
                    if (!int.TryParse(parts[2].Trim(), out int score))
                    {
                        throw new InvalidScoreFormatException($"Line {lineNumber}: Score '{parts[2]}' is not a valid integer.");
                    }

                    students.Add(new Student(id, fullName, score));
                }
            }

            return students;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                foreach (var student in students)
                {
                    writer.WriteLine($"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
                }
            }
        }

        // e. Main Flow
        public static void Main(string[] args)
        {
            StudentResultProcessor processor = new StudentResultProcessor();
            string inputFile = "students.txt";
            string outputFile = "summary_report.txt";

            // Create a test input file automatically for demonstration
            File.WriteAllText(inputFile, "101, Alice Smith, 84\n102, Bob Jones, 72\n103, Charlie Brown, 48");

            try
            {
                Console.WriteLine("Reading student data from file...");
                List<Student> students = processor.ReadStudentsFromFile(inputFile);

                Console.WriteLine("Writing summary report...");
                processor.WriteReportToFile(students, outputFile);

                Console.WriteLine($"Process finished successfully! Report saved to '{outputFile}'.\n");
                Console.WriteLine("Report Content:");
                Console.WriteLine(File.ReadAllText(outputFile));
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"[FILE ERROR] {ex.Message}");
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine($"[SCORE ERROR] {ex.Message}");
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine($"[DATA ERROR] {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UNFORESEEN ERROR] {ex.Message}");
            }
        }
    }
}