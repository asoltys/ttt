namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EventWithRegionAndBranch : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "Branch_ID", c => c.Int());
            AddColumn("dbo.Events", "Region_ID", c => c.Int());
            CreateIndex("dbo.Events", "Branch_ID");
            CreateIndex("dbo.Events", "Region_ID");
            AddForeignKey("dbo.Events", "Branch_ID", "dbo.Branches", "ID");
            AddForeignKey("dbo.Events", "Region_ID", "dbo.Regions", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Events", "Region_ID", "dbo.Regions");
            DropForeignKey("dbo.Events", "Branch_ID", "dbo.Branches");
            DropIndex("dbo.Events", new[] { "Region_ID" });
            DropIndex("dbo.Events", new[] { "Branch_ID" });
            DropColumn("dbo.Events", "Region_ID");
            DropColumn("dbo.Events", "Branch_ID");
        }
    }
}
