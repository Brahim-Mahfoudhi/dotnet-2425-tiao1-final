using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;


public class DateInThePastAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is DateTime dateTime)
        {
            // Check if the date is less than or equal to today
            return dateTime <= DateTime.Today;
        }

        return false;
    }
}


public class BelgianPhoneNumberAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string phoneNumber)
        {
            // Define regular expression for Belgian phone numbers
            var belgianPhoneNumberPattern = @"^(\+32\s?|0)(4[5-9]\s?\d{2}|\d{2})\s?\d{3}\s?\d{3}$";

            // Check if the phone number matches the pattern
            if (Regex.IsMatch(phoneNumber, belgianPhoneNumberPattern))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Invalid Belgian phone number. Must be in the format +32 XXX XXX XXX or 0XXX XXX XXX.");
            }
        }

        return new ValidationResult("Invalid phone number format.");
    }
}
public class NotNullOrEmptyAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return new ValidationResult(ErrorMessage ?? "The field is required and cannot be empty or whitespace.");
            }
        }
        else if (value == null)
        {
            return new ValidationResult(ErrorMessage ?? "The field is required.");
        }

        return ValidationResult.Success;
    }
}