namespace Backend.Src.Domain.ValueObjects.Jobs;

public enum JobStatus
{
    Scheduled, //Programed to be seen in X time
    Active,    //Currently visible and users can apply to it
    Closed     //Not visible for the employees
}