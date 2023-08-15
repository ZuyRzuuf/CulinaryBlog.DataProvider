using FluentMigrator;

namespace DataProvider.API.Migrations;

[Migration(202308011557)]
public class M_20230801_1557_Add_Table_Category : Migration
{
    public override void Up()
    {
        Create.Table("category")
            .WithColumn("id")
                .AsGuid()
                .NotNullable()
                .PrimaryKey()
            .WithColumn("name")
                .AsString(255)
                .NotNullable();
    }

    public override void Down()
    {
        Delete.Table("category");
    }
}