using System;

namespace TmsCore;

public class EnrollmentService
{
    // TODO 1 & 2: Define a delegate listener property using Action<T>
    // The '?' denotes that it is nullable (it won't throw if no one is listening)
    public Action<Student>? OnEnrollmentCompleted { get; set; }

    public EnrollmentRecord ProcessRegistration(Student? student, Course? course)
    {
        // Prior Guard Clauses and validations remain untouched here
        if (student is null) throw new ArgumentNullException(nameof(student));
        if (course is null) throw new ArgumentNullException(nameof(course));
        if (course.EnrolledCount >= course.Capacity) throw new CapacityReachedException(course.Code);

        string standing = student.GPA switch
        {
            >= 3.5m => "Honors",
            >= 2.5m => "Good Standing",
            _ => "Academic Warning"
        };
        Console.WriteLine($"  {student.Name} is in {standing}.");

        // Execute persistence and trigger the delegate notification pipeline
        FinalizeEnrollment(student);

        return new EnrollmentRecord(student.Id, course.Code, DateTime.UtcNow);
    }

    public void FinalizeEnrollment(Student s)
    {
        Console.WriteLine("  Persisting record to database...");

        // TODO 3: Check if the delegate listener is 'not null' and invoke it with the student object.
        // We use the modern C# Null-conditional operator (?.) to cleanly handle null safety.
        OnEnrollmentCompleted?.Invoke(s);
    }
}
