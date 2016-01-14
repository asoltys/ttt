namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoveEventTextToEdit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Edits", "TextE", c => c.String());
            AddColumn("dbo.Edits", "TextF", c => c.String());
            AddColumn("dbo.Edits", "HoverE", c => c.String());
            AddColumn("dbo.Edits", "HoverF", c => c.String());
            DropColumn("dbo.Events", "TextE");
            DropColumn("dbo.Events", "TextF");
            DropColumn("dbo.Events", "HoverE");
            DropColumn("dbo.Events", "HoverF");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Events", "HoverF", c => c.String());
            AddColumn("dbo.Events", "HoverE", c => c.String());
            AddColumn("dbo.Events", "TextF", c => c.String());
            AddColumn("dbo.Events", "TextE", c => c.String());
            DropColumn("dbo.Edits", "HoverF");
            DropColumn("dbo.Edits", "HoverE");
            DropColumn("dbo.Edits", "TextF");
            DropColumn("dbo.Edits", "TextE");
        }
    }
}
