namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MultipleRegionEvents : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Events", "RegionID", "dbo.Regions");
            DropIndex("dbo.Events", new[] { "RegionID" });
            AddColumn("dbo.Regions", "Event_ID", c => c.Int());
            CreateIndex("dbo.Regions", "Event_ID");
            AddForeignKey("dbo.Regions", "Event_ID", "dbo.Events", "ID");
            DropColumn("dbo.Events", "RegionID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Events", "RegionID", c => c.Int(nullable: false));
            DropForeignKey("dbo.Regions", "Event_ID", "dbo.Events");
            DropIndex("dbo.Regions", new[] { "Event_ID" });
            DropColumn("dbo.Regions", "Event_ID");
            CreateIndex("dbo.Events", "RegionID");
            AddForeignKey("dbo.Events", "RegionID", "dbo.Regions", "ID", cascadeDelete: true);
        }
    }
}
