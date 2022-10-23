using System.Text.RegularExpressions;

namespace RecipesDataProvider.Infrastructure.Helpers;

public static class ConnectionStringValidator
{
    public static bool Validate(string connectionString)
    {
        const string connectionStringPattern = 
            @"server=([a-z]{3,}(\/[a-z]{0,}){0,}|([1-9][0-9]{1,2}\.[0-9]{1,2}\.[0-9]{1,2}\.[1-9][0-9]{0,2}(\/[a-z]{0,}){0,}));port=[1-9][0-9]{3};userid=[a-zA-Z0-9_-]{1,};password=.{1,}";
        // const string connectionStringPattern = 
        //     @"server=([a-z]{3,}(\/[a-z]{0,}){0,}|([1-9][0-9]{1,2}\.[0-9]{1,2}\.[0-9]{1,2}\.[1-9][0-9]{0,2}(\/[a-z]{0,}){0,}));port=[1-9][0-9]{3};userid=[a-zA-Z0-9_-]{1,};password=.{1,};database=[a-z_-]{3,}";
        var regex = new Regex(connectionStringPattern);

        return regex.IsMatch(connectionString);
    }
}