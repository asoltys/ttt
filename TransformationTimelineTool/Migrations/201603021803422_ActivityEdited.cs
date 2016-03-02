namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ActivityEdited : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Edits", "Edited", c => c.Boolean(nullable: true, defaultValue: false));
            Sql("UPDATE [dbo].[Edits] SET Edited='false' WHERE Edited IS NULL");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Edits", "Edited");
        }
    }
}
