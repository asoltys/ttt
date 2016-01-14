namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StatusFromEditToEvent_EditPushlishedProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.Edits", "Published", c => c.Boolean(nullable: false));
            DropColumn("dbo.Edits", "Status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Edits", "Status", c => c.Int(nullable: false));
            DropColumn("dbo.Edits", "Published");
            DropColumn("dbo.Events", "Status");
        }
    }
}
