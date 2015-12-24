namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EventEdits : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.RegionImpacts", newName: "ImpactRegions");
            DropPrimaryKey("dbo.ImpactRegions");
            CreateTable(
                "dbo.Edits",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        EditorID = c.String(maxLength: 128),
                        EventID = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.EditorID)
                .ForeignKey("dbo.Events", t => t.EventID, cascadeDelete: true)
                .Index(t => t.EditorID)
                .Index(t => t.EventID);
            
            AddPrimaryKey("dbo.ImpactRegions", new[] { "Impact_ID", "Region_ID" });
            DropColumn("dbo.AspNetUsers", "Discriminator");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            DropForeignKey("dbo.Edits", "EventID", "dbo.Events");
            DropForeignKey("dbo.Edits", "EditorID", "dbo.AspNetUsers");
            DropIndex("dbo.Edits", new[] { "EventID" });
            DropIndex("dbo.Edits", new[] { "EditorID" });
            DropPrimaryKey("dbo.ImpactRegions");
            DropTable("dbo.Edits");
            AddPrimaryKey("dbo.ImpactRegions", new[] { "Region_ID", "Impact_ID" });
            RenameTable(name: "dbo.ImpactRegions", newName: "RegionImpacts");
        }
    }
}
