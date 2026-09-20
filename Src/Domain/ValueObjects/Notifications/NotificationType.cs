namespace Backend.Src.Domain.ValueObjects.Notifications;

public enum NotificationType
{
    //Job related
    CandidateSelected,
    ApplicationAccepted,
    ApplicationRejected,
    NewJobPublished,
    //Message related
    MessageReceived,
    //Auth related
    VerificationCode,
    PasswordReset,
    //Payment related
    PaymentSuccess,
    PaymentFailure
}