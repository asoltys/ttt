namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MultipleImpactsRegionsBranchs : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Impacts", "BranchID", "dbo.Branches");
            DropIndex("dbo.Impacts", new[] { "BranchID" });
            CreateTable(
                "dbo.ImpactBranches",
                c => new
                    {
                        Impact_ID = c.Int(nullable: false),
                        Branch_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Impact_ID, t.Branch_ID })
                .ForeignKey("dbo.Impacts", t => t.Impact_ID, cascadeDelete: true)
                .ForeignKey("dbo.Branches", t => t.Branch_ID, cascadeDelete: true)
                .Index(t => t.Impact_ID)
                .Index(t => t.Branch_ID);
            
            CreateTable(
                "dbo.RegionImpacts",
                c => new
                    {
                        Region_ID = c.Int(nullable: false),
                        Impact_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Region_ID, t.Impact_ID })
                .ForeignKey("dbo.Regions", t => t.Region_ID, cascadeDelete: true)
                .ForeignKey("dbo.Impacts", t => t.Impact_ID, cascadeDelete: true)
                .Index(t => t.Region_ID)
                .Index(t => t.Impact_ID);
            
            DropColumn("dbo.Impacts", "BranchID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Impacts", "BranchID", c => c.Int(nullable: false));
            DropForeignKey("dbo.RegionImpacts", "Impact_ID", "dbo.Impacts");
            DropForeignKey("dbo.RegionImpacts", "Region_ID", "dbo.Regions");
            DropForeignKey("dbo.ImpactBranches", "Branch_ID", "dbo.Branches");
            DropForeignKey("dbo.ImpactBranches", "Impact_ID", "dbo.Impacts");
            DropIndex("dbo.RegionImpacts", new[] { "Impact_ID" });
            DropIndex("dbo.RegionImpacts", new[] { "Region_ID" });
            DropIndex("dbo.ImpactBranches", new[] { "Branch_ID" });
            DropIndex("dbo.ImpactBranches", new[] { "Impact_ID" });
            DropTable("dbo.RegionImpacts");
            DropTable("dbo.ImpactBranches");
            CreateIndex("dbo.Impacts", "BranchID");
            AddForeignKey("dbo.Impacts", "BranchID", "dbo.Branches", "ID", cascadeDelete: true);
        }
    }
}
