using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;


public class MinimumAgeAttribute : ValidationAttribute
{
    private readonly int _minimumAge;

    public MinimumAgeAttribute(int minimumAge)
    {
        _minimumAge = minimumAge;
    }

    public override bool IsValid(object? value)
    {
        if (value is DateTime dateTime)
        {
            // Calculate the age based on the provided date
            var today = DateTime.Today;
            var age = today.Year - dateTime.Year;

            // Adjust if the birthday hasn't occurred yet this year
            if (dateTime.Date > today.AddYears(-age))
            {
                age--;
            }

            // Check if the age meets the minimum requirement
            return age >= _minimumAge;
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

public class ComparePasswordAttribute : ValidationAttribute
{
    private readonly string _comparisonProperty;

    public ComparePasswordAttribute(string comparisonProperty)
    {
        _comparisonProperty = comparisonProperty;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var currentValue = value?.ToString();
        var comparisonProperty = validationContext.ObjectType.GetProperty(_comparisonProperty);

        if (comparisonProperty == null)
            throw new ArgumentException("Property with this name not found");

        var comparisonValue = comparisonProperty.GetValue(validationContext.ObjectInstance)?.ToString();

        if (currentValue != comparisonValue)
        {
            return new ValidationResult(ErrorMessage ?? "The passwords do not match.");
        }

        return ValidationResult.Success;
    }
}