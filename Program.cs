// // string? region = null;
// // string? upperRegion = region?.ToUpper();
// // Console.WriteLine($"Region (conditional): {upperRegion}");
// // string displayRegion = region ?? "Unassigned";
// // Console.WriteLine($"Region (coalesced): {displayRegion}");
// // region ??= "Addis Ababa";
// // Console.WriteLine($"Region (assigned): {region}");

// string studentName = "Abeba";
// string studentId = "STU-001";
// int enrollmentCount = 3;
// decimal grantAmount = 1999.99m; // 'm' suffix marks a decimal literal
// DateTime enrolledAt = DateTime.UtcNow;
// string? campusRegion = null;
// Console.WriteLine($"Student: {studentName} ({studentId})");
// Console.WriteLine($"Courses: {enrollmentCount}");
// Console.WriteLine($"Grant amount: {grantAmount:F2}");
// Console.WriteLine($"Enrolled at: {enrolledAt:yyyy-MM-dd HH:mm:ss} UTC");
// Console.WriteLine($"Campus region: {campusRegion ?? "Unassigned"}");

// decimal grantPerStudent = 1999.99m;
// decimal totalAllocation = grantPerStudent * 100_000;
// Console.WriteLine($"Total allocated (decimal): {totalAllocation}");
// Console.WriteLine($"Total allocated (formatted): {totalAllocation:F2}");

using TmsCore;

// Create the original record
var enrollment = new EnrollmentRecord("STU-001", "CS-401", DateTime.UtcNow);
Console.WriteLine(enrollment);

// --- COMPILER PROTECTION TEST ---
// If you uncomment the line below, the compiler will refuse to build the project.
// enrollment.CourseCode = "HACKED"; // ERROR: Init-only property cannot be assigned here.
// ---------------------------------

// Non-destructive mutation: Creates a BRAND NEW instance with one modified value
var corrected = enrollment with { CourseCode = "CS-402" };
Console.WriteLine(corrected);

// Value-based Equality: Compares the actual data values, not the memory addresses
var duplicate = new EnrollmentRecord("STU-001", "CS-401", enrollment.EnrolledAt);
Console.WriteLine($"Same data? {enrollment == duplicate}"); // Expected: True

// 1. Initialize a valid course
var course = new Course { Code = "CS-401", Title = "Advanced C#", Capacity = 30 };
Console.WriteLine($"Course Created: {course.Title} (Capacity: {course.Capacity})");

// 2. Test Invalid Capacity Validation
try
{
    Console.WriteLine("Attempting to set invalid capacity (-5)...");
    course.Capacity = -5; // This should be intercepted by the setter validation
}
catch (ArgumentOutOfRangeException ex)
{
    Console.WriteLine($"[SUCCESSFULLY BLOCKED] Caught: {ex.Message}");
}

// 3. Test Invalid Title Validation
try
{
    Console.WriteLine("Attempting to set an empty title...");
    course.Title = ""; // This should be intercepted by the setter validation
}
catch (ArgumentException ex)
{
    Console.WriteLine($"[SUCCESSFULLY BLOCKED] Caught: {ex.Message}");
}

var s = new Student { Id = "S1", Name = "Abeba", Age = 20, GPA = 3.8m };
Console.WriteLine($"Student: {s.Name}, GPA: {s.GPA}");

IGradable[] cohortAssessments = [
    new Quiz { Title = "C# Basics", CorrectAnswers = 18, TotalQuestions = 20 },
    new LabAssignment { Title = "Registration API", FunctionalityScore = 90m, CodeQualityScore = 85m }
];

PrintGradeReport(cohortAssessments);

// Step 3 — Write the Polymorphic Report
void PrintGradeReport(IEnumerable<IGradable> assessments)
{
    Console.WriteLine("--- Grade Report ---");
    foreach (var item in assessments)
    {
        // The runtime automatically resolves which implementation of CalculateGrade() to invoke
        Console.WriteLine($"{item.Title}: {item.CalculateGrade():F2}%");
    }
}