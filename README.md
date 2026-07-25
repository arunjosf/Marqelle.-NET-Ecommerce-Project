# Marqelle - E-Commerce Backend API 

This is the official .NET Core Web API backend for the **Marqelle** E-Commerce platform. It handles everything from user authentication and cart management to secure payments and admin dashboard analytics.

**[Looking for the Frontend UI Repository? Click Here](https://github.com/arunjosf/Marqelle-Project-Front-end-React)** 

## Tech Stack
* **Framework:** ASP.NET Core 8 Web API
* **Language:** C#
* **Database:** SQL Server
* **ORM:** Entity Framework Core (EF Core)
* **Authentication:** JWT (JSON Web Tokens) with Refresh Tokens
* **Email Service:** MailKit / SMTP (OTP Verification)
* **Image Hosting:** Cloudinary
* **Payments:** Razorpay API

## Features
* **Authentication & Authorization:** Secure Login, Registration with Email OTP Verification, Password Resets, and Refresh Token rotation.
* **Product Management:** Full CRUD operations for products, categories, and sizes.
* **Shopping Experience:** Wishlist and Cart management saved to the database.
* **Checkout & Payments:** Secure payment gateway integration using Razorpay.
* **User Profiles:** Order history, profile updates, and address management.
* **Admin Dashboard:** Secure endpoints for managing users, tracking orders, adding products, and viewing sales analytics.

## Local Development Setup

1. Prerequisites
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
* SQL Server (LocalDB or full instance)
* Visual Studio 2022 / VS Code

2. Clone the Repository
```bash
git clone https://github.com/arunjosf/Marqelle.-NET-Ecommerce-Project
cd Marqelle.-NET-Ecommerce-Project
```
3. **Configure Database Connection:**
   Update your `appsettings.json` or `appsettings.Development.json` with your local SQL database connection string.

4. **Run Entity Framework Migrations:**
   ```bash
   dotnet ef database update
   ```

5. **Run the Server:**
   ```bash
   dotnet run
   ```
