using FluentMigrator;

namespace DataProvider.API.Migrations;

[Migration(202308011619)]
public class M_20230801_1619_Add_Foreign_Key_Category_Id_To_Table_Recipe : Migration
{
    public override void Up()
    {
        Create.ForeignKey()
            .FromTable("recipe")
            .ForeignColumn("category_id")
            .ToTable("category")
            .PrimaryColumn("id")
            .OnDelete(System.Data.Rule.None); // Change to Cascade if you want to delete recipes when their category is deleted
    }

    public override void Down()
    {
        Delete.ForeignKey().FromTable("recipe").ForeignColumn("category_id");
    }
}