namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BilingualEvent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "TextE", c => c.String());
            AddColumn("dbo.Events", "TextF", c => c.String());
            AddColumn("dbo.Events", "HoverE", c => c.String());
            AddColumn("dbo.Events", "HoverF", c => c.String());
            DropColumn("dbo.Events", "Text");
            DropColumn("dbo.Events", "Hover");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Events", "Hover", c => c.String());
            AddColumn("dbo.Events", "Text", c => c.String());
            DropColumn("dbo.Events", "HoverF");
            DropColumn("dbo.Events", "HoverE");
            DropColumn("dbo.Events", "TextF");
            DropColumn("dbo.Events", "TextE");
        }
    }
}
