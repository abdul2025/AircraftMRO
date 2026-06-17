#### RUN the MVC 
-- dotnet watch run
#### migrations and update DB
-- dotnet ef migrations add XXXXX
-- dotnet ef database update


##### User Secrets
-- dotnet user-secrets init
-- dotnet user-secrets list
-- dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=xx;Port=xx;Database=xx;Username=xx;Password=xx"