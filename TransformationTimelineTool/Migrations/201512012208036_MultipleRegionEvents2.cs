namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MultipleRegionEvents2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Regions", "Event_ID", "dbo.Events");
            DropIndex("dbo.Regions", new[] { "Event_ID" });
            CreateTable(
                "dbo.RegionEvents",
                c => new
                    {
                        Region_ID = c.Int(nullable: false),
                        Event_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Region_ID, t.Event_ID })
                .ForeignKey("dbo.Regions", t => t.Region_ID, cascadeDelete: true)
                .ForeignKey("dbo.Events", t => t.Event_ID, cascadeDelete: true)
                .Index(t => t.Region_ID)
                .Index(t => t.Event_ID);
            
            DropColumn("dbo.Regions", "Event_ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Regions", "Event_ID", c => c.Int());
            DropForeignKey("dbo.RegionEvents", "Event_ID", "dbo.Events");
            DropForeignKey("dbo.RegionEvents", "Region_ID", "dbo.Regions");
            DropIndex("dbo.RegionEvents", new[] { "Event_ID" });
            DropIndex("dbo.RegionEvents", new[] { "Region_ID" });
            DropTable("dbo.RegionEvents");
            CreateIndex("dbo.Regions", "Event_ID");
            AddForeignKey("dbo.Regions", "Event_ID", "dbo.Events", "ID");
        }
    }
}
