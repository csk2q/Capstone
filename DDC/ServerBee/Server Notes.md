# General Notes
Pages are rendered server side in a hierarchical manner.

# Important files & folders:

### wwwroot (folder)
This folder serves the static resources for the website. Eg: css, js, bootstrap, favion, etc.

### Components (Folder)
This is the root folder for the asp.net/Blazor part of the website.

### Pages (Folder)
 Contains pages or partial pages

### _Imports.razor
This file is first considered before processing any file in the same folder.
You could think of it as the shared top of all pages in the folder.  
Its primary function is for C# using statements (like python import keyword) and for setting the layout.
Check here if you find a layout is being applied that you did not set.

### App.razor
This is the heart of the website and contains DOCTYPE tag, \<head/\>, and \<body/\> element that will be shared for ALL pages of the website.   
To add elements to the <head> use <HeadContent> element in the page. For more information see [Control \<head> content in ASP.NET Core Blazor apps](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/control-head-content?view=aspnetcore-9.0).


### Routes.razor
This file is the primary element in the \<body/\> element of App.razor. Its primary function is to handle routing to pages (since a .razor file can define its address via @page "url"). This file also defines the default layout and where to navigate users when they try to access a page they are not authorized to do so.

### MainLayout.razor
This is the default layout as defined in Routes.razor and wraps around the body element (@body) from App.razor. This is the file that adds both the sidebar and the top-row. Though, note that the sidebar contents are defined in NavMenu.razor.

#### MainLayout.razor.css
All files in the format FileName.razor.css contain css that ONLY applies to the contents of FileName.razor without effecting any components or elements higher up or lower down the the hierarchy. For more information see [ASP.NET Core Blazor CSS isolation](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/css-isolation?view=aspnetcore-9.0).



# File chain/hierarchy for login/authentication


# File -> URL list
This list is likely out of date or incomplete.
Command to generate list: `grep -rni "@page " *`   
|URL|File|
|---|---|
|/Account/AccessDenied|DDC/Blazor/Components/Account/Pages/AccessDenied.razor|
|/Account/ConfirmEmail|DDC/Blazor/Components/Account/Pages/ConfirmEmail.razor|
|/Account/ConfirmEmailChange|DDC/Blazor/Components/Account/Pages/ConfirmEmailChange.razor|
|/Account/ExternalLogin|DDC/Blazor/Components/Account/Pages/ExternalLogin.razor|
|/Account/ForgotPassword|DDC/Blazor/Components/Account/Pages/ForgotPassword.razor|
|/Account/ForgotPasswordConfirmation|DDC/Blazor/Components/Account/Pages/ForgotPasswordConfirmation.razor|
|/Account/InvalidPasswordReset|DDC/Blazor/Components/Account/Pages/InvalidPasswordReset.razor|
|/Account/InvalidUser|DDC/Blazor/Components/Account/Pages/InvalidUser.razor|
|/Account/Lockout|DDC/Blazor/Components/Account/Pages/Lockout.razor|
|/Account/Login|DDC/Blazor/Components/Account/Pages/Login.razor|
|/Account/LoginWith2fa|DDC/Blazor/Components/Account/Pages/LoginWith2fa.razor|
|/Account/LoginWithRecoveryCode|DDC/Blazor/Components/Account/Pages/LoginWithRecoveryCode.razor|
|/Account/Manage/ChangePassword|DDC/Blazor/Components/Account/Pages/Manage/ChangePassword.razor|
|/Account/Manage/DeletePersonalData|DDC/Blazor/Components/Account/Pages/Manage/DeletePersonalData.razor|
|/Account/Manage/Disable2fa|DDC/Blazor/Components/Account/Pages/Manage/Disable2fa.razor|
|/Account/Manage/Email|DDC/Blazor/Components/Account/Pages/Manage/Email.razor|
|/Account/Manage/EnableAuthenticator|DDC/Blazor/Components/Account/Pages/Manage/EnableAuthenticator.razor|
|/Account/Manage/ExternalLogins|DDC/Blazor/Components/Account/Pages/Manage/ExternalLogins.razor|
|/Account/Manage/GenerateRecoveryCodes|DDC/Blazor/Components/Account/Pages/Manage/GenerateRecoveryCodes.razor|
|/Account/Manage|DDC/Blazor/Components/Account/Pages/Manage/Index.razor|
|/Account/Manage/PersonalData|DDC/Blazor/Components/Account/Pages/Manage/PersonalData.razor|
|/Account/Manage/ResetAuthenticator|DDC/Blazor/Components/Account/Pages/Manage/ResetAuthenticator.razor|
|/Account/Manage/SetPassword|DDC/Blazor/Components/Account/Pages/Manage/SetPassword.razor|
|/Account/Manage/TwoFactorAuthentication|DDC/Blazor/Components/Account/Pages/Manage/TwoFactorAuthentication.razor|
|/Account/Register|DDC/Blazor/Components/Account/Pages/Register.razor|
|/Account/RegisterConfirmation|DDC/Blazor/Components/Account/Pages/RegisterConfirmation.razor|
|/Account/ResendEmailConfirmation|DDC/Blazor/Components/Account/Pages/ResendEmailConfirmation.razor|
|/Account/ResetPassword|DDC/Blazor/Components/Account/Pages/ResetPassword.razor|
|/Account/ResetPasswordConfirmation|DDC/Blazor/Components/Account/Pages/ResetPasswordConfirmation.razor|
|/auth|DDC/Blazor/Components/Pages/Auth.razor|
|/counter|DDC/Blazor/Components/Pages/Counter.razor|
|/emptyPage|DDC/Blazor/Components/Pages/EmptyPage.razor|
|/Error|DDC/Blazor/Components/Pages/Error.razor|
|/|DDC/Blazor/Components/Pages/Home.razor|
|/weather|DDC/Blazor/Components/Pages/Weather.razor|












