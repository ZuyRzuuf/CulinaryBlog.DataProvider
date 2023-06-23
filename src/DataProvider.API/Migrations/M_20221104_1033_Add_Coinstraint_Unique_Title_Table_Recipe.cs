using FluentMigrator;

namespace DataProvider.API.Migrations;

[Migration(202211041033)]
public class M_20221104_1033_Add_Coinstraint_Unique_Title_Table_Recipe : Migration
{
    public override void Up()
    {
        Create.UniqueConstraint("title")
            .OnTable("recipe")
            .Column("title");
    }

    public override void Down()
    {
        Delete.UniqueConstraint("title")
            .FromTable("recipe")
            .Column("title");
    }
}