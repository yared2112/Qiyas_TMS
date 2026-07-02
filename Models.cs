namespace TmsCore;

// From Part 1
public record EnrollmentRecord(string StudentId, string CourseCode, DateTime EnrolledAt);

// Part 2: Mutable Entity with Modern Auto-Property Validation
public class Course
{
    public required string Code { get; init; }

    public required string Title
    {
        get;
        set => field = !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new ArgumentException("Title cannot be empty or whitespace.", nameof(value));
    }

    // Auto-property validation using the modern 'field' keyword
    public int Capacity
    {
        get;
        set => field = value > 0
            ? value
            : throw new ArgumentOutOfRangeException(nameof(value), "System constraint: Capacity must be greater than zero.");
    }

    public int EnrolledCount { get; set; }
}

public class Student
{
    public required string Id { get; init; }
    public required string Name
    {
        get;
        set => field = !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new ArgumentException("Name cannot be empty or whitespace.", nameof(value));
    }
    public int Age
    {
        get;
        set => field = value is >= 16 and <= 100
            ? value
            : throw new ArgumentOutOfRangeException(nameof(value), "Age must be between 16 and 100.");
    }
    public decimal GPA
    {
        get;
        set => field = value is >= 0.0m and <= 4.0m
            ? value : throw new ArgumentOutOfRangeException(nameof(value), "GPA must be between 0.0 and 4.0.");
    }

}

public interface IGradable
{
    string Title { get; }
    decimal CalculateGrade();
}
public class Quiz : IGradable
{
    public required string Title { get; init; }
    public required int CorrectAnswers { get; init; }
    public required int TotalQuestions { get; init; }
    public decimal CalculateGrade()
    {
        if (TotalQuestions == 0) return 0m;
        return (decimal)CorrectAnswers / TotalQuestions * 100m;
    }
}
public class LabAssignment : IGradable
{
    public required string Title { get; init; }
    public required decimal FunctionalityScore { get; init; }
    public required decimal CodeQualityScore { get; init; }
    public decimal CalculateGrade()
    {
        // 70% functionality, 30% code quality
        return (FunctionalityScore * 0.7m) + (CodeQualityScore * 0.3m);
    }
}

/// Thrown when database operations fail, capturing technical context for operators.
/// </summary>
public class TmsDatabaseException : Exception
{
    public string Operation { get; }

    public TmsDatabaseException(string operation, string message) : base(message)
    {
        Operation = operation;
    }

    public TmsDatabaseException(string operation, string message, Exception innerException)
        : base(message, innerException)
    {
        Operation = operation;
    }
}

/// <summary>
/// Thrown when a student attempts to enroll in a course that has zero remaining seats.
/// </summary>
public class CapacityReachedException : InvalidOperationException
{
    public string CourseCode { get; }

    public CapacityReachedException(string courseCode)
        : base($"Course {courseCode} has reached maximum capacity.")
    {
        CourseCode = courseCode;
    }

    public CapacityReachedException(string courseCode, Exception innerException)
        : base($"Course {courseCode} has reached maximum capacity.", innerException)
    {
        CourseCode = courseCode;
    }
}
