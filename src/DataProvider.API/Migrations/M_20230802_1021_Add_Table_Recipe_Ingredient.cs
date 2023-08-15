using FluentMigrator;

namespace DataProvider.API.Migrations;

[Migration(202308021021)]
public class M_20230802_1021_Add_Table_Recipe_Ingredient : Migration
{
    public override void Up()
    {
        Create.Table("recipe_ingredient")
            .WithColumn("id")
                .AsGuid()
                .NotNullable()
                .PrimaryKey()
            .WithColumn("recipe_id")
                .AsGuid()
                .NotNullable()
                .ForeignKey("recipe", "id")
            .WithColumn("name")
                .AsString(32)
                .NotNullable()
            .WithColumn("description")
                .AsString(255)
                .Nullable()
            .WithColumn("quantity")
                .AsInt32()
                .NotNullable()
            .WithColumn("quantity_type")
                .AsString(32)
                .NotNullable();
    }

    public override void Down()
    {
        Delete.Table("recipe_ingredient");
    }
}