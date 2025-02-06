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














