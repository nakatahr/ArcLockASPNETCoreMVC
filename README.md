# Introduction
This project demonstrates one of the implementation of a web app for using smart lock by a large number of people. 

"SwitchBot Lock" (https://www.switchbot.jp/products/switchbot-lock) is used for a smart lock device which provides nice and rich API access for IoT hobbyists (https://github.com/OpenWonderLabs/SwitchBotAPI).

ASP.NET Core MVC is used for a framework of the app and it can be hosted to your preferred cloud service such Azure Web Apps.

# Demo
First check the demo shown below that tells what the web app does.

(click ▶ to play)

![lockmovie](https://user-images.githubusercontent.com/24380329/235057520-2f8c1414-9777-4851-8228-19f7ef1f4cae.gif)

# Problem to be solved
SwitchBot is basically controlled by iOS and Android app, and they work perfectly in most of the cases. However, as the product is mainly targeted for home use, sharing devices with a large number of people is not considered well. As there is a feature called "Home Sharing" in SwitchBot, you can easily invite members you want to grant access to a device like SwitchBot Lock. But if you are going to use in SOHO or small enterprise like 10-50 people, it is time-consuming for both administrator and user to insall dedicated app just only to unlock a door.

<kbd><img src="https://user-images.githubusercontent.com/24380329/235060967-609ee05b-7862-40d1-bb44-12534f61e0fa.jpg" width="200"></kbd>

# Add AzureAD authentication
ASP.NET Core provides the way to integrate AzureAD to a web application that enables users to sign in using OAuth 2.0 with credentials from Azure Active Directory. Please check [Azure Active Directory with ASP.NET Core](https://learn.microsoft.com/ja-jp/aspnet/core/security/authentication/azure-active-directory/?view=aspnetcore-7.0) to learn more. Using AzureAD for authentication method and by adding [Authorize] prefix to controller of pages you want to control, those who in an organization can access to the company web page for office unlock. 

<kbd><img src="https://user-images.githubusercontent.com/24380329/235066801-02f558d6-34ae-43df-aff3-f5f426573cbb.png" width="200"></kbd>
<kbd><img src="https://user-images.githubusercontent.com/24380329/235062515-bec1b0a7-a062-4903-87aa-2cc4fb5c4315.png" width="200"></kbd>
