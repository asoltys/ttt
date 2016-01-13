namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PendingEventInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "PendingDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Events", "PendingTextE", c => c.String());
            AddColumn("dbo.Events", "PendingTextF", c => c.String());
            AddColumn("dbo.Events", "PendingHoverE", c => c.String());
            AddColumn("dbo.Events", "PendingHoverF", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Events", "PendingHoverF");
            DropColumn("dbo.Events", "PendingHoverE");
            DropColumn("dbo.Events", "PendingTextF");
            DropColumn("dbo.Events", "PendingTextE");
            DropColumn("dbo.Events", "PendingDate");
        }
    }
}
