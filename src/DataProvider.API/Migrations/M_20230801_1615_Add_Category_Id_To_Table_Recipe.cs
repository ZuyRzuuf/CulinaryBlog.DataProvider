using FluentMigrator;

namespace DataProvider.API.Migrations;

[Migration(202308011615)]
public class M_20230801_1615_Add_Category_Id_To_Table_Recipe : Migration
{
    public override void Up()
    {
        Alter.Table("recipe")
            .AddColumn("category_id")
            .AsGuid()
            .Nullable();
    }

    public override void Down()
    {
        Delete.Column("category_id").FromTable("recipe");
    }
}