# The_fantastic_four
Repository for an employee dashboard and sheduling system for the 2024 Monkey and river hackathon
##To get the Db successfuuly connected to your SQL:
Ensure you have Microsoft.EntityFrameworkCore, Microsoft.EntityFramework.Tools, Microsoft.EntityFramework.Design and Microsoft.EntityFramework.SqlServer installed
Here are their installation commands for the package manager console
Install-Package Microsoft.EntityFrameworkCore
Install-Package Microsoft.EntityFrameworkCore.Tools
Install-Package Microsoft.EntityFrameworkCore.Design
Install-Package Microsoft.EntityFrameworkCore.SqlServer

Without these commands, you will encounter issues of "Add-migration" does not exist in the cmdlet.
Once installed, run the folloowing commands:

Add-migration
Give the migration any name you find suitable it does not matter.
Run SQL Server and ensure your server is connected
Then run this command:
Update-database
This will create the database in your SQL Server

##In the Appsettings file, the server is represented by a "." this should directly call your SQL server but if it causes a problem, replace the "." with your SQL Server name