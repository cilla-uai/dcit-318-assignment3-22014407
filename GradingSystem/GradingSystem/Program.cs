using System;
using System.Collections.Generic;
using System.IO;

namespace GradingSystem
{
    public class Student
    {
        public int Id { get; }
        public string FullName { get; }
        public int Score { get; }

        public Student(int id, string fullName, int score)
        {
            Id = id;
            FullName = fullName;
            Score = score;
        }

        public string GetGrade()
        {
            if (Score >= 80) return "A";
            if (Score >= 70) return "B";
            if (Score >= 60) return "C";
            if (Score >= 50) return "D";
            return "F";
        }
    }

    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message) : base(message) { }
    }

    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message) { }
    }

    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            var students = new List<Student>();

            using (var reader = new StreamReader(inputFilePath))
            {
                string? line;
                int lineNumber = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;

                    // Skip blank lines so they do not crash the parser
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var parts = line.Split(',');

                    if (parts.Length < 3)
                    {
                        throw new MissingFieldException(
                            $"Line {lineNumber}: expected 3 fields (id, name, score) but found {parts.Length}.");
                    }

                    if (!int.TryParse(parts[0].Trim(), out int id))
                    {
                        throw new InvalidScoreFormatException(
                            $"Line {lineNumber}: student ID '{parts[0].Trim()}' is not a valid number.");
                    }

                    if (!int.TryParse(parts[2].Trim(), out int score))
                    {
                        throw new InvalidScoreFormatException(
                            $"Line {lineNumber}: score '{parts[2].Trim()}' is not a valid integer.");
                    }

                    students.Add(new Student(id, parts[1].Trim(), score));
                }
            }

            return students;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using (var writer = new StreamWriter(outputFilePath))
            {
                foreach (var student in students)
                {
                    writer.WriteLine($"{student.FullName} (ID:{student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
                }
            }
        }
    }

    public class Program
    {
        public static void Main()
        {
            var processor = new StudentResultProcessor();

            try
            {
                var students = processor.ReadStudentsFromFile("students.txt");
                processor.WriteReportToFile(students, "report.txt");
                Console.WriteLine($"Report generated for {students.Count} students.");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Error: the input file students.txt was not found.");
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }
    }
}