// using System;
// using Xunit.Abstractions;
// using Shouldly;
// using Rise.Shared.Users;
// using Rise.Shared.Enums;

// namespace Rise.Client.Users;

// public class RegistrationShould : TestContext
// {
//     public RegistrationShould(ITestOutputHelper outputHelper)
//     {
//         Services.AddXunitLogger(outputHelper);
//         Services.AddScoped<IUserService, FakeUserService>();
//     }

//     [Fact]
//     public void RegisterPageRendersCorrectly()
//     {
//         // Render the Register component
//         var cut = RenderComponent<Register>();

//         // Assert: form fields are rendered
//         cut.Find("input#FirstName").ShouldNotBeNull();
//         cut.Find("input#LastName").ShouldNotBeNull();
//         cut.Find("input#Email").ShouldNotBeNull();
//         cut.Find("input#PhoneNumber").ShouldNotBeNull();
//         cut.Find("input#Password").ShouldNotBeNull();
//         cut.Find("input#ConfirmPassword").ShouldNotBeNull();
//         cut.Find("input#BirthDate").ShouldNotBeNull();
//         cut.Find("select#Street").ShouldNotBeNull();
//         cut.Find("input#HouseNumber").ShouldNotBeNull();
//         cut.Find("input#Bus").ShouldNotBeNull();
//     }

//   [Fact]
//     public void SubmitForm_WithEmptyFields_ShowsValidationErrors()
//     {
//         // Render the Register component
//         var cut = RenderComponent<Register>();

//         // Act: Try to submit the form without filling any data
//         cut.Find("button[type='submit']").Click();

//         // Assert: Validation errors should be displayed
//         cut.FindAll("div.validation-message").Count.ShouldBeGreaterThan(0); // There should be validation errors
//     }

//     [Fact]
//     public void SubmittingValidForm_ShouldCallCreateUserService()
//     {
//         // Arrange
//         var user = new UserDto.CreateUser
//         {
//             FirstName = "John",
//             LastName = "Doe",
//             Email = "john.doe@example.com",
//             Password = "Password123",
//             BirthDate = DateTime.Now.AddYears(-20), // Valid birthdate
//             PhoneNumber = "+32475234567",
//             Address = new AddressDto.CreateAddress
//             {
//                 Street = StreetEnum.AFRIKALAAN,
//                 HouseNumber = "15",
//                 Bus = "B"
//             }
//         };

//         _mockUserService.Setup(x => x.CreateUserAsync(It.IsAny<UserDto.CreateUser>()))
//                         .ReturnsAsync(true);

//         // Render the Register component
//         var cut = RenderComponent<Register>();

//         // Act: Fill in the form with valid data
//         cut.Find("input#FirstName").Change(user.FirstName);
//         cut.Find("input#LastName").Change(user.LastName);
//         cut.Find("input#Email").Change(user.Email);
//         cut.Find("input#PhoneNumber").Change(user.PhoneNumber);
//         cut.Find("input#Password").Change(user.Password);
//         cut.Find("input#ConfirmPassword").Change(user.Password);
//         cut.Find("input#BirthDate").Change(user.BirthDate.ToString("yyyy-MM-dd"));
//         cut.Find("input#HouseNumber").Change(user.Address.HouseNumber);
//         cut.Find("input#Bus").Change(user.Address.Bus);
//         cut.Find("select#Street").Change(user.Address.Street.ToString());

//         // Submit the form
//         cut.Find("button[type='submit']").Click();

//         // Assert: Ensure CreateUserAsync is called with the correct user details
//         _mockUserService.Verify(x => x.CreateUserAsync(It.Is<UserDto.CreateUser>(u => u.Email == user.Email)), Times.Once);
//     }
// }
