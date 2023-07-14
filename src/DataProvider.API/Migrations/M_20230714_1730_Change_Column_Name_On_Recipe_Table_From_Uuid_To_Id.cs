using FluentMigrator;

namespace DataProvider.API.Migrations;

[Migration(202307141730)]
public class M_20230714_1730_Change_Column_Name_On_Recipe_Table_From_Uuid_To_Id : Migration
{
    public override void Up()
    {
        Rename.Column("uuid")
            .OnTable("recipe")
            .To("id");
    }

    public override void Down()
    {
        Rename.Column("id")
            .OnTable("recipe")
            .To("uuid");
    }
}