window.addAutofillEvent = function (inputId, dotNetHelper) {
    const inputElement = document.getElementById(inputId);

    if (inputElement) {
        // Detect changes to the input field, such as autofill events
        inputElement.addEventListener('input', function () {
            dotNetHelper.invokeMethodAsync('UpdateValue', inputId, inputElement.value);
        });

        // For cases where the 'input' event might not capture autofill:
        setInterval(() => {
            dotNetHelper.invokeMethodAsync('UpdateValue', inputId, inputElement.value);
        }, 1000);
    }
};
window.promptAutofill = function () {
    // Focus on each input field to trigger the autofill prompt
    const fields = ['FirstName', 'LastName', 'Email', 'PhoneNumber', 'BirthDate'];
    fields.forEach(fieldId => {
        const input = document.getElementById(fieldId);
        if (input) {
            input.focus();
            input.blur(); // Focus and then blur to prompt autofill
        }
    });
};
