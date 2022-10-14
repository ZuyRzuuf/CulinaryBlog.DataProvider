using FluentMigrator;

namespace RecipesDataProvider.Migrations;

[Migration(202210111943)]
public class M_20221011_1943_Add_Table_Recipe : Migration
{
    public override void Up()
    {
        Create.Table("recipe")
            .WithColumn("uuid")
                .AsGuid()
                .NotNullable()
                .PrimaryKey()
            .WithColumn("title")
                .AsString(255)
                .NotNullable();
    }

    public override void Down()
    {
        Delete.Table("recipe");
    }
}