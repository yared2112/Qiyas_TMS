using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
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

// 1. THE WRONG WAY: Blocking with Thread.Sleep
// The running thread is forcibly HELD for 1500ms total. It cannot serve any other requests.
var sw = Stopwatch.StartNew();
for (int i = 0; i < 5; i++)
{
    Thread.Sleep(300);
}
Console.WriteLine($"Blocking sequential: {sw.ElapsedMilliseconds}ms");


// 2. ASYNC BUT STILL SEQUENTIAL: Thread released, but calls are one-at-a-time
// The thread is released back to the pool during delays, but the loops wait one after another.
sw.Restart();
for (int i = 0; i < 5; i++)
{
    await Task.Delay(300);
}
Console.WriteLine($"Async sequential: {sw.ElapsedMilliseconds}ms");


// 3. THE RIGHT WAY: Async parallel all 5 start simultaneously
// All 5 operations kick off at the exact same moment. The pool is free, and they resolve concurrently.
sw.Restart();
var tasks = Enumerable.Range(0, 5).Select(_ => Task.Delay(300));
await Task.WhenAll(tasks);
Console.WriteLine($"Async parallel: {sw.ElapsedMilliseconds}ms");

// Step 3: Async Parallel with Task.WhenAll
var swTotal = Stopwatch.StartNew();

string[] studentIds = ["S1", "S2", "S3", "S4", "S5"];
string[] courseCodes = ["CRS-101", "CRS-201", "CRS-301"];

// TODO 8: Use LINQ to create a collection of tasks for fetching students and courses concurrently. 
var studentTasks = studentIds.Select(id => FetchStudentAsync(id));
var courseTasks = courseCodes.Select(code => FetchCourseAsync(code));

// TODO 9: Use Task.WhenAll to wait for all tasks to complete.
Student[] loadedStudents = await Task.WhenAll(studentTasks);
Course[] loadedCourses = await Task.WhenAll(courseCodes.Select(code => FetchCourseAsync(code)));

Console.WriteLine($"\nLoaded {loadedStudents.Length} students and {loadedCourses.Length} courses in {swTotal.ElapsedMilliseconds}ms");

// Step 4: Async Parallel with Task.WhenAll
foreach (var s in loadedStudents)
{
    Console.WriteLine($"  {s.Name} GPA: {s.GPA}");
}


#region DATABASE SIMULATOR METHODS
// Step 3: Async Parallel with Task.WhenAll

async Task<Student> FetchStudentAsync(string id)
{
    Console.WriteLine($"  [DB RUNNING] Fetching student {id}...");
    await Task.Delay(300); // Latency simulation for database fetch

    return new Student
    {
        Id = id,
        Name = $"Student-{id}",
        Age = 20,
        GPA = id switch
        {
            "S1" => 3.8m,
            "S2" => 2.4m,
            "S3" => 3.5m,
            "S4" => 1.9m,
            "S5" => 3.2m,
            _ => 2.5m
        }
    };
}

async Task<Course> FetchCourseAsync(string code)
{
    Console.WriteLine($"  [DB RUNNING] Fetching course {code}...");
    await Task.Delay(200); // Latency simulation for database fetch

    return new Course
    {
        Code = code,
        Title = $"Course-{code}",
        Capacity = code switch
        {
            "CRS-101" => 2,
            "CRS-201" => 30,
            "CRS-301" => 15,
            _ => 25
        }
    };
}
#endregion

// 1. Initialize our targeted training constraints
var enrollCourse = new Course { Code = "CRS-101", Title = "C# Mastery", Capacity = 2 };
var enrollService = new EnrollmentService();

var enrollments = new List<EnrollmentRecord>();
var failures = new List<string>();

var swEngine = Stopwatch.StartNew();

// 2. Loop through the array of parallel-fetched student profiles
// Note: We utilize 'loadedStudents' generated dynamically in Step 3
foreach (var student in loadedStudents)
{
    try
    {
        // Guard rules run internally inside the service here
        var record = enrollService.ProcessRegistration(student, enrollCourse);

        // Mutate the local tracking status only if the transaction passes validation
        enrollCourse.EnrolledCount++;
        enrollments.Add(record);

        Console.WriteLine($"  [SUCCESS] Enrolled: {student.Name} (Current Count: {enrollCourse.EnrolledCount}/{enrollCourse.Capacity})");
    }
    catch (InvalidOperationException ex)
    {
        // Gracefully catch and catalog business rule violations
        failures.Add($"{student.Name}: {ex.Message}");
        Console.WriteLine($"  [REJECTED] Rejected: {student.Name} -> Reason: {ex.Message}");
    }
}

// 3. Render the Consolidated Analytics Report
Console.WriteLine($"\n--- Enrollment Processing Completed in {swEngine.ElapsedMilliseconds}ms ---");
Console.WriteLine($"Total Successful Enrollments: {enrollments.Count}");
Console.WriteLine($"Total Failures/Rejections: {failures.Count}");

if (failures.Count > 0)
{
    Console.WriteLine("\nFailure Details:");
    foreach (var failure in failures)
    {
        Console.WriteLine($"  - {failure}");
    }
}

// 1. Dispatch background emails exclusively to successfully processed enrollments
foreach (var record in enrollments)
{
    // Correlate the fact record back to our loaded domain cache
    var student = loadedStudents.FirstOrDefault(s => s.Id == record.StudentId);

    if (student != null)
    {
        // SAFE FIRE-AND-FORGET: Notice there is NO 'await' keyword here.
        // The task triggers instantly in the background, but the execution loop continues immediately.
        _ = SendConfirmationAsync(student);
    }
}

// 2. Observe the main application thread continuing execution instantly
Console.WriteLine("  [MAIN THREAD] Core enrollment tasks complete. Main thread is moving on...");

// Hold the console process alive momentarily so background threads can process output before termination
await Task.Delay(500);


#region BACKGROUND ASSISTANT METHODS
// These execution blocks are written as local methods or class-level procedures

async Task SendConfirmationAsync(Student student)
{
    try
    {
        Console.WriteLine($"  [BACKGROUND] Initializing SMTP payload delivery for {student.Name}...");

        // Simulate real-world network transmission latency
        await Task.Delay(150);

        // SIMULATED FAILURE FAULT: Force a simulated server timeout for Student-S2 to test resiliency
        if (student.Id == "S2")
        {
            throw new System.Net.Mail.SmtpException("SMTP target host connection timed out.");
        }

        Console.WriteLine($"  [SUCCESS EMAIL] Confirmation dispatch successfully completed for {student.Name}");
    }
    catch (Exception ex)
    {
        // PRODUCTION ARCHITECTURE GOLDEN RULE: 
        // We log the context details here but DO NOT re-throw.
        // This isolates background subsystem exceptions from tearing down primary business engines.
        Console.WriteLine($"  [FATAL ERROR LOGGED] Background notification breakdown for {student.Name}: {ex.Message}");
    }
}
#endregion

Console.WriteLine("\n--- Starting Exercise 5: Custom Domain Exceptions ---");
var diagnosticService = new EnrollmentService();

try
{
    // 1. Capacity = 1 passes the Course auto-property field validation successfully
    var overflowCourse = new Course { Code = "CRS-999", Title = "Overflow Test", Capacity = 1 };

    // 2. Explicitly saturate the course state: 1 seat available, 1 seat taken. The course is now FULL.
    overflowCourse.EnrolledCount = 1;

    Console.WriteLine("Attempting enrollment processing on a full course (1/1 seats taken)...");

    diagnosticService.ProcessRegistration(
        new Student { Id = "S99", Name = "Diagnostic Student", Age = 22, GPA = 3.0m },
        overflowCourse
    );
}
catch (CapacityReachedException ex)
{
    // The enrollment service will now hit the '1 >= 1' condition and throw this specific domain failure
    Console.WriteLine($"\n[DOMAIN EXCEPTION CAUGHT SUCCESSFULLY]");
    Console.WriteLine($"  Violated Course Code : {ex.CourseCode}");
    Console.WriteLine($"  Resulting Message    : {ex.Message}");
}


// 1. Terminate the overarching diagnostics timer to capture full throughput efficiency
swTotal.Stop();

Console.WriteLine("\n--- Starting Final Integration: The Enrollment Report ---");

// 2. Leverage LINQ to aggregate memory cache metrics safely
// Note: We access the loaded arrays generated dynamically during parallel execution
decimal classAverage = loadedStudents.Length > 0
    ? loadedStudents.Average(s => s.GPA)
    : 0m;

// 3. Render the Final Consolidated Management Summary Report
Console.WriteLine("\n========== ENROLLMENT SUMMARY ==========");
Console.WriteLine($"Total students loaded   : {loadedStudents.Length}");
Console.WriteLine($"Successful enrollments : {enrollments.Count}");
Console.WriteLine($"Failed enrollments     : {failures.Count}");
Console.WriteLine($"Class average GPA      : {classAverage:F2}");
Console.WriteLine($"Total elapsed time     : {swTotal.ElapsedMilliseconds}ms");

// 4. Loop through and expose isolated domain rejection contexts if they exist
if (failures.Count > 0)
{
    Console.WriteLine("\n--- Failure Details ---");
    foreach (var failure in failures)
    {
        Console.WriteLine($"  [REJECTION TRACE] {failure}");
    }
}
Console.WriteLine("========================================");

Console.WriteLine("\n--- Starting Modular Audit Path: Delegates & Lambdas ---");

var auditService = new EnrollmentService();

// TODO 4: Create a lambda function that catches a student object and executes custom formatting
Action<Student> smsNotifier = s => Console.WriteLine($"  [SMS SENT]: Welcome to the TMS, {s.Name}!");

// Additional demonstration: We can bind multiple separate side-effects onto a single delegate hook
Action<Student> leaderboardUpdater = s => Console.WriteLine($"  [LEADERBOARD UPDATER]: Re-ranking campus charts for {s.Name}...");


// TODO 5: Attach that lambda to the EnrollmentService and call ProcessRegistration (which triggers Finalize)
// We use the '+=' operator to attach multiple standalone actions onto a single Multicast Delegate.
auditService.OnEnrollmentCompleted += smsNotifier;
auditService.OnEnrollmentCompleted += leaderboardUpdater;


// Set up a mock student and course to verify execution context behavior
var auditStudent = new Student { Id = "S100", Name = "Almaz", Age = 21, GPA = 3.9m };
var auditCourse = new Course { Code = "CS-500", Title = "Cloud Native C#", Capacity = 10 };

Console.WriteLine($"Registering student '{auditStudent.Name}' to trigger modular actions:");

// Execute registration pipeline
auditService.ProcessRegistration(auditStudent, auditCourse);