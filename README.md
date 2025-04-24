# The Smart Farm

## Project Overview
The Smart Farm is an IoT-based project that integrates **ASP.NET Core (MVC)** and **MySQL** with **Adafruit IO** to monitor and control farm conditions in real-time.

## Prerequisites
Ensure you have the following installed before setting up the project:
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [MySQL Server](https://dev.mysql.com/downloads/)
- [Git](https://git-scm.com/downloads)
- [Visual Studio](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [Postman](https://www.postman.com/) (optional, for API testing)

## Installation & Setup

### 1. Clone the Repository
```sh
git clone https://github.com/kansu4/The-Smart-Farm.git
cd The-Smart-Farm
```

### 2. Install Dependencies
```sh
dotnet restore
```

### 3. Install Required NuGet Packages
```sh
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 8.0.10
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.10
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.10
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.10
dotnet add package MySqlConnector.DependencyInjection --version 2.3.6
dotnet add package Pomelo.EntityFrameworkCore.MySql --version 8.0.0
dotnet add package MySql.Data --version 9.3.0
dotner add package DotNetEnv --version 3.1.1
```

### 4. Apply Migrations & Update Database ( only when apply changes to Model)
```sh
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 5. Run the Project
```sh
dotnet run
```
Then, open `http://localhost:5130/` in your browser.

##OPEN VISUAL CODE, ENTER THIS KEY: aio_GRns9174sgEdANWTkgIv9j3zUCNL

## Technologies Used
- **Backend:** ASP.NET Core MVC
- **Database:** MySQL (Pomelo.EntityFrameworkCore.MySql)
- **IoT:** Adafruit IO (REST API)
- **Frontend:** Razor Views (HTML, CSS, JS)

