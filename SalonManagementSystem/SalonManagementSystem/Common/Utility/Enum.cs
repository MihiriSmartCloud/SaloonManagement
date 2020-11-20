namespace SalonAPI.Enum
{

    #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public enum ActionType
    {
        INSERT,
        UPDATE,
        DELETE,
        LOGIN
    }

    public enum AppointmentStatus
    {
        TO_DO,
        IN_PROGRESS,  
        COMPLETED,
        CANCELED
    }


}