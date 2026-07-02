namespace TmsCore;

public class EnrollmentService
{
    public EnrollmentRecord ProcessRegistration(Student? student, Course? course)
    {
        // TODO 1: Guard clauses - (Fail Fast )
        if (student is null)
            throw new ArgumentNullException(nameof(student));

        if (course is null)
            throw new ArgumentNullException(nameof(course));

        if (course.Capacity <= 0)
            throw new InvalidOperationException("Course capacity must be greater than zero.");

        if (course.EnrolledCount >= course.Capacity)
            throw new InvalidOperationException("Course is full. Cannot process enrollment.");

        // TODO 2: Switch expression - (Pattern Matching)
        string standing = student.GPA switch
        {
            >= 3.5m => "Honors",
            >= 2.5m => "Good Standing",
            _ => "Academic Warning" // (Discard Pattern)
        };

        Console.WriteLine($"{student.Name} is in {standing}.");

        // TODO 3: EnrollmentRecord
        return new EnrollmentRecord(student.Id, course.Code, DateTime.UtcNow);
    }
}
