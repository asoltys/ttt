namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EventDateAndTypeToEdit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Edits", "Type", c => c.Int(nullable: true));
            AddColumn("dbo.Edits", "DisplayDate", c => c.DateTime(nullable: true));
            DropColumn("dbo.Events", "Type");
            DropColumn("dbo.Events", "Date");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Events", "Date", c => c.DateTime(nullable: true));
            AddColumn("dbo.Events", "Type", c => c.Int(nullable: true));
            DropColumn("dbo.Edits", "DisplayDate");
            DropColumn("dbo.Edits", "Type");
        }
    }
}
