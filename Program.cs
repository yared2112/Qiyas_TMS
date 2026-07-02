using TmsCore;


Console.WriteLine("\n---  Defeating the Pyramid of Doom ---");

var service = new EnrollmentService();

// (Valid registration)
var validStudent = new Student { Id = "S1", Name = "Abeba", Age = 20, GPA = 3.8m };
var validCourse = new Course { Code = "CS-401", Title = "Advanced C#", Capacity = 30 };

var resultRecord = service.ProcessRegistration(validStudent, validCourse);
Console.WriteLine($"Enrolled: {resultRecord.StudentId} in {resultRecord.CourseCode}");


// Null Student Guard Clause Test
try
{
    Console.WriteLine("\nTesting Null Student Guard...");
    service.ProcessRegistration(null, validCourse);
}
catch (ArgumentNullException ex)
{
    Console.WriteLine($"Guard caught: {ex.ParamName} cannot be null.");
}


// Full Course Business Rule Guard Test
var fullCourse = new Course { Code = "CS-402", Title = "Full Course", Capacity = 1 };
fullCourse.EnrolledCount = 1; // Simulate a full course

try
{
    Console.WriteLine("\nTesting Full Course Business Rule Guard...");
    service.ProcessRegistration(validStudent, fullCourse);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Business rule: {ex.Message}");
}

// Step 1: Create the Student Data (C# 12+ Collection Expressions)
List<Student> students = [
    new Student { Id = "S1", Name = "Abeba", Age = 22, GPA = 3.8m },
    new Student { Id = "S2", Name = "Kidane", Age = 21, GPA = 2.4m },
    new Student { Id = "S3", Name = "Dawit", Age = 20, GPA = 3.1m },
    new Student { Id = "S4", Name = "Sara", Age = 23, GPA = 3.9m },
    new Student { Id = "S5", Name = "Frehiwot", Age = 19, GPA = 2.0m },
    new Student { Id = "S6", Name = "Yonas", Age = 24, GPA = 3.5m },
    new Student { Id = "S7", Name = "Meron", Age = 22, GPA = 1.8m },
    new Student { Id = "S8", Name = "Tesfaye", Age = 21, GPA = 2.9m }
];

// Step 2: Build the Honors Leaderboard
var leaderboard = students
    .Where(s => s.GPA >= 3.5m)               // TODO 1: Extract students where GPA is >= 3.5m
    .OrderByDescending(s => s.GPA)          // TODO 2: Sort the remaining students by GPA descending
    .Select(s => s.Name)                     // TODO 3: Project the result so we only keep the 'Name' string
    .ToList();                               // TODO 4: Materialize the lazy query into a concrete List

Console.WriteLine($"Found {leaderboard.Count} Honors Students:");
foreach (var name in leaderboard)
{
    Console.WriteLine($"- {name}");
}

// Step 3: Class Average
// TODO 5: Use LINQ to calculate the average GPA across all students.
decimal averageGpa = students.Average(s => s.GPA);
Console.WriteLine($"\nClass Average GPA: {averageGpa:F2}");

// Step 4: Group by Academic Standing
// TODO 6: Use .GroupBy with a switch expression to classify each student.
var standingGroups = students.GroupBy(s => s.GPA switch
{
    >= 3.5m => "Honors",
    >= 2.5m => "Good Standing",
    >= 2.0m => "Probation",
    _ => "Academic Warning"
});

Console.WriteLine("\n--- Academic Standing Report ---");
foreach (var group in standingGroups)
{
    Console.WriteLine($"\n{group.Key} ({group.Count()}):");
    foreach (var s in group)
    {
        Console.WriteLine($"  {s.Name} GPA: {s.GPA}");
    }
}

// Step 5: Collection Expressions with Spread
// TODO 7: Use the spread operator (..) to merge two arrays and append a value.
string[] backendCourses = ["C#", "ASP.NET Core"];
string[] frontendCourses = ["TypeScript", "Angular"];
string[] allCourses = [.. backendCourses, .. frontendCourses, "Capstone"];

Console.WriteLine($"\nFull curriculum: {string.Join(", ", allCourses)}");
