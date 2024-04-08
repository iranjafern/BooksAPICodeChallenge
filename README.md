How to use the Web API
----------------------

1.  I have developed the web api using .Net 8, entitiy framework core, SQL server database and Serilog for logging.
2.  This solution has 3 projects as below,
       * Books.API - The web API
       * Books.DataAccess - The entitiy framework core for the data access layer
       * Books.Test - The unit testings
3.  Unit testing is done nUnit & Moq
4.  I have used Okta for identity and access management. 
5.  If different applications need to access this web API, we can easily create separate users in Okta to access this web API. 
6.  As I have used Okta for IAM, it offers a lot of capabilities.
7.  I am accessing the GoogleBookAPI by creating a API-key.
8.  Please download the code from git or clone the repo.
9.  IMPORTANT *I have stored the Okta (ClientId, ClientSecret, Issuer), the Google API-Key and the DB connectionstring in the appsettings.Development.json file but
    I didn't check-in the key values to git, only kept placeholders for the values. I will include these key values in the email. Please copy the Okta (ClientId, ClientSecret, Issuer) 
    and the Google API-Key from the email and put in the placeholders in the appsettings.Development.json file. I will be deleting these keys after the assessment.* 
10. In a production application, I will store these keys and the DB connection string in Azure Key vault or locally in the secrets file.
11. To set up the database, in the appsettings.Development.json file -> connection string (BooksConnection) please change the data source to your DB server.
12. In the package manager consloe please select the Books.DataAccess project and run the below command,
                     EntityFrameworkCore\Update-Database
13. In the appsettings.Development.json file, if required change log path. 
14. You can run the application and invoke the APIs using swagger.
15. If you face any issues, please feel free to call/email me.
